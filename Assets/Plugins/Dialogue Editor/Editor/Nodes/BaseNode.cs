using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 基础节点类，继承自GraphView的Node类，为对话系统提供基本的节点功能
/// </summary>
public class BaseNode : Node
{
    protected DialogueGraphView graphView;
    protected DialogueEditorWindow editorWindow;
    protected Vector2 defaultNodeSize = new Vector2(200, 250);
    
    protected string nodeGuid;
    public string NodeGuid { get => nodeGuid; set => nodeGuid = value; }

    /// <summary>
    /// 基础节点构造函数，加载节点样式表
    /// </summary>
    public BaseNode()
    {
        StyleSheet styleSheet = Resources.Load<StyleSheet>("NodeStyleSheet");
        styleSheets.Add(styleSheet);
    }

    /// <summary>
    /// 添加输出端口到节点
    /// </summary>
    /// <param name="name">端口名称</param>
    /// <param name="capacity">端口容量，决定可以连接多少条线</param>
    public void AddOutputPort(string name, Port.Capacity capacity = Port.Capacity.Single)
    {
        Port outputPort = GetPortInstance(Direction.Output, capacity);
        outputPort.portName = name;
        outputContainer.Add(outputPort);
    }
    
    /// <summary>
    /// 添加输入端口到节点
    /// </summary>
    /// <param name="name">端口名称</param>
    /// <param name="capacity">端口容量，决定可以连接多少条线</param>
    public void AddInputPort(string name, Port.Capacity capacity = Port.Capacity.Multi)
    {
        Port inputPort = GetPortInstance(Direction.Input, capacity);
        inputPort.portName = name;
        inputContainer.Add(inputPort);
    }

    /// <summary>
    /// 创建端口实例
    /// </summary>
    /// <param name="nodeDirection">端口方向（输入或输出）</param>
    /// <param name="capacity">端口容量</param>
    /// <returns>创建的端口实例</returns>
    public Port GetPortInstance(Direction nodeDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
    }

    /// <summary>
    /// 虚方法，用于将值加载到字段中，子类需要重写实现具体逻辑
    /// </summary>
    public virtual void LoadValueInToField()
    {
        
    }
    
    /// <summary>
    /// 虚方法，用于重新加载语言设置，子类需要重写实现具体逻辑
    /// </summary>
    public virtual void ReloadLanguage()
    {
        
    }
}

