using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Color Event", menuName = "Dialogue/Color Event")]
[Serializable]
public class EventRandomColors : DialogueEventSO
{
    [SerializeField] private int number;
    public override void RunEvent()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.CallRandomColorModel(number);
        }
        else
        {
            Debug.LogError("EventRandomColors: GameEvents.Instance is null");
        }
    }
}
