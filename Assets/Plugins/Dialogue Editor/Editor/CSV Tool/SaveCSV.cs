using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 用于将对话数据保存为 CSV 文件的类。
/// </summary>
public class SaveCSV
{
    private string csvDirectorName = "Resources/Dialogue/CSV File";
    private string csvFileName = "DialogueCSV_Save.csv";
    private string csvSeparator = ",";
    private List<string> csvHeader;
    private string idName = "Guid ID";

    /// <summary>
    /// 将所有对话容器中的节点和端口数据导出为 CSV 文件。
    /// </summary>
    public void Save()
    {
        List<DialogueContainerSO> dialogueContainers = Helper.FindAllObjectContainerSO();
        // List<DialogueContainerSO> dialogueContainers = Helper.FindAllObjectFromResources<DialogueContainerSO>();
        
        CreateFile();

        foreach (DialogueContainerSO dialogueContainer in dialogueContainers)
        {
            foreach (DialogueNodeData nodeData in dialogueContainer.DialogueNodeDatas)
            {
                List<string> texts = new List<string>();
                
                // 添加节点 GUID
                texts.Add(nodeData.NodeGuid);
                
                // 遍历所有语言类型，添加对应的文本内容
                foreach (LanguageType languageType in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
                {
                    string tmp = nodeData.TextLanguages.Find(language => language.LanguageType == languageType).LanguageGenericType.Replace("\"", "\"\"");
                    texts.Add($"\"{tmp}\"");
                }
                
                AppendToFile(texts);

                // 遍历节点的所有端口并写入文件
                foreach (DialogueNodePort nodePorts in nodeData.DialogueNodePorts)
                {
                    texts = new List<string>();
                    
                    texts.Add(nodePorts.PortGuid);

                    foreach (LanguageType languageType in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
                    {
                        string tmp = nodePorts.TextLanguages.Find(language => language.LanguageType == languageType).LanguageGenericType.Replace("\"", "\"\"");
                        texts.Add($"\"{tmp}\"");
                    }
                    
                    AppendToFile(texts);
                }
            }
        }
    }

    /// <summary>
    /// 向 CSV 文件追加一行数据。
    /// </summary>
    /// <param name="strings">要写入的一行数据列表</param>
    private void AppendToFile(List<string> strings)
    {
        using (StreamWriter sw = File.AppendText(GetFilePath()))
        {
            string finalString = "";
            foreach (string text in strings)
            {
                if (finalString != "")
                {
                    finalString += csvSeparator;
                }
                finalString += text;
            }
            
            sw.WriteLine(finalString);
        }
    }

    /// <summary>
    /// 创建 CSV 文件，并写入表头信息。
    /// </summary>
    private void CreateFile()
    {
        VerifDirectory();
        MakeHeader();
        using (StreamWriter sw = File.CreateText(GetFilePath()))
        {
            string finalString = "";
            foreach (string header in csvHeader)
            {
                if (finalString != "")
                {
                    finalString += csvSeparator;
                }
                finalString += header;
            }
            
            sw.WriteLine(finalString);
        }
    }

    /// <summary>
    /// 构造 CSV 文件的表头，包括 ID 名称和所有语言类型。
    /// </summary>
    private void MakeHeader()
    {
        List<string> headerText = new List<string>();
        headerText.Add(idName);
        
        foreach (LanguageType language in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
        {
            headerText.Add(language.ToString());
        }
        
        csvHeader = headerText;
    }

    /// <summary>
    /// 检查目录是否存在，如果不存在则创建该目录。
    /// </summary>
    private void VerifDirectory()
    {
        string directory = GetDirectoryPath();

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    /// <summary>
    /// 获取 CSV 文件所在目录的完整路径。
    /// </summary>
    /// <returns>目录的完整路径</returns>
    private string GetDirectoryPath()
    {
        return $"{Application.dataPath}/{csvDirectorName}";
    }
    
    /// <summary>
    /// 获取 CSV 文件的完整路径。
    /// </summary>
    /// <returns>文件的完整路径</returns>
    private string GetFilePath()
    {
        return $"{GetDirectoryPath()}/{csvFileName}";
    }
}
