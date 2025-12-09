using GameEvents;
using UnityEngine;

/// <summary>
/// 角色的移动脚本。
/// 它的移动参数直接由 Entity 的属性系统驱动。
/// </summary>
public class MoveBase : MonoBehaviour
{
    //[Header("冲刺参数")]
    //private float dashDuration = 0.2f; // 冲刺持续时间，这是一个固定值

    //// --- 运行时变量 ---
    //private Rigidbody2D rb;
    //private Vector2 moveInput;
    //private bool isDashing = false;
    //private float lastDashTime = -999f;

    //private Camera cam;
    //private bool facingRight = true;
    //private Animator animator;
    //private bool isFlipPaused = false;

    //// --- 核心改动：持有 Entity 的引用 ---
    //private Character owner;

    //void Awake()
    //{
    //    // 获取必要的组件引用
    //    rb = GetComponent<Rigidbody2D>();
    //    animator = GetComponent<Animator>();
    //    owner = GetComponent<Character>(); // 获取 Entity 组件
    //    cam = Camera.main;
    //}

    //// OnEnable 和 OnDestroy 保持不变，用于处理攻击时的翻转暂停
    //private void OnEnable()
    //{
    //    EventCenter.Subscribe<AttackStartEvent>(PauseFlip);
    //    EventCenter.Subscribe<AttackEndEvent>(ResumeFlip);
    //}

    //private void OnDestroy()
    //{
    //    EventCenter.Unsubscribe<AttackStartEvent>(PauseFlip);
    //    EventCenter.Unsubscribe<AttackEndEvent>(ResumeFlip);
    //}

    //void Update()
    //{
    //    // 输入处理保持不变
    //    moveInput.x = Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0;
    //    moveInput.y = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
    //    moveInput = moveInput.normalized;

    //    HandleDashInput();
    //    FlipByMouse();

    //    if (animator != null)
    //    {
    //        animator.SetFloat("Speed", rb.velocity.magnitude);
    //    }
    //}

    //void FixedUpdate()
    //{
    //    // 在 FixedUpdate 的开头检查冲刺是否结束
    //    if (isDashing && Time.time >= lastDashTime + dashDuration)
    //    {
    //        EndDash();
    //    }

    //    // 移动和冲刺逻辑现在直接使用 owner 的属性
    //    MoveAndDash();
    //}

    //void HandleDashInput()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space) && CanDash())
    //    {
    //        StartDash();
    //    }
    //}

    ///// <summary>
    ///// 检查是否可以冲刺。现在使用 owner 的属性。
    ///// </summary>
    //bool CanDash()
    //{
    //    // 使用 owner.currentDashCoolDown
    //    return !isDashing && Time.time >= lastDashTime + owner.currentDashCoolDown && moveInput != Vector2.zero;
    //}

    //protected virtual void StartDash()
    //{
    //    isDashing = true;
    //    lastDashTime = Time.time;
    //}

    ///// <summary>
    ///// 将移动和冲刺的物理更新合并到一个方法中。
    ///// </summary>
    //protected virtual void MoveAndDash()
    //{
    //    if (owner == null) return;

    //    // 根据是否在冲刺，选择使用 owner 的冲刺速度或移动速度
    //    float targetSpeed = isDashing ? owner.currentDashSpeed : owner.currentMoveSpeed;
    //    rb.velocity = moveInput * targetSpeed;
    //}

    //protected virtual void EndDash()
    //{
    //    isDashing = false;
    //}

    //// 翻转逻辑保持不变
    //void FlipByMouse()
    //{
    //    if (cam == null || isFlipPaused) return;
    //    Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
    //    if (mouseWorldPos.x > transform.position.x && !facingRight)
    //    {
    //        Flip();
    //    }
    //    else if (mouseWorldPos.x < transform.position.x && facingRight)
    //    {
    //        Flip();
    //    }
    //}

    //void Flip()
    //{
    //    facingRight = !facingRight;
    //    Vector3 scale = transform.localScale;
    //    scale.x *= -1;
    //    transform.localScale = scale;
    //}

    //public void PauseFlip() => isFlipPaused = true;
    //private void ResumeFlip() => isFlipPaused = false;
}
