using System;
using TMPro;
using UnityEngine;

public class DialogueTalkZone : MonoBehaviour
{
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private KeyCode talkKey = KeyCode.E;
    [SerializeField] private TextMeshProUGUI keyInputText;

    private DialogueTalk dialogueTalk;
    
    private void Awake()
    {
        speechBubble.SetActive(false);
        keyInputText.text = talkKey.ToString();
        dialogueTalk = GetComponent<DialogueTalk>();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(talkKey) && speechBubble.activeSelf)
        {
            dialogueTalk.StartDialogue();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            speechBubble.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            speechBubble.SetActive(false);
        }
    }
}
