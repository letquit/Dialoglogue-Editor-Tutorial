// DialogueController.cs
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 对话控制器，用于控制对话UI的显示、文本内容、角色图像以及选项按钮。
/// </summary>
public class DialogueController : MonoBehaviour
{
    [SerializeField] private GameObject dialogueUI;
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI textName;
    [SerializeField] private TextMeshProUGUI textBox;
    [Header("Image")]
    [SerializeField] private Image leftImage;
    [SerializeField] private GameObject leftImageGO;
    [SerializeField] private Image rightImage;
    [SerializeField] private GameObject rightImageGO;
    [Header("Buttons")]
    [SerializeField] private Button button01;
    [SerializeField] private TextMeshProUGUI buttonText01;
    [Space]
    [SerializeField] private Button button02;
    [SerializeField] private TextMeshProUGUI buttonText02;
    [Space]
    [SerializeField] private Button button03;
    [SerializeField] private TextMeshProUGUI buttonText03;
    [Space]
    [SerializeField] private Button button04;
    [SerializeField] private TextMeshProUGUI buttonText04;

    private List<Button> buttons = new List<Button>();
    private List<TextMeshProUGUI> buttonTexts = new List<TextMeshProUGUI>();
    
    /// <summary>
    /// 初始化对话控制器，在Awake阶段隐藏对话UI并初始化按钮和文本列表。
    /// </summary>
    private void Awake()
    {
        ShowDialogueUI(false);
        
        buttons.Add(button01);
        buttons.Add(button02);
        buttons.Add(button03);
        buttons.Add(button04);
        
        buttonTexts.Add(buttonText01);
        buttonTexts.Add(buttonText02);
        buttonTexts.Add(buttonText03);
        buttonTexts.Add(buttonText04);
    }

    /// <summary>
    /// 显示或隐藏整个对话UI。
    /// </summary>
    /// <param name="show">是否显示对话UI</param>
    public void ShowDialogueUI(bool show)
    {
        dialogueUI.SetActive(show);
    }

    /// <summary>
    /// 设置对话中的角色名称和对话内容。
    /// </summary>
    /// <param name="name">角色名称</param>
    /// <param name="textBox">对话内容</param>
    public void SetText(string name, string textBox)
    {
        textName.text = name;
        this.textBox.text = textBox;
        
        // 切换字体以适配当前语言
        UpdateFont();
    }

    /// <summary>
    /// 设置对话中角色的图像，并根据类型决定显示在左侧还是右侧。
    /// </summary>
    /// <param name="image">要显示的角色图像</param>
    /// <param name="dialogueFaceImageType">图像显示位置（左或右）</param>
    public void SetImage(Sprite image, DialogueFaceImageType dialogueFaceImageType)
    {
        leftImageGO.SetActive(false);
        rightImageGO.SetActive(false);

        if (image != null)
        {
            if (dialogueFaceImageType == DialogueFaceImageType.Left)
            {
                leftImage.sprite = image;
                leftImageGO.SetActive(true);
            }
            else
            {
                rightImage.sprite = image;
                rightImageGO.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 设置对话选项按钮的文本和点击事件。
    /// </summary>
    /// <param name="texts">按钮上显示的文本列表</param>
    /// <param name="unityActions">每个按钮对应的点击回调函数列表</param>
    public void SetButtons(List<string> texts, List<UnityAction> unityActions)
    {
        if (texts == null || unityActions == null)
        {
            Debug.LogError("DialogueController: Texts or actions list is null");
            return;
        }
        
        // 先隐藏所有按钮
        buttons.ForEach(button => button.gameObject.SetActive(false));

        // 为每个按钮设置文本和点击事件
        for (int i = 0; i < texts.Count && i < buttons.Count; i++)
        {
            // 设置按钮文本
            buttonTexts[i].text = texts[i];
            
            // 显示按钮
            buttons[i].gameObject.SetActive(true);
            
            // 清除之前的监听器并添加新的监听器
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(unityActions[i]);
        }
        
        // 切换按钮字体以适配当前语言
        UpdateButtonFonts();
    }
    
    /// <summary>
    /// 根据当前语言更新对话文本使用的字体。
    /// </summary>
    private void UpdateFont()
    {
        if (LanguageController.Instane != null)
        {
            TMP_FontAsset font = LanguageController.Instane.GetFontForLanguage(LanguageController.Instane.Language);
            if (font != null)
            {
                textName.font = font;
                textBox.font = font;
            }
        }
    }
    
    /// <summary>
    /// 根据当前语言更新按钮文本使用的字体。
    /// </summary>
    private void UpdateButtonFonts()
    {
        if (LanguageController.Instane != null)
        {
            TMP_FontAsset font = LanguageController.Instane.GetFontForLanguage(LanguageController.Instane.Language);
            if (font != null)
            {
                foreach (var buttonText in buttonTexts)
                {
                    buttonText.font = font;
                }
            }
        }
    }
}
