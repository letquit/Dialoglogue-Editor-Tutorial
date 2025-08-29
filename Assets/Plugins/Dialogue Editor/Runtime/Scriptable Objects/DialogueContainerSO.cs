using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "New Dialogue Container", menuName = "Dialogue/Dialogue Container")]
[Serializable]
public class DialogueContainerSO : ScriptableObject
{
    public List<NodeLinkData> NodeLinkDatas = new List<NodeLinkData>();
    
    public List<DialogueNodeData> DialogueNodeDatas = new List<DialogueNodeData>();
    public List<EndNodeData> EndNodeDatas = new List<EndNodeData>();
    public List<StartNodeData> StartNodeDatas = new List<StartNodeData>();
    public List<EventNodeData> EventNodeDatas = new List<EventNodeData>();

    public List<BaseNodeData> AllNodes
    {
        get
        {
            List<BaseNodeData> tmp = new List<BaseNodeData>();
            tmp.AddRange(DialogueNodeDatas);
            tmp.AddRange(EndNodeDatas);
            tmp.AddRange(StartNodeDatas);
            tmp.AddRange(EventNodeDatas);

            return tmp;
        }
    }
}

[Serializable]
public class NodeLinkData
{
    public string BaseNodeGuid;
    public string TargetNodeGuid;
}

[Serializable]
public class BaseNodeData
{
    public string NodeGuid;
    public Vector2 Position;
}

[Serializable]
public class DialogueNodeData : BaseNodeData
{
    public List<DialogueNodePort> DialogueNodePorts;
    public Sprite Sprite;
    public DialogueFaceImageType DialogueFaceImageType;
    public List<LanguageGeneric<AudioClip>> AudioClips;
    public string Name;
    [FormerlySerializedAs("TextType")] public List<LanguageGeneric<string>> TextLanguages;
}

[Serializable]
public class EndNodeData : BaseNodeData
{
    public EndNodeType EndNodeType;
}

[Serializable]
public class StartNodeData : BaseNodeData
{
    
}

[Serializable]
public class EventNodeData : BaseNodeData
{
    public DialogueEventSO DialogueEventSO;
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
    public string PortGuid;
    public string InputGuid;
    public string OutputGuid;
    public string PortId;
    public string TextFieldId;
    public List<LanguageGeneric<string>> TextLanguages = new List<LanguageGeneric<string>>();
}
