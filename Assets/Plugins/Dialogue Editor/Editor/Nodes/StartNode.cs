using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// StartNode类表示对话系统中的起始节点，继承自BaseNode基类。
/// 该节点作为对话流程的入口点，只能有一个输出端口连接到其他节点。
/// </summary>
public class StartNode : BaseNode
{
    /// <summary>
    /// 无参构造函数，用于初始化StartNode实例
    /// </summary>
    public StartNode()
    {
        
    }

    /// <summary>
    /// 带参数的构造函数，用于创建并初始化StartNode节点
    /// </summary>
    /// <param name="position">节点在图视图中的位置坐标</param>
    /// <param name="editorWindow">对话编辑器窗口引用</param>
    /// <param name="graphView">对话图表视图引用</param>
    public StartNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView)
    {
        // 设置编辑器窗口和图表视图引用
        base.editorWindow = editorWindow;
        base.graphView = graphView;
        
        // 初始化节点标题和位置
        title = "Start";
        SetPosition(new Rect(position, defaultNodeSize));
        nodeGuid = Guid.NewGuid().ToString();
        
        // 添加输出端口，容量为单连接
        AddOutputPort("Output", Port.Capacity.Single);
        
        // 刷新节点的展开状态和端口显示
        RefreshExpandedState();
        RefreshPorts();
    }
}

