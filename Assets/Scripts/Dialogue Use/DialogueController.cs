// DialogueController.cs
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    public void ShowDialogueUI(bool _show)
    {
        dialogueUI.SetActive(_show);
    }

    public void SetText(string _name, string _textBox)
    {
        textName.text = _name;
        textBox.text = _textBox;
        
        // 切换字体
        UpdateFont();
    }

    public void SetImage(Sprite _image, DialogueFaceImageType _dialogueFaceImageType)
    {
        leftImageGO.SetActive(false);
        rightImageGO.SetActive(false);

        if (_image != null)
        {
            if (_dialogueFaceImageType == DialogueFaceImageType.Left)
            {
                leftImage.sprite = _image;
                leftImageGO.SetActive(true);
            }
            else
            {
                rightImage.sprite = _image;
                rightImageGO.SetActive(true);
            }
        }
    }

    public void SetButtons(List<string> _texts, List<UnityAction> _unityActions)
    {
        if (_texts == null || _unityActions == null)
        {
            Debug.LogError("DialogueController: Texts or actions list is null");
            return;
        }
        
        // 先隐藏所有按钮
        buttons.ForEach(button => button.gameObject.SetActive(false));

        // 为每个按钮设置文本和点击事件
        for (int i = 0; i < _texts.Count && i < buttons.Count; i++)
        {
            // 设置按钮文本
            buttonTexts[i].text = _texts[i];
            
            // 显示按钮
            buttons[i].gameObject.SetActive(true);
            
            // 清除之前的监听器并添加新的监听器
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(_unityActions[i]);
        }
        
        // 切换按钮字体
        UpdateButtonFonts();
    }
    
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
