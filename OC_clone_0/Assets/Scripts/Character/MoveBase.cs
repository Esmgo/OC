using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBase : MonoBehaviour
{
    [Header("�ƶ�����")]
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
    private bool facingRight = true; // ��ʼ����
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        animator = GetComponent<Animator>();
        // ��ֵ���ý�����Inspector������
    }

    /// <summary>
    /// ��ʼ���ƶ����
    /// </summary>
    /// <param name="config">��ɫ����</param>
    public virtual void Init(RoleConfiguration config)
    {
        moveSpeed = config.moveSpeed;
        dashSpeed = config.dashSpeed;
        dashCooldown = config.dashCooldown;
        
        // �����ƶ�״̬
        isDashing = false;
        dashTimeLeft = 0f;
        lastDashTime = -999f;
        moveInput = Vector2.zero;
        
        Debug.Log($"MoveBase ��ʼ����� - �ƶ��ٶ�: {moveSpeed}, ����ٶ�: {dashSpeed}");
    }

    void Update()
    {
        // ��ȡ�ƶ�����
        moveInput.x = Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0;
        moveInput.y = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
        moveInput = moveInput.normalized;

        // �������
        HandleDashInput();

        // ���λ�÷�ת
        FlipByMouse();

        // ���ö�������
        if (animator != null)
        {
            animator.SetFloat("Speed", rb.velocity.magnitude);
        }
    }

    void FixedUpdate()
    {
        Dash();
        // ���³��״̬
        UpdateDash();
    }

    /// <summary>
    /// ����������
    /// </summary>
    void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && CanDash())
        {
            StartDash();
        }
    }

    /// <summary>
    /// ����Ƿ���Գ��
    /// </summary>
    bool CanDash()
    {
        return !isDashing && Time.time >= lastDashTime + dashCooldown && moveInput != Vector2.zero;
    }

    /// <summary>
    /// ��ʼ���
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
    /// ���³��״̬
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
    /// �������
    /// </summary>
    protected virtual void EndDash()
    {
        isDashing = false;
    }

    /// <summary>
    /// ��ɫ�������λ�÷�ת
    /// </summary>
    void FlipByMouse()
    {
        if (cam == null) return;
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        float mouseX = mouseWorldPos.x;
        float playerX = transform.position.x;

        // ������Ҳ࣬���ﳯ�ң��������࣬���ﳯ��
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
    /// ��ȡ��ǰ�ƶ�״̬��Ϣ
    /// </summary>
    public virtual void GetMovementInfo(out float currentSpeed, out bool isDashingState, out float dashCooldownRemaining)
    {
        currentSpeed = rb != null ? rb.velocity.magnitude : 0f;
        isDashingState = isDashing;
        dashCooldownRemaining = Mathf.Max(0f, (lastDashTime + dashCooldown) - Time.time);
    }
}
