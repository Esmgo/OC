using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [Header("移动参数")]
    public float moveSpeed = 5f;
    public float dashSpeed = 12f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isDashing = false;
    private float dashTimeLeft;
    private float lastDashTime = -999f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // 插值设置建议在Inspector中设置
    }

    void Update()
    {
        // 获取移动输入
        moveInput.x = Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0;
        moveInput.y = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
        moveInput = moveInput.normalized;

        // 冲刺输入
        if (Input.GetKeyDown(KeyCode.Space) && !isDashing && Time.time >= lastDashTime + dashCooldown && moveInput != Vector2.zero)
        {
            isDashing = true;
            dashTimeLeft = dashDuration;
            lastDashTime = Time.time;
        }
    }

    void FixedUpdate()
    {
        Vector2 targetVelocity = isDashing ? moveInput * dashSpeed : moveInput * moveSpeed;
        rb.velocity = targetVelocity;

        if (isDashing)
        {
            dashTimeLeft -= Time.fixedDeltaTime;
            if (dashTimeLeft <= 0)
            {
                isDashing = false;
            }
        }
    }
}
