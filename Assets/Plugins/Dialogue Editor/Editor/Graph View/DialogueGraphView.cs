using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 对话框图视图类，继承自GraphView，用于在Unity编辑器中可视化和编辑对话流程图。
/// </summary>
public class DialogueGraphView : GraphView
{
    private string styleSheetsName = "GraphViewStyleSheet";
    private DialogueEditorWindow editorWindow;
    private NodeSearchWindow searchWindow;
    
    /// <summary>
    /// 构造函数，初始化对话框图视图的基本设置。
    /// </summary>
    /// <param name="editorWindow">关联的对话编辑器窗口实例。</param>
    public DialogueGraphView(DialogueEditorWindow editorWindow)
    {
        this.editorWindow = editorWindow;

        // 设置缩放范围
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        // 加载并应用样式表
        StyleSheet tmpStyleSheet = Resources.Load<StyleSheet>(styleSheetsName);
        styleSheets.Add(tmpStyleSheet);
        
        // 添加各种操作器以支持交互功能
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new FreehandSelector());
        
        // 插入网格背景并拉伸至父容器大小
        GridBackground grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        AddSearchWindow();
    }

    /// <summary>
    /// 添加节点搜索窗口，配置其上下文并绑定到节点创建请求事件。
    /// </summary>
    private void AddSearchWindow()
    {
        searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        searchWindow.Configure(editorWindow, this);
        nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
    }

    /// <summary>
    /// 获取与指定端口兼容的其他端口列表。
    /// 端口必须满足以下条件：
    /// - 不是同一个端口；
    /// - 不属于同一节点；
    /// - 方向相反（输入/输出）。
    /// </summary>
    /// <param name="startPort">起始端口。</param>
    /// <param name="nodeAdapter">节点适配器（未使用）。</param>
    /// <returns>兼容端口的列表。</returns>
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new List<Port>();
        Port startPortView = startPort;

        ports.ForEach((port) =>
        {
            Port portView = port;
            
            if (startPortView != portView && startPortView.node != portView.node && startPortView.direction != portView.direction)
            {
                compatiblePorts.Add(port);
            }
        });
        
        return compatiblePorts;
    }
    
    /// <summary>
    /// 重新加载所有对话节点的语言内容。
    /// 遍历当前图中的所有BaseNode类型的节点，并调用其ReloadLanguage方法。
    /// </summary>
    public void ReloadLanguage()
    {
        List<BaseNode> dialogueNodes = nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();
        foreach (BaseNode dialogueNode in dialogueNodes)
        {
            dialogueNode.ReloadLanguage();
        }
    }

    /// <summary>
    /// 创建一个起始节点并返回。
    /// </summary>
    /// <param name="pos">节点的位置。</param>
    /// <returns>新创建的StartNode实例。</returns>
    public StartNode CreateStartNode(Vector2 pos)
    {
        StartNode tmp = new StartNode(pos, editorWindow, this);

        return tmp;
    }
    
    /// <summary>
    /// 创建一个对话节点并返回。
    /// </summary>
    /// <param name="pos">节点的位置。</param>
    /// <returns>新创建的DialogueNode实例。</returns>
    public DialogueNode CreateDialogueNode(Vector2 pos)
    {
        DialogueNode tmp = new DialogueNode(pos, editorWindow, this);

        return tmp;
    }

    /// <summary>
    /// 创建一个事件节点并返回。
    /// </summary>
    /// <param name="pos">节点的位置。</param>
    /// <returns>新创建的EventNode实例。</returns>
    public EventNode CreateEventNode(Vector2 pos)
    {
        EventNode tmp = new EventNode(pos, editorWindow, this);
        
        return tmp;
    }
    
    /// <summary>
    /// 创建一个结束节点并返回。
    /// </summary>
    /// <param name="pos">节点的位置。</param>
    /// <returns>新创建的EndNode实例。</returns>
    public EndNode CreateEndNode(Vector2 pos)
    {
        EndNode tmp = new EndNode(pos, editorWindow, this);
        
        return tmp;
    }
}
