using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening; // ����DOTween�����ռ�

public class Enemy : MonoBehaviour
{
    [Header("��������")]
    public TextMeshProUGUI testText;

    [Header("�ƶ�����")]
    public float moveSpeed = 2f;

    [Header("����")]
    public int maxHp = 3;
    protected int hp;

    protected Transform player;
    public SpriteRenderer spriteRenderer; // ����

    private Rigidbody2D rb;
    private Tween flashTween;
    private Color originalColor = Color.white; // ����OnEnable����

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // ��ȡRigidbody2D

        // ֻ��Awake����һ��ԭɫ
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        hp = maxHp;
    }

    protected virtual void OnEnable()
    {
        if (EnemyManager.Instance != null)
        {
            player = EnemyManager.Instance.role;
        }
        
        // ÿ����������ʱ����Ѫ����״̬
        hp = maxHp;
        
        // ��ԭ��ɫ��ֹͣ��˸����
        if (spriteRenderer != null)
        {
            if (flashTween != null && flashTween.IsActive()) 
                flashTween.Kill();
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
        Move(); // �ƶ��߼�

        // ���²����ı���ʾ
        if (testText != null)
            testText.text = $"{hp}/{maxHp}";
    }

    protected virtual void Move()
    {
        if (player == null) return;

        // ���㷽��������ʽ�ƶ�
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    public virtual void TakeDamage(int damage)
    {
        hp -= damage;
        
        // ��ʾ�˺��ı�
        if (DamageTextPool.Instance != null)
        {
            DamageTextPool.Instance.Show(damage, transform.position + Vector3.up * 1.2f);
        }
        
        FlashSprite(); // ����ʱ��˸
        
        if (hp <= 0)
        {            
            gameObject.SetActive(false); // ����ػ���
        }
    } 

    private void FlashSprite()
    {
        if (spriteRenderer == null) return;
        
        if (flashTween != null && flashTween.IsActive()) 
            flashTween.Kill();

        spriteRenderer.color = Color.red;
        flashTween = spriteRenderer.DOColor(originalColor, 0.15f);
    }
    
    protected virtual void OnDisable()
    {
        // ֹͣ���ж���
        if (flashTween != null && flashTween.IsActive()) 
            flashTween.Kill();
    }
}
