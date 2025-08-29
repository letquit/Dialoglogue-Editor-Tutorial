using System;
using UnityEngine;

/// <summary>
/// 玩家控制类，负责处理玩家角色的移动、旋转和动画控制
/// </summary>
public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotaSpeed = 5f;

    private Animator animator;
    private Rigidbody rb;
    private float vertical;
    private float horizontal;

    /// <summary>
    /// 在对象被唤醒时执行，用于初始化组件引用
    /// 获取子对象中的Animator组件和自身的Rigidbody组件
    /// </summary>
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// 每帧更新时调用，用于处理输入检测
    /// </summary>
    private void Update()
    {
        InputHandler();
    }

    /// <summary>
    /// 固定时间间隔更新时调用，用于处理物理相关的移动逻辑
    /// </summary>
    private void FixedUpdate()
    {
        Movement();
    }

    /// <summary>
    /// 处理玩家输入，获取垂直和水平轴的输入值
    /// </summary>
    private void InputHandler()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
    }

    /// <summary>
    /// 执行玩家移动逻辑，包括位置移动、角色旋转和动画控制
    /// </summary>
    private void Movement()
    {
        // 计算移动向量并应用到刚体上
        Vector3 movement = new Vector3(horizontal, 0, vertical) * moveSpeed;
        rb.linearVelocity = movement;

        // 计算角色朝向并平滑旋转
        Vector3 direction = Vector3.RotateTowards(transform.forward, movement, rotaSpeed * Time.fixedDeltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(direction);

        // 设置动画参数
        float animMove = Vector3.Magnitude(movement.normalized);
        animator.SetFloat("moveSpeed", animMove);
    }
}

