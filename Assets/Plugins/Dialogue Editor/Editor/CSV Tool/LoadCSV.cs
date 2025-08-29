using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 负责从CSV文件中加载对话数据并填充到ScriptableObject中的类。
/// </summary>
public class LoadCSV
{
    private string csvDirectorName = "Resources/Dialogue/CSV File";
    private string csvFileName = "DialogueCSV_Load.csv";
    
    /// <summary>
    /// 加载CSV文件内容，并将数据填充到对应的对话节点和端口中。
    /// </summary>
    public void Load()
    {
        // 读取CSV文件内容
        string text = File.ReadAllText($"{Application.dataPath}/{csvDirectorName}/{csvFileName}");
        List<List<string>> result = ParseCSV(text);
        
        List<string> headers = result[0];
        
        // 获取所有对话容器对象
        List<DialogueContainerSO> dialogueContainers = Helper.FindAllObjectContainerSO();
        // List<DialogueContainerSO> dialogueContainers = Helper.FindAllObjectFromResources<DialogueContainerSO>();

        // 遍历所有对话容器，填充数据
        foreach (DialogueContainerSO dialogueContainer in dialogueContainers)
        {
            foreach (DialogueNodeData nodeData in dialogueContainer.DialogueNodeDatas)
            {
                LoadInToNode(result, headers, nodeData);
                foreach (DialogueNodePort nodePort in nodeData.DialogueNodePorts)
                {
                    LoadInToNodePort(result, headers, nodePort);
                }
            }
            EditorUtility.SetDirty(dialogueContainer);
            AssetDatabase.SaveAssets();
        }
    }

    /// <summary>
    /// 将CSV数据加载到指定的对话节点中。
    /// </summary>
    /// <param name="result">解析后的CSV数据</param>
    /// <param name="headers">CSV文件的表头</param>
    /// <param name="nodeData">要填充的对话节点数据</param>
    private void LoadInToNode(List<List<string>> result, List<string> headers, DialogueNodeData nodeData)
    {
        foreach (List<string> line in result)
        {
            if (line[0] == nodeData.NodeGuid)
            {
                for (int i = 0; i < line.Count; i++)
                {
                    foreach (LanguageType languageType in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
                    {
                        if (headers[i] == languageType.ToString())
                        {
                            nodeData.TextLanguages.Find(x => x.LanguageType == languageType).LanguageGenericType = line[i];
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 将CSV数据加载到指定的对话节点端口中。
    /// </summary>
    /// <param name="result">解析后的CSV数据</param>
    /// <param name="headers">CSV文件的表头</param>
    /// <param name="nodePort">要填充的对话节点端口</param>
    private void LoadInToNodePort(List<List<string>> result, List<string> headers, DialogueNodePort nodePort)
    {
        foreach (List<string> line in result)
        {
            if (line[0] == nodePort.PortGuid)
            {
                for (int i = 0; i < line.Count; i++)
                {
                    foreach (LanguageType languageType in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
                    {
                        if (headers[i] == languageType.ToString())
                        {
                            nodePort.TextLanguages.Find(x => x.LanguageType == languageType).LanguageGenericType = line[i];
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// CSV解析状态枚举，用于标识当前解析器处于引号内还是引号外。
    /// </summary>
    private enum ParsingMode
    {
        /// <summary>
        /// 默认状态（未使用）
        /// </summary>
        None,
        /// <summary>
        /// 处理引号外的字符
        /// </summary>
        OutQuote,
        /// <summary>
        /// 处理引号内的字符
        /// </summary>
        InQuote
    }

    /// <summary>
    /// 解析CSV字符串内容为二维列表。
    /// </summary>
    /// <returns>解析后的二维字符串列表，第一维表示行，第二维表示列</returns>
    /// <param name="src">原始CSV文件内容字符串</param>
    public List<List<string>> ParseCSV(string src)
    {
        var rows = new List<List<string>>();
        var cols = new List<string>();
#pragma warning disable XS0001 // Find APIs marked as TODO in Mono
        var buffer = new StringBuilder();
#pragma warning restore XS0001 // Find APIs marked as TODO in Mono

        ParsingMode mode = ParsingMode.OutQuote;
        bool requireTrimLineHead = false;
        var isBlank = new Regex(@"\s");

        int len = src.Length;

        for (int i = 0; i < len; ++i)
        {

            char c = src[i];

            // 移除行首空白字符
            if (requireTrimLineHead)
            {
                if (isBlank.IsMatch(c.ToString()))
                {
                    continue;
                }
                requireTrimLineHead = false;
            }

            // 处理最后一个字符
            if ((i + 1) == len)
            {
                switch (mode)
                {
                    case ParsingMode.InQuote:
                        if (c == '"')
                        {
                            // 忽略结束引号
                        }
                        else
                        {
                            // 如果缺少结束引号，则添加字符
                            buffer.Append(c);
                        }
                        cols.Add(buffer.ToString());
                        rows.Add(cols);
                        return rows;

                    case ParsingMode.OutQuote:
                        if (c == ',')
                        {
                            // 如果最后一个字符是逗号，则添加一个空单元格
                            cols.Add(buffer.ToString());
                            cols.Add(string.Empty);
                            rows.Add(cols);
                            return rows;
                        }
                        if (cols.Count == 0)
                        {
                            // 如果最后一行为空，则忽略
                            if (string.Empty.Equals(c.ToString().Trim()))
                            {
                                return rows;
                            }
                        }
                        buffer.Append(c);
                        cols.Add(buffer.ToString());
                        rows.Add(cols);
                        return rows;
                }
            }

            // 下一个字符
            char n = src[i + 1];

            switch (mode)
            {
                case ParsingMode.OutQuote:
                    // 引号外处理
                    if (c == '"')
                    {
                        // 进入引号内模式
                        mode = ParsingMode.InQuote;
                        continue;

                    }
                    else if (c == ',')
                    {
                        // 添加当前单元格并准备下一个
                        cols.Add(buffer.ToString());
                        buffer.Remove(0, buffer.Length);

                    }
                    else if (c == '\r' && n == '\n')
                    {
                        // 处理换行符(CR+LF)
                        cols.Add(buffer.ToString());
                        rows.Add(cols);
                        cols = new List<string>();
                        buffer.Remove(0, buffer.Length);
                        ++i; // 跳过下一个字符
                        requireTrimLineHead = true;

                    }
                    else if (c == '\n' || c == '\r')
                    {
                        // 处理换行符
                        cols.Add(buffer.ToString());
                        rows.Add(cols);
                        cols = new List<string>();
                        buffer.Remove(0, buffer.Length);
                        requireTrimLineHead = true;

                    }
                    else
                    {
                        // 添加普通字符
                        buffer.Append(c);
                    }
                    break;

                case ParsingMode.InQuote:
                    // 引号内处理
                    if (c == '"' && n != '"')
                    {
                        // 退出引号内模式
                        mode = ParsingMode.OutQuote;

                    }
                    else if (c == '"' && n == '"')
                    {
                        // 处理转义引号
                        buffer.Append('"');
                        ++i;

                    }
                    else
                    {
                        // 添加引号内字符
                        buffer.Append(c);
                    }
                    break;
            }
        }
        return rows;
    }
}
