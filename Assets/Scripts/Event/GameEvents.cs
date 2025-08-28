using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    private event Action<int> randomColorModel;

    public static GameEvents Instance { get; private set; }
    public Action<int> RandomColorModel { get => randomColorModel; set => randomColorModel = value; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void CallRandomColorModel(int number)
    {
        randomColorModel?.Invoke(number);
    }
}
