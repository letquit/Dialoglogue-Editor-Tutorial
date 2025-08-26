using System;
using TMPro;
using UnityEngine;

public class DialogueTalkZone : MonoBehaviour
{
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private KeyCode talkKey = KeyCode.E;
    [SerializeField] private TextMeshProUGUI keyInputText;

    private void Awake()
    {
        speechBubble.SetActive(false);
        keyInputText.text = talkKey.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            speechBubble.SetActive(true);
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(talkKey) && speechBubble.activeSelf)
        {
            // TODO: Start Dialogue
            Debug.Log("Start Dialogue");
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
