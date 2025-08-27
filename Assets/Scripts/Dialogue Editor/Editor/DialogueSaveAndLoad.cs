using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueSaveAndLoad
{
    private List<Edge> edges => graphView.edges.ToList();
    
    private List<BaseNode> nodes => graphView.nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();
    
    private DialogueGraphView graphView;
    
    public DialogueSaveAndLoad(DialogueGraphView _graphView)
    {
        graphView = _graphView;
    }

    public void Save(DialogueContainerSO _dialogueContainerSO)
    {
        SaveEdges(_dialogueContainerSO);
        SaveNodes(_dialogueContainerSO);
        
        EditorUtility.SetDirty(_dialogueContainerSO);
        AssetDatabase.SaveAssets();
    }
    
    public void Load(DialogueContainerSO _dialogueContainerSO)
    {
        ClearGraph();
        GenerateNodes(_dialogueContainerSO);
        ConnectNodes(_dialogueContainerSO);
    }

    #region Save

    private void SaveEdges(DialogueContainerSO _dialogueContainerSO)
    {
        _dialogueContainerSO.NodeLinkDatas.Clear();

        Edge[] connectedEdges = edges.Where(edge => edge.input.node != null).ToArray();
        for (int i = 0; i < connectedEdges.Count(); i++)
        {
            BaseNode outputNode = (BaseNode)connectedEdges[i].output.node;
            BaseNode inputNode = connectedEdges[i].input.node as BaseNode;
            
            _dialogueContainerSO.NodeLinkDatas.Add(new NodeLinkData()
            {
                BaseNodeGuid = outputNode.NodeGuid,
                TargetNodeGuid = inputNode.NodeGuid
            });
        }
    }

    public void SaveNodes(DialogueContainerSO _dialogueContainerSO)
    {
        // 修复数据清理错误
        _dialogueContainerSO.DialogueNodeDatas.Clear();
        _dialogueContainerSO.EndNodeDatas.Clear();
        _dialogueContainerSO.StartNodeDatas.Clear();
        _dialogueContainerSO.EventNodeDatas.Clear(); // 修复：原来是EndNodeDatas.Clear()调用了两次
        
        nodes.ForEach(node =>
        {
            switch (node)
            {
                case DialogueNode dialogueNode:
                    _dialogueContainerSO.DialogueNodeDatas.Add(SaveNodeData(dialogueNode));
                    break;
                case StartNode startNode:
                    _dialogueContainerSO.StartNodeDatas.Add(SaveNodeData(startNode));
                    break;
                case EndNode endNode:
                    _dialogueContainerSO.EndNodeDatas.Add(SaveNodeData(endNode));
                    break;
                case EventNode eventNode:
                    _dialogueContainerSO.EventNodeDatas.Add(SaveNodeData(eventNode));
                    break;
                default:
                    break;
            }
        });
        
    }

    private DialogueNodeData SaveNodeData(DialogueNode _node)
    {
        DialogueNodeData dialogueNodeData = new DialogueNodeData
        {
            NodeGuid = _node.NodeGuid,
            Position = _node.GetPosition().position,
            TextType = _node.Texts,
            Name = _node.Name,
            AudioClips = _node.AudioClips,
            DialogueFaceImageType = _node.FaceImageType,
            Sprite = _node.FaceImage,
            DialogueNodePorts = _node.dialogueNodePorts
        };

        // 修复：确保所有端口的连接信息都被正确保存
        foreach (DialogueNodePort nodePort in dialogueNodeData.DialogueNodePorts)
        {
            nodePort.OutputGuid = string.Empty;
            nodePort.InputGuid = string.Empty;
            
            // 遍历所有边来查找与这个端口的连接
            foreach (Edge edge in edges)
            {
                if (edge.output.viewDataKey == nodePort.PortId)
                {
                    nodePort.OutputGuid = (edge.output.node as BaseNode).NodeGuid;
                    nodePort.InputGuid = (edge.input.node as BaseNode).NodeGuid;
                }
            }
        }
        
        return dialogueNodeData;
    }

    private StartNodeData SaveNodeData(StartNode _node)
    {
        StartNodeData nodeData = new StartNodeData()
        {
            NodeGuid = _node.NodeGuid,
            Position = _node.GetPosition().position
        };
        
        return nodeData;
    }
    
    private EndNodeData SaveNodeData(EndNode _node)
    {
        EndNodeData nodeData = new EndNodeData()
        {
            NodeGuid = _node.NodeGuid,
            Position = _node.GetPosition().position,
            EndNodeType = _node.EndNodeType
        };
        
        return nodeData;
    }
    
    private EventNodeData SaveNodeData(EventNode _node)
    {
        EventNodeData nodeData = new EventNodeData()
        {
            NodeGuid = _node.NodeGuid,
            Position = _node.GetPosition().position,
            DialogueEventSO = _node.DialogueEvent
        };
        
        return nodeData;
    }
    

    #endregion
    
    #region Load

    private void ClearGraph()
    {
        edges.ForEach(edge => graphView.RemoveElement(edge));

        foreach (BaseNode node in nodes)
        {
            graphView.RemoveElement(node);
        }
    }

    private void GenerateNodes(DialogueContainerSO _dialogueContainer)
    {
        // Start
        foreach (StartNodeData node in _dialogueContainer.StartNodeDatas)
        {
            StartNode tempNode = graphView.CreateStartNode(node.Position);
            tempNode.NodeGuid = node.NodeGuid;
            
            graphView.AddElement(tempNode);
        }
        
        // End Node
        foreach (EndNodeData node in _dialogueContainer.EndNodeDatas)
        {
            EndNode tempNode = graphView.CreateEndNode(node.Position);
            tempNode.NodeGuid = node.NodeGuid;
            tempNode.EndNodeType = node.EndNodeType;
            
            tempNode.LoadValueInToField();
            graphView.AddElement(tempNode);
        }
        
        // Event Node
        foreach (EventNodeData node in _dialogueContainer.EventNodeDatas)
        {
            EventNode tempNode = graphView.CreateEventNode(node.Position);
            tempNode.NodeGuid = node.NodeGuid;
            tempNode.DialogueEvent = node.DialogueEventSO;
            
            tempNode.LoadValueInToField();
            graphView.AddElement(tempNode);
        }
        
        // Dialogue Node
        foreach (DialogueNodeData node in _dialogueContainer.DialogueNodeDatas)
        {
            DialogueNode tempNode = graphView.CreateDialogueNode(node.Position);
            tempNode.NodeGuid = node.NodeGuid;
            tempNode.Name = node.Name;
            tempNode.Texts = node.TextType;
            tempNode.FaceImage = node.Sprite;
            tempNode.FaceImageType = node.DialogueFaceImageType;
            tempNode.AudioClips = node.AudioClips;
            
            // 修复：确保所有端口都被正确添加
            foreach (DialogueNodePort nodePort in node.DialogueNodePorts)
            {
                tempNode.AddChoicePort(tempNode, nodePort);
            }
            
            tempNode.LoadValueInToField();
            graphView.AddElement(tempNode);
        }
    }

    private void ConnectNodes(DialogueContainerSO _dialogueContainer)
    {
        // 连接所有节点（包括对话节点）
        for (int i = 0; i < nodes.Count; i++)
        {
            List<NodeLinkData> connections = _dialogueContainer.NodeLinkDatas
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
    }

    // 通过索引查找对话节点的端口
    private Port FindPortByPortId(DialogueNode dialogueNode, int index)
    {
        if (index < dialogueNode.outputContainer.childCount)
        {
            VisualElement portElement = dialogueNode.outputContainer[index];
            return portElement.Q<Port>();
        }
        return null;
    }

    // 安全地获取输出端口
    private Port GetOutputPortAt(BaseNode node, int index)
    {
        if (node.outputContainer.childCount > index)
        {
            return node.outputContainer[index].Q<Port>();
        }
        return null;
    }

    // 安全地获取输入端口
    private Port GetInputPortAt(BaseNode node, int index)
    {
        if (node.inputContainer.childCount > index)
        {
            return node.inputContainer[index].Q<Port>();
        }
        return null;
    }

    private void LinkNodesTogether(Port _outputPort, Port _inputPort)
    {
        Edge tempEdge = new Edge()
        {
            output = _outputPort,
            input = _inputPort
        };
        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);
        graphView.Add(tempEdge);
    }
    
    #endregion
}
