using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 对话系统的核心执行类，用于控制对话流程的启动、节点执行和按钮生成。
/// 继承自 DialogueGetData，提供对对话数据的访问和处理能力。
/// </summary>
public class DialogueTalk : DialogueGetData
{
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private AudioSource audioSource;
    
    private DialogueNodeData currentDialogueNodeData;
    private DialogueNodeData lastDialogueNodeData;

    /// <summary>
    /// 在Awake阶段初始化对话控制器和音频源组件。
    /// </summary>
    private void Awake()
    {
        dialogueController = FindAnyObjectByType<DialogueController>();
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 启动对话流程。从对话容器中获取起始节点并开始执行。
    /// 如果对话容器或起始节点为空，则输出错误日志并返回。
    /// </summary>
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

    /// <summary>
    /// 根据节点类型分发到对应的执行方法。
    /// </summary>
    /// <param name="baseNodeData">要检查的节点数据</param>
    private void CheckNodeType(BaseNodeData baseNodeData)
    {
        if (baseNodeData == null)
        {
            Debug.LogError("DialogueTalk: BaseNodeData is null");
            return;
        }
        
        switch (baseNodeData)
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
                Debug.LogError($"DialogueTalk: Unknown node type: {baseNodeData.GetType()}");
                break;
        }
    }

    /// <summary>
    /// 执行起始节点逻辑，获取下一个节点并继续执行。
    /// </summary>
    /// <param name="nodeData">起始节点数据</param>
    private void RunNode(StartNodeData nodeData)
    {
        if (dialogueContainer.StartNodeDatas.Count > 0)
        {
            BaseNodeData nextNode = GetNextNode(dialogueContainer.StartNodeDatas[0]);
            CheckNodeType(nextNode);
        }
    }
    
    /// <summary>
    /// 执行对话节点逻辑，包括设置文本、图像、按钮和播放音频。
    /// </summary>
    /// <param name="nodeData">对话节点数据</param>
    private void RunNode(DialogueNodeData nodeData)
    {
        if (currentDialogueNodeData != nodeData)
        {
            lastDialogueNodeData = currentDialogueNodeData;
            currentDialogueNodeData = nodeData;
        }
        
        // 设置对话文本
        var textEntry = nodeData.TextLanguages.Find(text => text.LanguageType == LanguageController.Instane.Language);
        string displayText = textEntry != null ? textEntry.LanguageGenericType : "No text found";
        dialogueController.SetText(nodeData.Name, displayText);
        
        // 设置图像
        dialogueController.SetImage(nodeData.Sprite, nodeData.DialogueFaceImageType);
        
        // 创建按钮
        MakeButtons(nodeData.DialogueNodePorts);
        
        // 播放音频
        var audioEntry = nodeData.AudioClips.Find(clip => clip.LanguageType == LanguageController.Instane.Language);
        AudioClip clip = audioEntry != null ? audioEntry.LanguageGenericType : null;
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
    
    /// <summary>
    /// 执行事件节点逻辑，触发事件并继续执行下一个节点。
    /// </summary>
    /// <param name="nodeData">事件节点数据</param>
    private void RunNode(EventNodeData nodeData)
    {
        if (nodeData.DialogueEventSO != null)
        {
            nodeData.DialogueEventSO.RunEvent();
        }
        
        BaseNodeData nextNode = GetNextNode(nodeData);
        CheckNodeType(nextNode);
    }
    
    /// <summary>
    /// 执行结束节点逻辑，根据结束类型决定是否关闭对话界面或跳转到其他节点。
    /// </summary>
    /// <param name="nodeData">结束节点数据</param>
    private void RunNode(EndNodeData nodeData)
    {
        switch (nodeData.EndNodeType)
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

    /// <summary>
    /// 根据节点端口生成对话按钮，每个按钮对应一个跳转动作。
    /// </summary>
    /// <param name="nodePorts">节点端口列表</param>
    private void MakeButtons(List<DialogueNodePort> nodePorts)
    {
        if (nodePorts == null)
        {
            Debug.LogError("DialogueTalk: NodePorts is null");
            return;
        }
        
        List<string> texts = new List<string>();
        List<UnityAction> unityActions = new List<UnityAction>();

        foreach (DialogueNodePort nodePort in nodePorts)
        {
            // 获取按钮文本
            var textEntry = nodePort.TextLanguages.Find(text => text.LanguageType == LanguageController.Instane.Language);
            string buttonText = textEntry != null ? textEntry.LanguageGenericType : "Unknown";
            texts.Add(buttonText);
            
            // 创建按钮动作
            string outputGuid = nodePort.OutputGuid;
            UnityAction tempAction = () =>
            {
                audioSource.Stop();
                BaseNodeData targetNode = GetNodeByGuid(outputGuid);
                if (targetNode != null)
                {
                    CheckNodeType(targetNode);
                }
                else
                {
                    Debug.LogError($"DialogueTalk: Target node with GUID {outputGuid} not found");
                }
            };
            unityActions.Add(tempAction);
        }
        
        dialogueController.SetButtons(texts, unityActions);
    }

}
