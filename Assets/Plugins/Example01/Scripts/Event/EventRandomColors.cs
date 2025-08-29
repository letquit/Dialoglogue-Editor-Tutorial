using System;
using UnityEngine;

/// <summary>
/// 随机颜色事件类，继承自DialogueEventSO，用于触发随机颜色模型事件
/// </summary>
[CreateAssetMenu(fileName = "New Color Event", menuName = "Dialogue/Color Event")]
[Serializable]
public class EventRandomColors : DialogueEventSO
{
    [SerializeField] private int number;
    
    /// <summary>
    /// 执行随机颜色事件，调用GameEvents实例中的随机颜色模型方法
    /// </summary>
    public override void RunEvent()
    {
        // 检查GameEvents实例是否存在，防止空引用异常
        if (GameEvents.Instance != null)
        {
            // 调用随机颜色模型方法，传入number参数
            GameEvents.Instance.CallRandomColorModel(number);
        }
        else
        {
            // 记录错误日志，提示GameEvents实例为空
            Debug.LogError("EventRandomColors: GameEvents.Instance is null");
        }
    }
}
