using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueEditorWindow : EditorWindow
{
    private DialogueContainerSO currentDialogueContainer;
    private DialogueGraphView graphView;

    private ToolbarMenu toolbarMenu;
    private Label nameOfDialogueContainer;
    
    private LanguageType languageType = LanguageType.English;
    public LanguageType LanguageType { get => languageType; set => languageType = value; }

    [OnOpenAsset(1)]
    public static bool ShowWindow(int _instanceID, int line)
    {
        UnityEngine.Object item = EditorUtility.InstanceIDToObject(_instanceID);
        
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
    
    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
        Load();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }

    private void ConstructGraphView()
    {
        graphView = new DialogueGraphView(this);
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }
    
    private void GenerateToolbar()
    {
        StyleSheet styleSheet = Resources.Load<StyleSheet>("GraphViewStyleSheet");
        rootVisualElement.styleSheets.Add(styleSheet);
        
        Toolbar toolbar = new Toolbar();
        
        // Save button
        Button saveBtn = new Button()
        {
            text = "Save"
        };
        saveBtn.clicked += Save;
        toolbar.Add(saveBtn);
        
        // Load button
        Button loadBtn = new Button()
        {
            text = "Load"
        };
        loadBtn.clicked += Load;
        toolbar.Add(loadBtn);
        
        // Dropdown menu for languages
        toolbarMenu = new ToolbarMenu();
        foreach (LanguageType language in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
        {
            toolbarMenu.menu.AppendAction(language.ToString(), x => Language(language, toolbarMenu));

        }
        toolbar.Add(toolbarMenu);
        
        // Name of current DialogueContainer you have open.
        nameOfDialogueContainer = new Label("");
        toolbar.Add(nameOfDialogueContainer);
        nameOfDialogueContainer.AddToClassList("nameOfDialogueContainer");
        
        rootVisualElement.Add(toolbar);
    }
    
    private void Load()
    {
        // TODO: load it   
        Debug.Log("Load");
        if (currentDialogueContainer != null)
        {
            Language(LanguageType.English, toolbarMenu);
            nameOfDialogueContainer.text = $"Name:   {currentDialogueContainer.name}";
        }
    }
    
    private void Save()
    {
        // TODO: save it
        Debug.Log("Save");
    }

    private void Language(LanguageType _language, ToolbarMenu _toolbarMenu)
    {
        // TODO: language
        toolbarMenu.text = $"Language: {_language}";
        languageType = _language;
        graphView.LanguageReload();
    }
}
