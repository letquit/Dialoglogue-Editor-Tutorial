using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 对话编辑器窗口类，用于在Unity编辑器中可视化地编辑对话内容。
/// 提供图形化界面来管理对话节点、保存和加载对话数据等功能。
/// </summary>
public class DialogueEditorWindow : EditorWindow
{
    private DialogueContainerSO currentDialogueContainer;
    private DialogueGraphView graphView;
    private DialogueSaveAndLoad saveAndLoad;

    private ToolbarMenu toolbarMenu;
    private Label nameOfDialogueContainer;
    
    private LanguageType languageType = LanguageType.English;
    
    /// <summary>
    /// 获取或设置当前的语言类型。
    /// </summary>
    public LanguageType LanguageType { get => languageType; set => languageType = value; }

    /// <summary>
    /// 当资源被双击打开时调用此方法，如果资源是DialogueContainerSO类型，则打开对话编辑器窗口并加载该资源。
    /// </summary>
    /// <param name="instanceID">被打开资源的实例ID。</param>
    /// <param name="line">行号（未使用）。</param>
    /// <returns>是否处理了该资源的打开操作。</returns>
    [OnOpenAsset(1)]
    public static bool ShowWindow(int instanceID, int line)
    {
        UnityEngine.Object item = EditorUtility.InstanceIDToObject(instanceID);
        
        if (item is DialogueContainerSO)
        {
            DialogueEditorWindow window = GetWindow<DialogueEditorWindow>();
            window.titleContent = new GUIContent("Dialogue Editor");
            window.currentDialogueContainer = item as DialogueContainerSO;
            window.minSize = new Vector2(500, 250);
            window.Load();
        }

        return false;
    }
    
    /// <summary>
    /// 当窗口启用时调用，初始化图形视图和工具栏，并尝试加载当前对话容器。
    /// </summary>
    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
        Load();
    }

    /// <summary>
    /// 当窗口禁用时调用，移除图形视图以释放资源。
    /// </summary>
    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }

    /// <summary>
    /// 构造对话图形视图，并初始化保存与加载功能模块。
    /// </summary>
    private void ConstructGraphView()
    {
        graphView = new DialogueGraphView(this);
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
        
        saveAndLoad = new DialogueSaveAndLoad(graphView);
    }
    
    /// <summary>
    /// 生成顶部工具栏，包括保存、加载按钮、语言选择菜单以及当前对话容器名称显示。
    /// </summary>
    private void GenerateToolbar()
    {
        StyleSheet styleSheet = Resources.Load<StyleSheet>("GraphViewStyleSheet");
        rootVisualElement.styleSheets.Add(styleSheet);
        
        Toolbar toolbar = new Toolbar();
        
        // 创建保存按钮
        Button saveBtn = new Button()
        {
            text = "Save"
        };
        saveBtn.clicked += Save;
        toolbar.Add(saveBtn);
        
        // 创建加载按钮
        Button loadBtn = new Button()
        {
            text = "Load"
        };
        loadBtn.clicked += Load;
        toolbar.Add(loadBtn);
        
        // 创建语言选择下拉菜单
        toolbarMenu = new ToolbarMenu();
        foreach (LanguageType language in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
        {
            toolbarMenu.menu.AppendAction(language.ToString(), x => Language(language, toolbarMenu));

        }
        toolbar.Add(toolbarMenu);
        
        // 显示当前对话容器名称
        nameOfDialogueContainer = new Label("");
        toolbar.Add(nameOfDialogueContainer);
        nameOfDialogueContainer.AddToClassList("nameOfDialogueContainer");
        
        rootVisualElement.Add(toolbar);
    }
    
    /// <summary>
    /// 加载当前对话容器的内容到图形视图中。
    /// </summary>
    private void Load()
    {
        if (currentDialogueContainer != null)
        {
            Language(LanguageType.English, toolbarMenu);
            nameOfDialogueContainer.text = $"Name:   {currentDialogueContainer.name}";
            saveAndLoad.Load(currentDialogueContainer);
        }
    }
    
    /// <summary>
    /// 将当前图形视图中的内容保存到对话容器中。
    /// </summary>
    private void Save()
    {
        if (currentDialogueContainer != null)
        {
            saveAndLoad.Save(currentDialogueContainer);
        }
    }

    /// <summary>
    /// 切换当前显示的语言，并更新图形视图的语言显示。
    /// </summary>
    /// <param name="language">要切换到的语言类型。</param>
    /// <param name="toolbarMenu">语言选择菜单控件。</param>
    private void Language(LanguageType language, ToolbarMenu toolbarMenu)
    {
        this.toolbarMenu.text = $"Language: {language}";
        languageType = language;
        graphView.ReloadLanguage();
    }
}
