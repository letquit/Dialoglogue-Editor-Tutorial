using System;
using TMPro;
using UnityEngine;

/// <summary>
/// 对话触发区域类，用于控制玩家进入特定区域时显示对话气泡和触发对话功能
/// </summary>
public class DialogueTalkZone : MonoBehaviour
{
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private KeyCode talkKey = KeyCode.E;
    [SerializeField] private TextMeshProUGUI keyInputText;

    private DialogueTalk dialogueTalk;
    
    /// <summary>
    /// 初始化组件引用和初始状态设置
    /// 在对象唤醒时执行一次初始化操作
    /// </summary>
    private void Awake()
    {
        // 初始化时隐藏对话气泡
        speechBubble.SetActive(false);
        // 设置按键提示文本
        keyInputText.text = talkKey.ToString();
        // 获取对话组件引用
        dialogueTalk = GetComponent<DialogueTalk>();
    }
    
    /// <summary>
    /// 每帧检测输入按键，当玩家按下对话键且对话气泡处于激活状态时开始对话
    /// </summary>
    private void Update()
    {
        // 检测玩家是否按下对话键并且对话气泡已激活
        if (Input.GetKeyDown(talkKey) && speechBubble.activeSelf)
        {
            dialogueTalk.StartDialogue();
        }
    }

    /// <summary>
    /// 当碰撞体进入触发器时调用，用于检测玩家进入对话区域
    /// </summary>
    /// <param name="other">进入触发器的碰撞体对象</param>
    private void OnTriggerEnter(Collider other)
    {
        // 判断进入触发器的对象是否为玩家标签
        if (other.CompareTag("Player"))
        {
            speechBubble.SetActive(true);
        }
    }

    /// <summary>
    /// 当碰撞体离开触发器时调用，用于检测玩家离开对话区域
    /// </summary>
    /// <param name="other">离开触发器的碰撞体对象</param>
    private void OnTriggerExit(Collider other)
    {
        // 判断离开触发器的对象是否为玩家标签
        if (other.CompareTag("Player"))
        {
            speechBubble.SetActive(false);
        }
    }
}

