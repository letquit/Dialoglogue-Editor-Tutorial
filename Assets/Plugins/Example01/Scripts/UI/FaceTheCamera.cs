using System;
using UnityEngine;

/// <summary>
/// 使游戏对象始终面向主摄像机的脚本类
/// </summary>
public class FaceTheCamera : MonoBehaviour
{
    private Camera mainCamera;

    /// <summary>
    /// 在对象被唤醒时执行的初始化操作
    /// 获取主摄像机的引用并存储在mainCamera变量中
    /// </summary>
    private void Awake()
    {
        mainCamera = Camera.main;
    }

    /// <summary>
    /// 每帧更新时调用，使对象始终面向摄像机
    /// 通过LookAt方法让对象朝向摄像机，然后绕Y轴旋转180度来调整朝向
    /// </summary>
    private void Update()
    {
        // 让对象面向摄像机
        transform.LookAt(mainCamera.transform);
        // 绕Y轴旋转180度，调整正面朝向
        transform.Rotate(0, 180, 0);
    }
}

