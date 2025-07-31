using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBase : MonoBehaviour
{
    [Header("移动参数")]
    public float moveSpeed = 1f;
    public float dashSpeed = 2f;
    public float dashCooldown = 0.5f;

    private float dashDuration = 0.2f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isDashing = false;
    private float dashTimeLeft;
    private float lastDashTime = -999f;

    private Camera cam;
    private bool facingRight = true; // 初始朝右
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        animator = GetComponent<Animator>();
        // 插值设置建议在Inspector中设置
    }

    /// <summary>
    /// 初始化移动组件
    /// </summary>
    /// <param name="config">角色配置</param>
    public virtual void Init(RoleConfiguration config)
    {
        moveSpeed = config.moveSpeed;
        dashSpeed = config.dashSpeed;
        dashCooldown = config.dashCooldown;
        
        // 重置移动状态
        isDashing = false;
        dashTimeLeft = 0f;
        lastDashTime = -999f;
        moveInput = Vector2.zero;
        
        Debug.Log($"MoveBase 初始化完成 - 移动速度: {moveSpeed}, 冲刺速度: {dashSpeed}");
    }

    void Update()
    {
        // 获取移动输入
        moveInput.x = Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0;
        moveInput.y = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
        moveInput = moveInput.normalized;

        // 冲刺输入
        HandleDashInput();

        // 鼠标位置反转
        FlipByMouse();

        // 设置动画参数
        if (animator != null)
        {
            animator.SetFloat("Speed", rb.velocity.magnitude);
        }
    }

    void FixedUpdate()
    {
        Dash();
        // 更新冲刺状态
        UpdateDash();
    }

    /// <summary>
    /// 处理冲刺输入
    /// </summary>
    void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && CanDash())
        {
            StartDash();
        }
    }

    /// <summary>
    /// 检查是否可以冲刺
    /// </summary>
    bool CanDash()
    {
        return !isDashing && Time.time >= lastDashTime + dashCooldown && moveInput != Vector2.zero;
    }

    /// <summary>
    /// 开始冲刺
    /// </summary>
    protected virtual void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        lastDashTime = Time.time;
    }

    protected virtual void Dash()
    {
        Vector2 targetVelocity = isDashing ? moveInput * dashSpeed : moveInput * moveSpeed;
        rb.velocity = targetVelocity;
    }

    /// <summary>
    /// 更新冲刺状态
    /// </summary>
    void UpdateDash()
    {
        if (isDashing)
        {
            dashTimeLeft -= Time.fixedDeltaTime;
            if (dashTimeLeft <= 0)
            {
                EndDash();
            }
        }
    }

    /// <summary>
    /// 结束冲刺
    /// </summary>
    protected virtual void EndDash()
    {
        isDashing = false;
    }

    /// <summary>
    /// 角色随着鼠标位置翻转
    /// </summary>
    void FlipByMouse()
    {
        if (cam == null) return;
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        float mouseX = mouseWorldPos.x;
        float playerX = transform.position.x;

        // 鼠标在右侧，人物朝右；鼠标在左侧，人物朝左
        if (mouseX > playerX && !facingRight)
        {
            Flip();
        }
        else if (mouseX < playerX && facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    /// <summary>
    /// 获取当前移动状态信息
    /// </summary>
    public virtual void GetMovementInfo(out float currentSpeed, out bool isDashingState, out float dashCooldownRemaining)
    {
        currentSpeed = rb != null ? rb.velocity.magnitude : 0f;
        isDashingState = isDashing;
        dashCooldownRemaining = Mathf.Max(0f, (lastDashTime + dashCooldown) - Time.time);
    }
}
