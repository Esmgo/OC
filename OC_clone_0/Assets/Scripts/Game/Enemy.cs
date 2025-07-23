using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening; // 引入DOTween命名空间

public class Enemy : MonoBehaviour
{
    [Header("测试属性")]
    public TextMeshProUGUI testText;

    [Header("移动参数")]
    public float moveSpeed = 2f;

    [Header("属性")]
    public int maxHp = 3;
    protected int hp;

    protected Transform player;
    public SpriteRenderer spriteRenderer; // 新增

    private Rigidbody2D rb;
    private Tween flashTween;
    private Color originalColor = Color.white; // 可在OnEnable缓存

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // 获取Rigidbody2D

        // 只在Awake缓存一次原色
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        //// 自动查找场景中的玩家（带有Move组件的对象）
        //var moveObj = FindObjectOfType<Move>();
        //if (moveObj != null)
        //    player = moveObj.transform;

        hp = maxHp;
    }

    protected virtual void OnEnable()
    {
        player = EnemyManager.Instance.role; // 获取玩家角色引用
        // 每次重新启用时重置血量和状态
        hp = maxHp;
        // 还原颜色并Kill Tween
        if (spriteRenderer != null)
        {
            if (flashTween != null && flashTween.IsActive()) flashTween.Kill();
            spriteRenderer.color = originalColor;
        }
    }

    void Start()
    {
        
    }

    protected virtual void Update()
    {
        
    }

    protected virtual void FixedUpdate() 
    {
        Move(); // 移动逻辑

        // 更新测试文本显示
        if (testText != null)
            testText.text = $"{hp}/{maxHp}";
    }

    protected virtual void Move()
    {
        if (player == null) return;

        // 计算方向并用物理方式移动
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    public virtual void TakeDamage(int damage)
    {
        hp -= damage;
        DamageTextPool.Instance.Show(damage, transform.position + Vector3.up * 1.2f);
        FlashSprite(); // 受伤时闪烁
        if (hp <= 0)
        {
            //// 获取击杀者
            //var networkPlayer = EnemyManager.Instance.role.GetComponent<NetworkPlayer>();

            //// 如果击杀者有网络组件，通知服务器增加击杀
            //if (networkPlayer != null)
            //{
            //    networkPlayer.CmdAddKill();
            //}
            //else
            //{
            //    // 本地模式
            //    //GameApplication.Instance.AddKill();
            //}
            gameObject.SetActive(false); // 对象池回收
        }
    } 

    private void FlashSprite()
    {
        if (spriteRenderer == null) return;
        if (flashTween != null && flashTween.IsActive()) flashTween.Kill();

        spriteRenderer.color = Color.red;
        flashTween = spriteRenderer.DOColor(originalColor, 0.15f);
    }
}
