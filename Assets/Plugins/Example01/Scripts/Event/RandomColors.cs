using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 随机颜色控制器类，用于控制模型材质的随机颜色变化
/// </summary>
public class RandomColors : MonoBehaviour
{
    [SerializeField] private int myNumber;
    private List<Material> materials = new List<Material>();

    /// <summary>
    /// 在对象唤醒时执行，获取所有子对象的SkinnedMeshRenderer组件的材质
    /// </summary>
    private void Awake()
    {
        // 获取所有子对象的SkinnedMeshRenderer组件
        SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        
        // 遍历所有SkinnedMeshRenderer组件，收集其材质
        foreach (SkinnedMeshRenderer smr in skinnedMeshRenderers)
        {
            foreach (Material mat in smr.materials)
            {
                materials.Add(mat);
            }
        }
    }

    /// <summary>
    /// 在Start阶段注册事件监听器
    /// </summary>
    private void Start()
    {
        // 检查GameEvents实例是否存在并注册事件
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.RandomColorModel += DoRandomColorModel;
        }
        else
        {
            Debug.LogError("RandomColors: GameEvents.Instance is null");
        }
    }

    /// <summary>
    /// 在对象销毁时注销事件监听器，防止内存泄漏
    /// </summary>
    private void OnDestroy()
    {
        // 检查GameEvents实例是否存在并注销事件
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.RandomColorModel -= DoRandomColorModel;
        }
    }

    /// <summary>
    /// 执行随机颜色变化的回调函数
    /// </summary>
    /// <param name="number">用于匹配的数字标识</param>
    private void DoRandomColorModel(int number)
    {
        // 检查数字标识是否匹配
        if (myNumber == number)
        {
            // 为所有收集的材质设置随机颜色
            foreach (Material material in materials)
            {
                Color newColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                material.color = newColor;
            }
        }
    }
}

