using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 语言类型更新类，用于更新对话系统中的多语言数据
/// </summary>
public class UpdateLanguageType
{
    /// <summary>
    /// 更新所有对话容器中的语言数据，确保包含所有语言类型
    /// 遍历所有对话容器及其节点数据，为每个节点的文本和音频数据添加缺失的语言类型支持
    /// </summary>
    public void UpdateLanguage()
    {
        // 获取所有对话容器资源
        List<DialogueContainerSO> dialogueContainers = Helper.FindAllObjectFromResources<DialogueContainerSO>();
        
        // 遍历所有对话容器
        foreach (DialogueContainerSO dialogueContainer in dialogueContainers)
        {
            // 遍历容器中的所有对话节点数据
            foreach (DialogueNodeData nodeData in dialogueContainer.DialogueNodeDatas)
            {
                // 更新节点的文本语言数据
                nodeData.TextLanguages = UpdateLanguageGeneric(nodeData.TextLanguages);
                // 更新节点的音频剪辑数据
                nodeData.AudioClips = UpdateLanguageGeneric(nodeData.AudioClips);
                
                // 遍历节点的所有端口数据
                foreach (DialogueNodePort nodePort in nodeData.DialogueNodePorts)
                {
                    // 更新端口的文本语言数据
                    nodePort.TextLanguages = UpdateLanguageGeneric(nodePort.TextLanguages);
                }
            }
        }
    }

    /// <summary>
    /// 通用语言数据更新方法，确保列表包含所有语言类型
    /// 为缺失的语言类型添加占位数据，并保留原有数据
    /// </summary>
    /// <typeparam name="T">语言数据的泛型类型</typeparam>
    /// <param name="languageGenerics">原始语言数据列表</param>
    /// <returns>包含所有语言类型的更新后列表</returns>
    private List<LanguageGeneric<T>> UpdateLanguageGeneric<T>(List<LanguageGeneric<T>> languageGenerics)
    {
        // 创建包含所有语言类型的新列表
        List<LanguageGeneric<T>> tmp = new List<LanguageGeneric<T>>();

        // 为每种语言类型创建占位数据
        foreach (LanguageType languageType in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
        {
            tmp.Add(new LanguageGeneric<T>
            {
                LanguageType = languageType
            });
        }

        // 将原始数据复制到新列表中对应的语言类型位置
        foreach (LanguageGeneric<T> languageGeneric in languageGenerics)
        {
            if (tmp.Find(language => language.LanguageType == languageGeneric.LanguageType) != null)
            {
                tmp.Find(language => language.LanguageType == languageGeneric.LanguageType).LanguageGenericType = languageGeneric.LanguageGenericType;
            }
        }

        return tmp;
    }
}

