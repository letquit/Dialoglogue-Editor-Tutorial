using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 对话节点类，继承自BaseNode，用于在对话编辑器中表示一个对话节点。
/// 包含文本、音频、角色头像等信息，并支持多语言。
/// </summary>
public class DialogueNode : BaseNode
{
    private List<LanguageGeneric<string>> texts = new List<LanguageGeneric<string>>();
    private List<LanguageGeneric<AudioClip>> audioClips = new List<LanguageGeneric<AudioClip>>();
    private new string name = "";
    private Sprite faceImage;
    private DialogueFaceImageType faceImageType;
    
    
    // 存储TextField引用的字典，键为TextFieldId
    private Dictionary<string, TextField> portTextFields = new Dictionary<string, TextField>();


    public List<DialogueNodePort> dialogueNodePorts = new List<DialogueNodePort>();
    
    public List<LanguageGeneric<string>> Texts
    {
        get { return texts; }
        set { texts = value; }
    }
    public List<LanguageGeneric<AudioClip>> AudioClips
    {
        get { return audioClips; }
        set { audioClips = value; }
    }
    public string Name
    {
        get { return name; }
        set { name = value; }
    }
    public Sprite FaceImage
    {
        get { return faceImage; }
        set { faceImage = value; }
    }
    public DialogueFaceImageType FaceImageType
    {
        get { return faceImageType; }
        set { faceImageType = value; }
    }

    private TextField texts_Field;
    private ObjectField audioClips_Field;
    private ObjectField faceImage_Field;
    private Image faceImagePreview;
    private TextField name_Field;
    private EnumField faceImageType_Field;
    
    /// <summary>
    /// 默认构造函数
    /// </summary>
    public DialogueNode()
    { 
    }
    
    /// <summary>
    /// 构造函数，初始化对话节点的基本信息和UI元素
    /// </summary>
    /// <param name="position">节点在图中的位置</param>
    /// <param name="editorWindow">对话编辑器窗口</param>
    /// <param name="graphView">对话图视图</param>
    public DialogueNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView)
    {
        base.editorWindow = editorWindow;
        base.graphView = graphView;
    
        title = "Dialogue";
        SetPosition(new Rect(position, defaultNodeSize));
        nodeGuid = Guid.NewGuid().ToString();
    
        AddInputPort("Input", Port.Capacity.Multi);

        foreach (LanguageType language in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
        {
            texts.Add(new LanguageGeneric<string>
            {
                LanguageType = language,
                LanguageGenericType = ""
            });
        
            audioClips.Add(new LanguageGeneric<AudioClip>
            {
                LanguageType = language,
                LanguageGenericType = null
            });
        }

        // Face Image
        faceImage_Field = new ObjectField
        {
            objectType = typeof(Sprite),
            allowSceneObjects = false,
            value = faceImage
        };
        
        faceImagePreview = new Image();
        faceImagePreview.AddToClassList("faceImagePreview");
        
        faceImage_Field.RegisterValueChangedCallback(value =>
        {
            Sprite tmp = value.newValue as Sprite;
            faceImage = tmp;
            faceImagePreview.image = (tmp != null ? tmp.texture : null);
        });
        mainContainer.Add(faceImagePreview);
        mainContainer.Add(faceImage_Field);
        
        // Face Image Enum
        faceImageType_Field = new EnumField()
        {
            value = faceImageType
        };
        faceImageType_Field.Init(faceImageType);
        faceImageType_Field.RegisterValueChangedCallback(value =>
        {
            faceImageType = (DialogueFaceImageType)value.newValue;
        });
        mainContainer.Add(faceImageType_Field);
        
        // Audio Clips
        audioClips_Field = new ObjectField
        {
            objectType = typeof(AudioClip),
            allowSceneObjects = false,
            value = audioClips.Find(audioClip => audioClip.LanguageType == base.editorWindow.LanguageType).LanguageGenericType
        };
        audioClips_Field.RegisterValueChangedCallback(value =>
        {
            audioClips.Find(audioClip => audioClip.LanguageType == base.editorWindow.LanguageType).LanguageGenericType = value.newValue as AudioClip;
        });
        audioClips_Field.SetValueWithoutNotify(audioClips.Find(audioClip => audioClip.LanguageType == base.editorWindow.LanguageType).LanguageGenericType);
        mainContainer.Add(audioClips_Field);

        // Text Name
        Label label_name = new Label("Name");
        label_name.AddToClassList("label_name");
        label_name.AddToClassList("Label");
        mainContainer.Add(label_name);
        
        name_Field = new TextField("");
        name_Field.RegisterValueChangedCallback(value =>
        {
            name = value.newValue;
        });
        name_Field.SetValueWithoutNotify(name);
        name_Field.AddToClassList("TextName");
        mainContainer.Add(name_Field);
        
        // Text Box
        Label label_texts = new Label("Text Box");
        label_texts.AddToClassList("label_texts");
        label_texts.AddToClassList("Label");
        mainContainer.Add(label_texts);
        
        texts_Field = new TextField("");
        texts_Field.RegisterValueChangedCallback(value =>
        {
            texts.Find(text => text.LanguageType == base.editorWindow.LanguageType).LanguageGenericType = value.newValue;
        });
        texts_Field.SetValueWithoutNotify(texts.Find(text => text.LanguageType == base.editorWindow.LanguageType).LanguageGenericType);
        texts_Field.multiline = true;
        
        texts_Field.AddToClassList("TextBox");
        mainContainer.Add(texts_Field);

        Button button = new Button()
        {
            text = "Add Choice"
        };
        button.clicked += () =>
        {
            AddChoicePort(this);
        };
        
        titleButtonContainer.Add(button);
    }
    
    /// <summary>
    /// 重新加载语言设置，更新文本和音频字段的值
    /// </summary>
    public override void ReloadLanguage()
    { 
        texts_Field.RegisterValueChangedCallback(value =>
        {
            texts.Find(text => text.LanguageType == editorWindow.LanguageType).LanguageGenericType = value.newValue;
        });
        texts_Field.SetValueWithoutNotify(texts.Find(text => text.LanguageType == editorWindow.LanguageType).LanguageGenericType);

        audioClips_Field.RegisterValueChangedCallback(value =>
        {
            audioClips.Find(audioClip => audioClip.LanguageType == editorWindow.LanguageType).LanguageGenericType = value.newValue as AudioClip;
        });
        audioClips_Field.SetValueWithoutNotify(audioClips.Find(audioClip => audioClip.LanguageType == editorWindow.LanguageType).LanguageGenericType);

        // 修改后的代码：通过TextFieldId从字典中获取TextField引用
        foreach (DialogueNodePort nodePort in dialogueNodePorts)
        {
            if (portTextFields.ContainsKey(nodePort.TextFieldId))
            {
                TextField textField = portTextFields[nodePort.TextFieldId];
                textField.RegisterValueChangedCallback(value =>
                { 
                    nodePort.TextLanguages.Find(text => text.LanguageType == editorWindow.LanguageType).LanguageGenericType = value.newValue;
                });
                textField.SetValueWithoutNotify(nodePort.TextLanguages.Find(language => language.LanguageType == editorWindow.LanguageType).LanguageGenericType);
            }
        }
    }

    /// <summary>
    /// 将当前节点的数据加载到对应的UI字段中
    /// </summary>
    public override void LoadValueInToField()
    {
        texts_Field.SetValueWithoutNotify(texts.Find(language => language.LanguageType == editorWindow.LanguageType).LanguageGenericType);
        audioClips_Field.SetValueWithoutNotify(audioClips.Find(language => language.LanguageType == editorWindow.LanguageType).LanguageGenericType);
        faceImage_Field.SetValueWithoutNotify(faceImage);
        faceImageType_Field.SetValueWithoutNotify(FaceImageType);
        name_Field.SetValueWithoutNotify(Name);

        if (faceImage != null)
        {
            faceImagePreview.image = ((Sprite)faceImage_Field.value).texture;
        }
        
        // 更新端口连接信息
        UpdatePortConnections();
    }

    /// <summary>
    /// 更新所有输出端口的连接信息
    /// </summary>
    private void UpdatePortConnections()
    {
        // 确保所有端口的连接信息是最新的
        foreach (DialogueNodePort nodePort in dialogueNodePorts)
        {
            // 查找与该端口连接的边
            foreach (Edge edge in graphView.edges)
            {
                if (edge.output.node == this && edge.output.viewDataKey == nodePort.PortId)
                {
                    if (edge.input.node is BaseNode inputNode)
                    {
                        nodePort.OutputGuid = inputNode.NodeGuid; // 修改这里：设置OutputGuid而不是InputGuid
                    }
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 添加一个选择端口到当前节点
    /// </summary>
    /// <param name="baseNode">要添加端口的基础节点</param>
    /// <param name="dialogueNodePort">可选的对话节点端口，用于复用已有端口信息</param>
    /// <returns>新创建的端口</returns>
    public Port AddChoicePort(BaseNode baseNode, DialogueNodePort dialogueNodePort = null)
    {
        Port port = GetPortInstance(Direction.Output);

        int outputPortCount = baseNode.outputContainer.Query("connector").ToList().Count();
        string outputPortName = "Continue";

        DialogueNodePort nodePort = new DialogueNodePort();
        nodePort.PortGuid = Guid.NewGuid().ToString();
        
        // 确保在任何条件下都设置PortId
        if (dialogueNodePort != null)
        {
            // 复用已存在的PortId
            nodePort.PortId = dialogueNodePort.PortId;
            
            // 检查是否已经存在相同ID的端口，避免重复添加
            if (dialogueNodePorts.Any(p => p.PortId == dialogueNodePort.PortId))
            {
                // 如果已存在，直接返回null或跳过
                return null;
            }
        }
        else
        {
            // 创建新的PortId
            nodePort.PortId = Guid.NewGuid().ToString();
        }

        foreach (LanguageType language in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
        {
            nodePort.TextLanguages.Add(new LanguageGeneric<string>
            {
                LanguageType = language,
                LanguageGenericType = outputPortName
            });
        }

        if (dialogueNodePort != null)
        {
            nodePort.InputGuid = dialogueNodePort.InputGuid;
            nodePort.OutputGuid = dialogueNodePort.OutputGuid;
            nodePort.PortGuid = dialogueNodePort.PortGuid;

            foreach (LanguageGeneric<string> languageGeneric in dialogueNodePort.TextLanguages)
            {
                nodePort.TextLanguages.Find(language => language.LanguageType == languageGeneric.LanguageType).LanguageGenericType = languageGeneric.LanguageGenericType;
            }
        }

        nodePort.TextFieldId = Guid.NewGuid().ToString();
        
        TextField textField = new TextField();
        textField.RegisterValueChangedCallback(value =>
        {
            nodePort.TextLanguages.Find(language => language.LanguageType == editorWindow.LanguageType).LanguageGenericType = value.newValue;
        });
        textField.SetValueWithoutNotify(nodePort.TextLanguages.Find(language => language.LanguageType == editorWindow.LanguageType).LanguageGenericType);
        
        // 将TextField存储在字典中
        portTextFields[nodePort.TextFieldId] = textField;
        
        // 将TextField添加到port的内容容器中
        port.contentContainer.Add(textField);
        
        //Delete button
        Button deleteButton = new Button(() => DeletePort(baseNode, port))
        {
            text = "X"
        };
        port.contentContainer.Add(deleteButton);

        port.portName = "";
        
        dialogueNodePorts.Add(nodePort);
        
        // 设置端口的viewDataKey，这很重要，用于保存和加载时识别端口
        port.viewDataKey = nodePort.PortId;
        
        baseNode.outputContainer.Add(port);
        
        //Refresh
        baseNode.RefreshPorts();
        baseNode.RefreshExpandedState();
        
        return port;
    }




    /// <summary>
    /// 删除指定的端口及其相关数据
    /// </summary>
    /// <param name="node">要删除端口的节点</param>
    /// <param name="port">要删除的端口</param>
    private void DeletePort(BaseNode node, Port port)
    {
        DialogueNodePort tmp = dialogueNodePorts.Find(nodePort => nodePort.PortId == port.viewDataKey);
        if (tmp != null)
        {
            // 从字典中移除TextField引用
            if (portTextFields.ContainsKey(tmp.TextFieldId))
            {
                portTextFields.Remove(tmp.TextFieldId);
            }
            
            dialogueNodePorts.Remove(tmp);
        }

        IEnumerable<Edge> portEdge = graphView.edges.ToList().Where(edge => edge.output == port);

        if (portEdge.Any())
        {
            Edge edge = portEdge.First();
            edge.input.Disconnect(edge);
            edge.output.Disconnect(edge);
            graphView.RemoveElement(edge);
        }
        
        node.outputContainer.Remove(port);
        
        // Refresh
        node.RefreshPorts();
        node.RefreshExpandedState();
    }
}

