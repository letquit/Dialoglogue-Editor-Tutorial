using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Event", menuName = "Dialogue/Dialogue Event")]
[Serializable]
public class DialogueEventSO : ScriptableObject
{
    public virtual void RunEvent()
    {
        Debug.Log("Event was Call");
    }
}
