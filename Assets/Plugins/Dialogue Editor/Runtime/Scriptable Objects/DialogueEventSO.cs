using System;
using UnityEngine;

[Serializable]
public class DialogueEventSO : ScriptableObject
{
    public virtual void RunEvent()
    {
        Debug.Log("Event was Call");
    }
}
