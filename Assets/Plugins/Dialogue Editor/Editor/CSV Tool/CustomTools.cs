using UnityEditor;
using UnityEngine;

/// <summary>
/// 自定义工具类，提供对话系统相关的CSV文件操作和语言更新功能
/// </summary>
public class CustomTools
{
    /// <summary>
    /// 将对话数据保存到CSV文件的菜单命令
    /// 通过Unity编辑器菜单"Custom Tools/Dialogue/Save to CSV"触发
    /// </summary>
    [MenuItem("Custom Tools/Dialogue/Save to CSV")]
    public static void SaveToCSV()
    {
        // 创建CSV保存实例并执行保存操作
        SaveCSV saveCSV = new SaveCSV();
        saveCSV.Save();
        
        // 操作完成提示
        EditorApplication.Beep();
        Debug.Log("<color=green> Save CSV File successfully! </color>");
    }
    
    /// <summary>
    /// 从CSV文件加载对话数据的菜单命令
    /// 通过Unity编辑器菜单"Custom Tools/Dialogue/Load from CSV"触发
    /// </summary>
    [MenuItem("Custom Tools/Dialogue/Load from CSV")]
    public static void LoadFromCSV()
    { 
        // 创建CSV加载实例并执行加载操作
        LoadCSV loadCSV = new LoadCSV();
        loadCSV.Load();
        
        // 操作完成提示
        EditorApplication.Beep();
        Debug.Log("<color=green> Loading CSV File successfully! </color>");
    }
    
    /// <summary>
    /// 更新对话语言类型的菜单命令
    /// 通过Unity编辑器菜单"Custom Tools/Dialogue/Update Dialogue Languages"触发
    /// </summary>
    [MenuItem("Custom Tools/Dialogue/Update Dialogue Languages")]
    public static void UpdateDialogueLanguage()
    {
        // 创建语言更新实例并执行更新操作
        UpdateLanguageType updateLanguageType = new UpdateLanguageType();
        updateLanguageType.UpdateLanguage();
        
        // 操作完成提示
        EditorApplication.Beep();
        Debug.Log("<color=green> Update languages successfully! </color>");
    }
}

