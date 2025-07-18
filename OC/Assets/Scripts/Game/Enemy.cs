using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening; // 引入DOTween命名空间

public class Enemy : MonoBehaviour
{
    [Title("测试属性")]
    public TextMeshProUGUI testText;

    [Title("移动参数")]
    public float moveSpeed = 2f;

    [Title("属性")]
    public int maxHp = 3;
    private int hp;

    private Transform player; 
    public SpriteRenderer spriteRenderer; // 新增

    void Awake()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>(); // 获取SpriteRenderer

        // 自动查找场景中的玩家（带有Move组件的对象）
        var moveObj = FindObjectOfType<Move>();
        if (moveObj != null)
            player = moveObj.transform;

        hp = maxHp;
    }

    void OnEnable()
    {
        // 每次重新启用时重置血量和状态
        hp = maxHp;
        // 如有其他需要重置的状态，在此添加
    }

    void Start()
    {
        
    }
     
    void Update()
    {
        if (player == null) return;

        // 计算方向并移动
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);


        /// 更新测试文本显示
        testText.text = $"{hp}/{maxHp}";
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        FlashSprite(); // 受伤时闪烁
        if (hp <= 0)
        {
            GameApplication.Instance.AddKill(); // 新增：通知全局
            gameObject.SetActive(false); // 对象池回收
        }
    } 

    private void FlashSprite()
    {
        if (spriteRenderer == null) return;
        // 先杀死之前的tween，避免叠加
        spriteRenderer.DOKill();
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        spriteRenderer.DOColor(originalColor, 0.15f);
    }
}
