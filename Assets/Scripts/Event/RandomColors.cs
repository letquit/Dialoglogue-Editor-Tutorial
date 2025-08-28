using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomColors : MonoBehaviour
{
    [SerializeField] private int myNumber;
    private List<Material> materials = new List<Material>();

    private void Awake()
    {
        SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        
        foreach (SkinnedMeshRenderer smr in skinnedMeshRenderers)
        {
            foreach (Material mat in smr.materials)
            {
                materials.Add(mat);
            }
        }
    }

    private void Start()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.RandomColorModel += DoRandomColorModel;
        }
        else
        {
            Debug.LogError("RandomColors: GameEvents.Instance is null");
        }
    }

    private void OnDestroy()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.RandomColorModel -= DoRandomColorModel;
        }
    }

    private void DoRandomColorModel(int _number)
    {
        if (myNumber == _number)
        {
            foreach (Material material in materials)
            {
                Color newColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                material.color = newColor;
            }
        }
    }
}
