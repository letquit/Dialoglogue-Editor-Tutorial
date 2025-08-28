using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTalk : DialogueGetData
{
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private AudioSource audioSource;
    
    private DialogueNodeData currentDialogueNodeData;
    private DialogueNodeData lastDialogueNodeData;

    private void Awake()
    {
        dialogueController = FindAnyObjectByType<DialogueController>();
        audioSource = GetComponent<AudioSource>();
    }

    public void StartDialogue()
    {
        if (dialogueContainer == null)
        {
            Debug.LogError("DialogueTalk: dialogueContainer is null");
            return;
        }
        
        if (dialogueContainer.StartNodeDatas == null || dialogueContainer.StartNodeDatas.Count == 0)
        {
            Debug.LogError("DialogueTalk: No start node data found");
            return;
        }
        
        BaseNodeData nextNode = GetNextNode(dialogueContainer.StartNodeDatas[0]);
        CheckNodeType(nextNode);
        dialogueController.ShowDialogueUI(true);
    }

    private void CheckNodeType(BaseNodeData _baseNodeData)
    {
        if (_baseNodeData == null)
        {
            Debug.LogError("DialogueTalk: BaseNodeData is null");
            return;
        }
        
        switch (_baseNodeData)
        {
            case StartNodeData nodeData:
                RunNode(nodeData);
                break;
            case DialogueNodeData nodeData:
                RunNode(nodeData);
                break;
            case EventNodeData nodeData:
                RunNode(nodeData);
                break;
            case EndNodeData nodeData:
                RunNode(nodeData);
                break;
            default:
                Debug.LogError($"DialogueTalk: Unknown node type: {_baseNodeData.GetType()}");
                break;
        }
    }

    private void RunNode(StartNodeData _nodeData)
    {
        if (dialogueContainer.StartNodeDatas.Count > 0)
        {
            BaseNodeData nextNode = GetNextNode(dialogueContainer.StartNodeDatas[0]);
            CheckNodeType(nextNode);
        }
    }
    
    private void RunNode(DialogueNodeData _nodeData)
    {
        if (currentDialogueNodeData != _nodeData)
        {
            lastDialogueNodeData = currentDialogueNodeData;
            currentDialogueNodeData = _nodeData;
        }
        
        // 设置对话文本
        var textEntry = _nodeData.TextType.Find(text => text.LanguageType == LanguageController.Instane.Language);
        string displayText = textEntry != null ? textEntry.LanguageGenericType : "No text found";
        dialogueController.SetText(_nodeData.Name, displayText);
        
        // 设置图像
        dialogueController.SetImage(_nodeData.Sprite, _nodeData.DialogueFaceImageType);
        
        // 创建按钮
        MakeButtons(_nodeData.DialogueNodePorts);
        
        // 播放音频
        var audioEntry = _nodeData.AudioClips.Find(clip => clip.LanguageType == LanguageController.Instane.Language);
        AudioClip clip = audioEntry != null ? audioEntry.LanguageGenericType : null;
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
    
    private void RunNode(EventNodeData _nodeData)
    {
        if (_nodeData.DialogueEventSO != null)
        {
            _nodeData.DialogueEventSO.RunEvent();
        }
        
        BaseNodeData nextNode = GetNextNode(_nodeData);
        CheckNodeType(nextNode);
    }
    
    private void RunNode(EndNodeData _nodeData)
    {
        switch (_nodeData.EndNodeType)
        {
            case EndNodeType.End:
                dialogueController.ShowDialogueUI(false);
                break;
            case EndNodeType.Repeat:
                CheckNodeType(GetNodeByGuid(currentDialogueNodeData.NodeGuid));
                break;
            case EndNodeType.Goback:
                CheckNodeType(GetNodeByGuid(lastDialogueNodeData.NodeGuid));
                break;
            case EndNodeType.ReturnToStart:
                CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[0]));
                break;
            default:
                break;
        }
    }

    private void MakeButtons(List<DialogueNodePort> _nodePorts)
    {
        if (_nodePorts == null)
        {
            Debug.LogError("DialogueTalk: NodePorts is null");
            return;
        }
        
        List<string> texts = new List<string>();
        List<UnityAction> unityActions = new List<UnityAction>();

        foreach (DialogueNodePort nodePort in _nodePorts)
        {
            // 获取按钮文本
            var textEntry = nodePort.TextLanguages.Find(text => text.LanguageType == LanguageController.Instane.Language);
            string buttonText = textEntry != null ? textEntry.LanguageGenericType : "Unknown";
            texts.Add(buttonText);
            
            // 创建按钮动作
            string inputGuid = nodePort.InputGuid; // 创建局部副本避免闭包问题
            UnityAction tempAction = () =>
            {
                audioSource.Stop();
                BaseNodeData targetNode = GetNodeByGuid(inputGuid);
                if (targetNode != null)
                {
                    CheckNodeType(targetNode);
                }
                else
                {
                    Debug.LogError($"DialogueTalk: Target node with GUID {inputGuid} not found");
                }
            };
            unityActions.Add(tempAction);
        }
        
        dialogueController.SetButtons(texts, unityActions);
    }
}
