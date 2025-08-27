using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    private string styleSheetsName = "GraphViewStyleSheet";
    private DialogueEditorWindow editorWindow;
    
    public DialogueGraphView(DialogueEditorWindow _editorWindow)
    {
        editorWindow = _editorWindow;

        StyleSheet tmpStyleSheet = Resources.Load<StyleSheet>(styleSheetsName);
        styleSheets.Add(tmpStyleSheet);
        
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new FreehandSelector());
        
        GridBackground grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();
    }

    public void LanguageReload()
    {
        List<DialogueNode> dialogueNodes = nodes.ToList().Where(node => node is DialogueNode).Cast<DialogueNode>().ToList();
        foreach (DialogueNode dialogueNode in dialogueNodes)
        {
            dialogueNode.ReloadLanguage();
        }
    }
}
