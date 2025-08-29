using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 结束节点类，用于表示对话流程的结束点
/// 继承自BaseNode，提供结束节点的特定功能和属性
/// </summary>
public class EndNode : BaseNode
{
    private EndNodeType endNodeType = EndNodeType.End;
    private EnumField enumField;
    
    /// <summary>
    /// 获取或设置结束节点的类型
    /// </summary>
    public EndNodeType EndNodeType
    {
        get { return endNodeType; }
        set { endNodeType = value; }
    }

    /// <summary>
    /// 默认构造函数
    /// </summary>
    public EndNode()
    {
        
    }
    
    /// <summary>
    /// 带参数的构造函数，初始化结束节点的各项属性
    /// </summary>
    /// <param name="position">节点在图中的位置坐标</param>
    /// <param name="editorWindow">对话编辑器窗口引用</param>
    /// <param name="graphView">对话图表视图引用</param>
    public EndNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView)
    {
        // 设置基础引用
        base.editorWindow = editorWindow;
        base.graphView = graphView;

        // 初始化节点基本信息
        title = "End";
        SetPosition(new Rect(position, defaultNodeSize));
        nodeGuid = Guid.NewGuid().ToString();
        
        // 添加输入端口，支持多个连接
        AddInputPort("Input", Port.Capacity.Multi);

        // 创建枚举字段用于选择结束节点类型
        enumField = new EnumField()
        {
            value = endNodeType
        };
        
        enumField.Init(endNodeType);

        // 注册值变化回调，当枚举值改变时更新内部状态
        enumField.RegisterValueChangedCallback((value) =>
        {
            endNodeType = (EndNodeType)value.newValue;
        });
        enumField.SetValueWithoutNotify(endNodeType);
        
        // 将枚举字段添加到主容器中显示
        mainContainer.Add(enumField);
    }
    
    /// <summary>
    /// 重写方法，将当前值加载到字段中
    /// 用于在节点数据变更后刷新显示
    /// </summary>
    public override void LoadValueInToField()
    {
        enumField.SetValueWithoutNotify(endNodeType);
    }
}

