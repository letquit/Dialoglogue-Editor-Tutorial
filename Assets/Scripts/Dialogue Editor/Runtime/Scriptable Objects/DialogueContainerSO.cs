using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "New Dialogue Container", menuName = "Dialogue/Dialogue Container")]
[Serializable]
public class DialogueContainerSO : ScriptableObject
{
    
}

[Serializable]
public class LanguageGeneric<T>
{
    public LanguageType LanguageType;
    public T LanguageGenericType;
}

[Serializable]
public class DialogueNodePort
{
    public string InputGuid;
    public string OutputGuid;
    public string PortId;
    public string TextFieldId;
    public List<LanguageGeneric<string>> TextLanguages = new List<LanguageGeneric<string>>();
}
