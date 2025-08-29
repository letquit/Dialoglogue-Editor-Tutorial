using System;
using UnityEngine;

/// <summary>
/// 游戏事件管理类，用于处理游戏中的事件分发和订阅
/// 实现单例模式，确保全局唯一实例
/// </summary>
public class GameEvents : MonoBehaviour
{
    /// <summary>
    /// 随机颜色模型事件，当需要触发随机颜色相关逻辑时调用
    /// </summary>
    private event Action<int> randomColorModel;

    /// <summary>
    /// 获取GameEvents的单例实例
    /// </summary>
    public static GameEvents Instance { get; private set; }
    
    /// <summary>
    /// 获取或设置随机颜色模型事件的访问器
    /// </summary>
    public Action<int> RandomColorModel { get => randomColorModel; set => randomColorModel = value; }
    
    /// <summary>
    /// Unity生命周期函数，在对象启用时调用
    /// 用于初始化单例实例，确保全局唯一性
    /// </summary>
    private void Awake()
    {
        // 单例模式实现：检查是否已存在实例
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
    
    /// <summary>
    /// 调用随机颜色模型事件
    /// </summary>
    /// <param name="number">传递给事件处理函数的整数参数</param>
    public void CallRandomColorModel(int number)
    {
        randomColorModel?.Invoke(number);
    }
}

