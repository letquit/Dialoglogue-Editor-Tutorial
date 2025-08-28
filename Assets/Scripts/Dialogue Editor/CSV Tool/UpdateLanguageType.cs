using System;
using System.Collections.Generic;
using UnityEngine;

public class UpdateLanguageType
{
    public void UpdateLanguage()
    {
        List<DialogueContainerSO> dialogueContainers = Helper.FindAllObjectFromResources<DialogueContainerSO>();
        
        foreach (DialogueContainerSO dialogueContainer in dialogueContainers)
        {
            foreach (DialogueNodeData nodeData in dialogueContainer.DialogueNodeDatas)
            {
                nodeData.TextLanguages = UpdateLanguageGeneric(nodeData.TextLanguages);
                nodeData.AudioClips = UpdateLanguageGeneric(nodeData.AudioClips);
                foreach (DialogueNodePort nodePort in nodeData.DialogueNodePorts)
                {
                    nodePort.TextLanguages = UpdateLanguageGeneric(nodePort.TextLanguages);
                }
            }
        }
    }

    private List<LanguageGeneric<T>> UpdateLanguageGeneric<T>(List<LanguageGeneric<T>> languageGenerics)
    {
        List<LanguageGeneric<T>> tmp = new List<LanguageGeneric<T>>();

        foreach (LanguageType languageType in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
        {
            tmp.Add(new LanguageGeneric<T>
            {
                LanguageType = languageType
            });
        }

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
