using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterMoveComponent : MonoBehaviour
{
    private float dashDuration = 0.2f;

    private float dashSpeed;
    private float moveSpeed;
    private float dashCooldown;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isDashing;
    private float lastDashTime;
    private Animator animator;

    private bool facingRight = true;
    private bool isFlipPaused = false;

    public void Init(CharacterConfiguration config)
    {
        moveSpeed = config.moveSpeed;
        dashSpeed = config.dashSpeed;
        dashCooldown = config.dashCooldown;

        rb = GetComponent<Rigidbody2D>();
        moveInput = Vector2.zero;
        isDashing = false;
        lastDashTime = -999f;
        animator = GetComponent<Animator>();

        facingRight = transform.localScale.x >= 0;
        isFlipPaused = false;
    }

    public void UpdateInfo(CharacterMoveStats moveStats)
    {
        moveStats.moveSpeed = moveSpeed;
        moveStats.dashSpeed = dashSpeed;
        moveStats.dashCooldown = dashCooldown;
    }

    private void Update()
    {
        moveInput.x = Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0;
        moveInput.y = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
        moveInput = moveInput.normalized;

        HandleDashInput();
        FlipByMouse();

        if (animator != null)
        {
            animator.SetFloat("Speed", rb.velocity.magnitude);
        }
    }

    void FixedUpdate()
    {
        if (isDashing && Time.time >= lastDashTime + dashDuration)
        {
            EndDash();
        }

        MoveAndDash();
    }

    void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && CanDash())
        {
            StartDash();
        }
    }

    bool CanDash()
    {
        return !isDashing && Time.time >= lastDashTime + dashCooldown && moveInput != Vector2.zero;
    }

    protected virtual void StartDash()
    {
        isDashing = true;
        lastDashTime = Time.time;
    }

    protected virtual void MoveAndDash()
    {
        float targetSpeed = isDashing ? dashSpeed : moveSpeed;
        rb.velocity = moveInput * targetSpeed;
    }

    protected virtual void EndDash()
    {
        isDashing = false;
    }

    // 翻转逻辑保持不变
    void FlipByMouse()
    {
        if (isFlipPaused) return;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mouseWorldPos.x > transform.position.x && !facingRight)
        {
            Flip();
        }
        else if (mouseWorldPos.x < transform.position.x && facingRight)
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
    public void PauseFlip(bool value) 
    {
        isFlipPaused = value;
    } 
}

public struct CharacterMoveStats
{
    public float moveSpeed;
    public float dashSpeed;
    public float dashCooldown;
}
