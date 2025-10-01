using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using GameEvents; // 引入DOTween命名空间

public class EnemyBase : MonoBehaviour
{
    [Header("测试属性")]
    [Tooltip("测试用显示血量")]
    [SerializeField] protected TextMeshProUGUI testText;

    [Header("移动参数")]
    [Tooltip("移动速度")]
    [SerializeField] protected float moveSpeed = 0f;

    [Header("击退参数")]
    [Tooltip("击退抗性，越大击退效果持续越短，100为完全免疫击退")]
    [Range(0.1f, 100f)]
    [SerializeField] protected float knockbackResistance = 1f;
    [Tooltip("当前击退速度，初始为0")]
    protected Vector2 knockbackVelocity = Vector2.zero; // 当前击退速度

    [Header("属性")]
    [Tooltip("最大血量")]
    [SerializeField] protected int maxHp = 100;
    [Tooltip("当前血量")]
    protected int currentHp;

    [Header("其他")]
    [SerializeField] protected Canvas canvas; //UI画布组件
    [SerializeField] protected SpriteRenderer spriteRenderer;   // 精灵渲染器，用于闪烁效果

    protected bool isDead = false;    //死亡标记
    protected Transform player;     //玩家位置
    protected Rigidbody2D rb;
    protected Tween flashTween;
    protected Color originalColor = Color.white;

    protected virtual void Awake()
    {
        Init();
        Reset();
    }

    protected virtual void OnEnable()
    {
        Reset();
    }

    /// <summary>
    /// 初始化组件和变量
    /// </summary>
    protected virtual void Init()
    {
        rb = GetComponent<Rigidbody2D>(); // 获取Rigidbody2D
        if (spriteRenderer != null)     //缓存原色
            originalColor = spriteRenderer.color;
        currentHp = maxHp;
        isDead = false; // 初始化死亡状态
    }

    /// <summary>
    /// 重置敌人状态
    /// </summary>
    protected virtual void Reset()
    {
        player = Tools.GetCharacter().transform;

        // 每次重新启用时重置血量和状态
        currentHp = maxHp;
        isDead = false; // 重置死亡状态
        knockbackVelocity = Vector2.zero; // 重置击退速度

        // 还原颜色并停止闪烁动画
        if (spriteRenderer != null)
        {
            if (flashTween != null && flashTween.IsActive())
                flashTween.Kill();
            spriteRenderer.color = originalColor;
        }

        // 确保碰撞器启用
        var collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = true;
    }

    protected virtual void Update()
    {
        
    }

    protected virtual void FixedUpdate() 
    {
        // 如果已死亡，停止所有行为
        if (isDead) return;
        
        // 处理击退效果
        HandleKnockback();
        
        // 只有在击退速度很小时才进行正常移动
        if (knockbackVelocity.magnitude < 1f)
        {
            Move(); // 移动逻辑
        }

        // 更新测试文本显示
        if (testText != null)
            testText.text = $"{currentHp}/{maxHp}";
    }

    /// <summary>
    /// 处理击退效果
    /// </summary>
    protected void HandleKnockback()
    {
        if (knockbackVelocity.magnitude > 0.1f)
        {
            // 应用击退移动
            rb.MovePosition(rb.position + knockbackVelocity * Time.fixedDeltaTime);

            // 计算抗性系数：0到1之间，100时为1
            float resistanceFactor = Mathf.Clamp01(knockbackResistance / 100f);
            // 计算阻尼：抗性越大，阻尼越小（衰减越快）
            float damping = 1f - (resistanceFactor * Time.fixedDeltaTime * 15f);
            knockbackVelocity *= Mathf.Clamp01(damping);
        }
        else
        {
            knockbackVelocity = Vector2.zero;
        }
    }

    protected virtual void Move()
    {
        if (player == null || isDead) return; // 死亡后停止移动

        // 计算方向并用物理方式移动
        Vector2 direction = Tools.GetDir(transform, player);
        //Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    public virtual void TakeDamage(float physicalDamage, float energyDamage)
    {
        // 如果已死亡，不接受伤害
        if (isDead) return;

        //处理物理伤害
        bool iscritical = Tools.RandomInt(0, 99) > 90; // 10%几率暴击
        if (iscritical) 
        {
            physicalDamage = physicalDamage * 2; 
        }
        currentHp -= Mathf.FloorToInt(physicalDamage);
        
        if(physicalDamage > 0)
        {
            // 显示伤害文本
            var dt = ObjectPoolManager.Instance.GetObject<TextPopUp>("TextPopUp", transform.position);
            dt.transform.SetParent(canvas.transform);
            dt.Show(physicalDamage.ToString(), 3f, iscritical ? Color.red : Color.white, 0.6f, iscritical);
        }

        // 处理异能伤害
        currentHp -= Mathf.FloorToInt(energyDamage);
        if (energyDamage > 0)
        {
            // 显示伤害文本
            var dt = ObjectPoolManager.Instance.GetObject<TextPopUp>("TextPopUp", transform.position);
            dt.transform.SetParent(canvas.transform);
            dt.Show(energyDamage.ToString(), 3f, Color.blue, 0.6f, false);
        }


        FlashSprite(); // 受伤时闪烁
        
        if (currentHp <= 0)
        {
            Die(); // 调用死亡方法
        }
    }

    /// <summary>
    /// 敌人死亡处理
    /// </summary>
    protected virtual void Die()
    {
        if (isDead) return; // 防止重复死亡
        
        isDead = true; // 标记为已死亡
        
        // 禁用碰撞器，防止继续接受攻击
        var collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;
        
        // 停止移动
        knockbackVelocity = Vector2.zero;
        if (rb != null)
            rb.velocity = Vector2.zero;
        
        // 先发布事件，再延迟回收
        StartCoroutine(HandleDeath());
    }

    /// <summary>
    /// 处理敌人死亡逻辑（延迟回收）
    /// </summary>
    protected  virtual IEnumerator HandleDeath()
    {
        // 生成死亡效果
        ObjectPoolManager.Instance.GetObject<Item>("Coin", transform.position); // 掉落金币
        
        //设置个延迟加个死亡动画

        // 清理伤害文本
        TextPopUp[] dts = canvas.GetComponentsInChildren<TextPopUp>();
        foreach (var d in dts)
        {
            ObjectPoolManager.Instance.ReturnObject("TextPopUp", d.gameObject);
        }
        
        // 发布死亡事件
        EventCenter.Publish<EnemyDiedEvent>();
        
        // 等待几帧，确保事件处理完成
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        
        // 回收到对象池
        ObjectPoolManager.Instance.ReturnObject(GetComponent<PooledObject>().GetPoolName(), gameObject);
    }

    /// <summary>
    /// 应用击退效果
    /// </summary>
    /// <param name="knockbackDirection">击退方向</param>
    /// <param name="knockbackForce">击退力度</param>
    public virtual void ApplyKnockback(Vector2 knockbackDirection, float knockbackForce)
    {
        if (isDead) return;
        knockbackVelocity = knockbackDirection.normalized * knockbackForce;
    }

    protected void FlashSprite()
    {
        if (spriteRenderer == null || isDead) return;
        
        if (flashTween != null && flashTween.IsActive()) 
            flashTween.Kill();

        spriteRenderer.color = Color.red;
        flashTween = spriteRenderer.DOColor(originalColor, 0.15f);
    }

    /// <summary>
    /// 检查敌人是否已死亡
    /// </summary>
    public bool IsDead()
    {
        return isDead;
    }
    
    protected virtual void OnDisable()
    {
        // 停止所有动画
        if (flashTween != null && flashTween.IsActive()) 
            flashTween.Kill();
        
        knockbackVelocity = Vector2.zero;
        isDead = false; // 重置死亡状态，为下次使用做准备
    }
}
