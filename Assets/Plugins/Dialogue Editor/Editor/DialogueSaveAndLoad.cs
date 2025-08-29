using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 对话系统中用于保存和加载对话图数据的类。
/// 负责将图中的节点和连接关系序列化到 ScriptableObject 中，或从其中反序列化回图结构。
/// </summary>
public class DialogueSaveAndLoad
{
    /// <summary>
    /// 获取当前图视图中所有边（连接）的列表。
    /// </summary>
    private List<Edge> edges => graphView.edges.ToList();
    
    /// <summary>
    /// 获取当前图视图中所有基础节点的列表。
    /// </summary>
    private List<BaseNode> nodes => graphView.nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();
    
    /// <summary>
    /// 当前操作的对话图视图实例。
    /// </summary>
    private DialogueGraphView graphView;
    
    /// <summary>
    /// 构造函数，初始化对话图视图引用。
    /// </summary>
    /// <param name="graphView">要操作的对话图视图实例。</param>
    public DialogueSaveAndLoad(DialogueGraphView graphView)
    {
        this.graphView = graphView;
    }

    /// <summary>
    /// 将当前图中的节点和连接信息保存到指定的 ScriptableObject 容器中。
    /// </summary>
    /// <param name="dialogueContainerSO">用于存储对话数据的 ScriptableObject 容器。</param>
    public void Save(DialogueContainerSO dialogueContainerSO)
    {
        SaveEdges(dialogueContainerSO);
        SaveNodes(dialogueContainerSO);
        
        EditorUtility.SetDirty(dialogueContainerSO);
        AssetDatabase.SaveAssets();
    }
    
    /// <summary>
    /// 从指定的 ScriptableObject 容器中加载对话图数据，并重建图结构。
    /// </summary>
    /// <param name="dialogueContainerSO">包含对话图数据的 ScriptableObject 容器。</param>
    public void Load(DialogueContainerSO dialogueContainerSO)
    {
        ClearGraph();
        GenerateNodes(dialogueContainerSO);
        ConnectNodes(dialogueContainerSO);
    }

    #region Save

    /// <summary>
    /// 保存图中所有连接边的信息到 ScriptableObject 容器。
    /// </summary>
    /// <param name="dialogueContainerSO">用于存储连接数据的 ScriptableObject 容器。</param>
    private void SaveEdges(DialogueContainerSO dialogueContainerSO)
    {
        dialogueContainerSO.NodeLinkDatas.Clear();

        Edge[] connectedEdges = edges.Where(edge => edge.input.node != null).ToArray();
        for (int i = 0; i < connectedEdges.Count(); i++)
        {
            BaseNode outputNode = (BaseNode)connectedEdges[i].output.node;
            BaseNode inputNode = connectedEdges[i].input.node as BaseNode;
            
            dialogueContainerSO.NodeLinkDatas.Add(new NodeLinkData()
            {
                BaseNodeGuid = outputNode.NodeGuid,
                TargetNodeGuid = inputNode.NodeGuid
            });
        }
    }

    /// <summary>
    /// 保存图中所有节点的数据到 ScriptableObject 容器。
    /// </summary>
    /// <param name="dialogueContainerSO">用于存储节点数据的 ScriptableObject 容器。</param>
    public void SaveNodes(DialogueContainerSO dialogueContainerSO)
    {
        dialogueContainerSO.DialogueNodeDatas.Clear();
        dialogueContainerSO.EndNodeDatas.Clear();
        dialogueContainerSO.StartNodeDatas.Clear();
        dialogueContainerSO.EventNodeDatas.Clear();
        
        nodes.ForEach(node =>
        {
            switch (node)
            {
                case DialogueNode dialogueNode:
                    dialogueContainerSO.DialogueNodeDatas.Add(SaveNodeData(dialogueNode));
                    break;
                case StartNode startNode:
                    dialogueContainerSO.StartNodeDatas.Add(SaveNodeData(startNode));
                    break;
                case EndNode endNode:
                    dialogueContainerSO.EndNodeDatas.Add(SaveNodeData(endNode));
                    break;
                case EventNode eventNode:
                    dialogueContainerSO.EventNodeDatas.Add(SaveNodeData(eventNode));
                    break;
                default:
                    break;
            }
        });
        
    }

    /// <summary>
    /// 保存对话节点的数据。
    /// 包括 GUID、位置、文本、音频、头像等信息，并处理端口连接关系。
    /// </summary>
    /// <param name="node">要保存的对话节点。</param>
    /// <returns>保存后的对话节点数据对象。</returns>
    private DialogueNodeData SaveNodeData(DialogueNode node)
    {
        DialogueNodeData dialogueNodeData = new DialogueNodeData
        {
            NodeGuid = node.NodeGuid,
            Position = node.GetPosition().position,
            TextLanguages = node.Texts,
            Name = node.Name,
            AudioClips = node.AudioClips,
            DialogueFaceImageType = node.FaceImageType,
            Sprite = node.FaceImage,
            DialogueNodePorts = new List<DialogueNodePort>() // 创建空列表，避免引用问题
        };

        // 正确复制端口信息，包括 PortGuid
        foreach (DialogueNodePort originalPort in node.dialogueNodePorts)
        {
            DialogueNodePort newPort = new DialogueNodePort
            {
                PortGuid = originalPort.PortGuid,
                PortId = originalPort.PortId,
                TextFieldId = originalPort.TextFieldId,
                InputGuid = originalPort.InputGuid,
                OutputGuid = originalPort.OutputGuid,
                TextLanguages = new List<LanguageGeneric<string>>()
            };
            
            // 复制文本语言信息
            foreach (LanguageGeneric<string> textLanguage in originalPort.TextLanguages)
            {
                newPort.TextLanguages.Add(new LanguageGeneric<string>
                {
                    LanguageType = textLanguage.LanguageType,
                    LanguageGenericType = textLanguage.LanguageGenericType
                });
            }
            
            dialogueNodeData.DialogueNodePorts.Add(newPort);
        }

        // 确保所有端口的连接信息都被正确更新
        foreach (DialogueNodePort nodePort in dialogueNodeData.DialogueNodePorts)
        {
            // 重置连接信息
            nodePort.OutputGuid = string.Empty;
            nodePort.InputGuid = string.Empty;
    
            // 遍历所有边来查找与这个端口的连接
            foreach (Edge edge in edges)
            {
                // 使用PortId来匹配端口，而不是索引
                if (edge.output.node != null && edge.output.node is DialogueNode outputNode && 
                    edge.output.node == node && edge.output.viewDataKey == nodePort.PortId)
                {
                    // 确保输入节点存在并且是BaseNode类型
                    if (edge.input.node is BaseNode inputNode)
                    {
                        nodePort.OutputGuid = inputNode.NodeGuid; // 改为设置OutputGuid
                    }
                }
            }
        }

        
        return dialogueNodeData;
    }

    /// <summary>
    /// 保存开始节点的数据。
    /// </summary>
    /// <param name="node">要保存的开始节点。</param>
    /// <returns>保存后的开始节点数据对象。</returns>
    private StartNodeData SaveNodeData(StartNode node)
    {
        StartNodeData nodeData = new StartNodeData()
        {
            NodeGuid = node.NodeGuid,
            Position = node.GetPosition().position
        };
        
        return nodeData;
    }
    
    /// <summary>
    /// 保存结束节点的数据。
    /// </summary>
    /// <param name="node">要保存的结束节点。</param>
    /// <returns>保存后的结束节点数据对象。</returns>
    private EndNodeData SaveNodeData(EndNode node)
    {
        EndNodeData nodeData = new EndNodeData()
        {
            NodeGuid = node.NodeGuid,
            Position = node.GetPosition().position,
            EndNodeType = node.EndNodeType
        };
        
        return nodeData;
    }
    
    /// <summary>
    /// 保存事件节点的数据。
    /// </summary>
    /// <param name="node">要保存的事件节点。</param>
    /// <returns>保存后的事件节点数据对象。</returns>
    private EventNodeData SaveNodeData(EventNode node)
    {
        EventNodeData nodeData = new EventNodeData()
        {
            NodeGuid = node.NodeGuid,
            Position = node.GetPosition().position,
            DialogueEventSO = node.DialogueEvent
        };
        
        return nodeData;
    }
    
    #endregion
    
    #region Load

    /// <summary>
    /// 清除当前图中的所有节点和连接。
    /// </summary>
    private void ClearGraph()
    {
        edges.ForEach(edge => graphView.RemoveElement(edge));

        foreach (BaseNode node in nodes)
        {
            graphView.RemoveElement(node);
        }
    }

    /// <summary>
    /// 根据 ScriptableObject 中的数据生成图中的节点。
    /// </summary>
    /// <param name="dialogueContainer">包含节点数据的 ScriptableObject 容器。</param>
    private void GenerateNodes(DialogueContainerSO dialogueContainer)
    {
        // Start
        foreach (StartNodeData node in dialogueContainer.StartNodeDatas)
        {
            StartNode tempNode = graphView.CreateStartNode(node.Position);
            tempNode.NodeGuid = node.NodeGuid;
            
            graphView.AddElement(tempNode);
        }
        
        // End Node
        foreach (EndNodeData node in dialogueContainer.EndNodeDatas)
        {
            EndNode tempNode = graphView.CreateEndNode(node.Position);
            tempNode.NodeGuid = node.NodeGuid;
            tempNode.EndNodeType = node.EndNodeType;
            
            tempNode.LoadValueInToField();
            graphView.AddElement(tempNode);
        }
        
        // Event Node
        foreach (EventNodeData node in dialogueContainer.EventNodeDatas)
        {
            EventNode tempNode = graphView.CreateEventNode(node.Position);
            tempNode.NodeGuid = node.NodeGuid;
            tempNode.DialogueEvent = node.DialogueEventSO;
            
            tempNode.LoadValueInToField();
            graphView.AddElement(tempNode);
        }
        
        // Dialogue Node
        foreach (DialogueNodeData node in dialogueContainer.DialogueNodeDatas)
        {
            DialogueNode tempNode = graphView.CreateDialogueNode(node.Position);
            tempNode.NodeGuid = node.NodeGuid;
            tempNode.Name = node.Name;
            tempNode.FaceImage = node.Sprite;
            tempNode.FaceImageType = node.DialogueFaceImageType;

            // 正确设置文本和音频数据
            foreach (LanguageGeneric<string> languageGeneric in node.TextLanguages)
            {
                var targetText = tempNode.Texts.Find(language => language.LanguageType == languageGeneric.LanguageType);
                if (targetText != null)
                {
                    targetText.LanguageGenericType = languageGeneric.LanguageGenericType;
                }
            }
            
            foreach (LanguageGeneric<AudioClip> languageGeneric in node.AudioClips)
            {
                var targetAudio = tempNode.AudioClips.Find(language => language.LanguageType == languageGeneric.LanguageType);
                if (targetAudio != null)
                {
                    targetAudio.LanguageGenericType = languageGeneric.LanguageGenericType;
                }
            }
            
            // 关键修改：先清空现有的端口，避免重复创建
            tempNode.dialogueNodePorts.Clear();
            
            // 然后添加端口
            foreach (DialogueNodePort nodePort in node.DialogueNodePorts)
            {
                tempNode.AddChoicePort(tempNode, nodePort);
            }
            
            tempNode.LoadValueInToField();
            graphView.AddElement(tempNode);
        }
    }

    /// <summary>
    /// 根据 ScriptableObject 中的连接数据连接图中的节点。
    /// </summary>
    /// <param name="dialogueContainer">包含连接数据的 ScriptableObject 容器。</param>
    private void ConnectNodes(DialogueContainerSO dialogueContainer)
    {
        // 先连接所有节点
        for (int i = 0; i < nodes.Count; i++)
        {
            List<NodeLinkData> connections = dialogueContainer.NodeLinkDatas
                .Where(edge => edge.BaseNodeGuid == nodes[i].NodeGuid).ToList();

            for (int j = 0; j < connections.Count; j++)
            {
                string targetNodeGuid = connections[j].TargetNodeGuid;
                BaseNode targetNode = nodes.FirstOrDefault(node => node.NodeGuid == targetNodeGuid);

                if (targetNode != null)
                {
                    // 获取输出端口
                    Port outputPort = null;
                    
                    // 对于对话节点，需要特殊处理
                    if (nodes[i] is DialogueNode dialogueNode)
                    {
                        // 通过PortId查找对应的Port对象
                        outputPort = FindPortByPortId(dialogueNode, j);
                    }
                    else
                    {
                        // 对于非对话节点，使用索引查找端口
                        outputPort = GetOutputPortAt(nodes[i], j);
                    }
                    
                    // 获取输入端口
                    Port inputPort = GetInputPortAt(targetNode, 0);
                    
                    if (outputPort != null && inputPort != null)
                    {
                        LinkNodesTogether(outputPort, inputPort);
                    }
                }
            }
        }
        
        // 特殊处理：确保对话节点的端口连接信息正确保存
        foreach (BaseNode node in nodes)
        {
            if (node is DialogueNode dialogueNode)
            {
                // 重新加载对话节点的端口连接信息
                dialogueNode.LoadValueInToField();
            }
        }
    }

    /// <summary>
    /// 通过 PortId 查找对话节点的输出端口。
    /// </summary>
    /// <param name="dialogueNode">目标对话节点。</param>
    /// <param name="index">端口索引。</param>
    /// <returns>找到的端口对象，若未找到则返回 null。</returns>
    private Port FindPortByPortId(DialogueNode dialogueNode, int index)
    {
        // 首先尝试通过索引和PortId匹配查找
        if (index < dialogueNode.dialogueNodePorts.Count)
        {
            string portId = dialogueNode.dialogueNodePorts[index].PortId;
            // 在节点的输出容器中查找匹配PortId的端口
            foreach (var child in dialogueNode.outputContainer.Children())
            {
                Port port = child.Q<Port>();
                if (port != null && port.viewDataKey == portId)
                {
                    return port;
                }
            }
        }
        
        // 如果通过索引查找失败，尝试遍历所有端口
        if (index < dialogueNode.outputContainer.childCount)
        {
            VisualElement portElement = dialogueNode.outputContainer[index];
            return portElement.Q<Port>();
        }
        
        return null;
    }

    /// <summary>
    /// 安全地获取指定节点的输出端口。
    /// </summary>
    /// <param name="node">目标节点。</param>
    /// <param name="index">端口索引。</param>
    /// <returns>找到的端口对象，若未找到则返回 null。</returns>
    private Port GetOutputPortAt(BaseNode node, int index)
    {
        if (node.outputContainer.childCount > index)
        {
            return node.outputContainer[index].Q<Port>();
        }
        return null;
    }

    /// <summary>
    /// 安全地获取指定节点的输入端口。
    /// </summary>
    /// <param name="node">目标节点。</param>
    /// <param name="index">端口索引。</param>
    /// <returns>找到的端口对象，若未找到则返回 null。</returns>
    private Port GetInputPortAt(BaseNode node, int index)
    {
        if (node.inputContainer.childCount > index)
        {
            return node.inputContainer[index].Q<Port>();
        }
        return null;
    }

    /// <summary>
    /// 连接两个端口。
    /// </summary>
    /// <param name="outputPort">输出端口。</param>
    /// <param name="inputPort">输入端口。</param>
    private void LinkNodesTogether(Port outputPort, Port inputPort)
    {
        Edge tempEdge = new Edge()
        {
            output = outputPort,
            input = inputPort
        };
        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);
        graphView.Add(tempEdge);
    }
    
    #endregion
}
