using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 事件节点类，用于在对话编辑器中表示和处理对话事件
/// </summary>
public class EventNode : BaseNode
{
    private DialogueEventSO dialogueEvent;
    private ObjectField objectField;
    
    /// <summary>
    /// 获取或设置对话事件对象
    /// </summary>
    public DialogueEventSO DialogueEvent
    {
        get { return dialogueEvent; }
        set { dialogueEvent = value; }
    }
    
    /// <summary>
    /// 无参构造函数
    /// </summary>
    public EventNode()
    { 
    }
    
    /// <summary>
    /// 带参数的构造函数，用于初始化事件节点
    /// </summary>
    /// <param name="position">节点在图中的位置</param>
    /// <param name="editorWindow">对话编辑器窗口引用</param>
    /// <param name="graphView">对话图表视图引用</param>
    public EventNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView)
    {
        base.editorWindow = editorWindow;
        base.graphView = graphView;
        
        title =  "Event";
        SetPosition(new Rect(position, defaultNodeSize));
        nodeGuid = Guid.NewGuid().ToString();
        
        // 添加输入和输出端口
        AddInputPort("Input", Port.Capacity.Multi);
        AddOutputPort("Output", Port.Capacity.Single);

        // 创建对象字段用于选择对话事件ScriptableObject
        objectField = new ObjectField()
        {
            objectType = typeof(DialogueEventSO),
            allowSceneObjects = false,
            value = dialogueEvent
        };
        
        // 注册值变化回调，当选择的对象改变时更新内部引用
        objectField.RegisterValueChangedCallback(value =>
        {
            dialogueEvent = objectField.value as DialogueEventSO;
        });
        objectField.SetValueWithoutNotify(dialogueEvent);
        
        mainContainer.Add(objectField);
    }
    
    /// <summary>
    /// 重写方法，将当前值加载到字段中
    /// </summary>
    public override void LoadValueInToField()
    {
        objectField.SetValueWithoutNotify(dialogueEvent);
    }
}

