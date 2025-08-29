using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 节点搜索窗口类，用于在对话编辑器中提供节点创建的搜索功能。
/// 实现了 ISearchWindowProvider 接口以支持 Unity GraphView 的搜索窗口机制。
/// </summary>
public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private DialogueEditorWindow editorWindow;
    private DialogueGraphView graphView;

    private Texture2D pic;

    /// <summary>
    /// 配置搜索窗口所需的依赖项。
    /// </summary>
    /// <param name="editorWindow">对话编辑器窗口实例。</param>
    /// <param name="graphView">对话图视图实例。</param>
    public void Configure(DialogueEditorWindow editorWindow, DialogueGraphView graphView)
    {
        this.editorWindow = editorWindow;
        this.graphView = graphView;

        pic = new Texture2D(1, 1);
        pic.SetPixel(0, 0, new Color(0, 0, 0, 0));
        pic.Apply();
    }
    
    /// <summary>
    /// 创建搜索树结构，供搜索窗口显示可用的节点类型。
    /// </summary>
    /// <param name="context">当前搜索窗口上下文信息。</param>
    /// <returns>包含所有可选节点类型的搜索树条目列表。</returns>
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Dialogue Node"), 0),
            new SearchTreeGroupEntry(new GUIContent("Dialogue"), 1),
            
            AddNodeSearch("Start Node", new StartNode()),
            AddNodeSearch("Dialogue Node", new DialogueNode()),
            AddNodeSearch("Event Node", new EventNode()),
            AddNodeSearch("End Node", new EndNode()),
        };
        
        return tree;
    }

    /// <summary>
    /// 创建一个节点搜索条目。
    /// </summary>
    /// <param name="name">节点名称，将作为搜索条目的显示文本。</param>
    /// <param name="baseNode">节点实例，用于存储在 userData 中供后续使用。</param>
    /// <returns>构建好的搜索树条目对象。</returns>
    private SearchTreeEntry AddNodeSearch(string name, BaseNode baseNode)
    {
        SearchTreeEntry tmp = new SearchTreeEntry(new GUIContent(name, pic))
        {
            level = 2,
            userData = baseNode
        };

        return tmp;
    }

    /// <summary>
    /// 当用户从搜索窗口选择一个条目时调用此方法。
    /// 根据所选条目创建对应的节点并添加到图中。
    /// </summary>
    /// <param name="searchTreeEntry">用户选择的搜索树条目。</param>
    /// <param name="context">当前搜索窗口上下文信息。</param>
    /// <returns>是否成功处理了该条目选择事件。</returns>
    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        // 将屏幕坐标转换为图内容容器中的局部坐标
        Vector2 mousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(
            editorWindow.rootVisualElement.parent, context.screenMousePosition - editorWindow.position.position);

        Vector2 graphMousePosition = graphView.contentViewContainer.WorldToLocal(mousePosition);
        
        return CheckForNodeType(searchTreeEntry, graphMousePosition);
    }

    /// <summary>
    /// 检查所选条目的节点类型，并根据类型创建相应的节点添加到图中。
    /// </summary>
    /// <param name="searchTreeEntry">用户选择的搜索树条目。</param>
    /// <param name="pos">节点应被创建的位置坐标。</param>
    /// <returns>是否成功识别并创建了对应类型的节点。</returns>
    private bool CheckForNodeType(SearchTreeEntry searchTreeEntry, Vector2 pos)
    {
        switch (searchTreeEntry.userData)
        {
            case StartNode node:
                graphView.AddElement(graphView.CreateStartNode(pos));
                return true;
            case DialogueNode node:
                graphView.AddElement(graphView.CreateDialogueNode(pos));
                return true;
            case EventNode node:
                graphView.AddElement(graphView.CreateEventNode(pos));
                return true;
            case EndNode node:
                graphView.AddElement(graphView.CreateEndNode(pos));
                return true;
            default:
                break;
        }

        return false;
    }
}
