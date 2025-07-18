using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening; // ����DOTween�����ռ�

public class Enemy : MonoBehaviour
{
    [Title("��������")]
    public TextMeshProUGUI testText;

    [Header("�ƶ�����")]
    public float moveSpeed = 2f;

    [Header("����")]
    public int maxHp = 3;
    protected int hp;

    protected Transform player;
    public SpriteRenderer spriteRenderer; // ����

    protected virtual void Awake()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>(); // ��ȡSpriteRenderer

        // �Զ����ҳ����е���ң�����Move����Ķ���
        var moveObj = FindObjectOfType<Move>();
        if (moveObj != null)
            player = moveObj.transform;

        hp = maxHp;
    }

    protected virtual void OnEnable()
    {
        // ÿ����������ʱ����Ѫ����״̬
        hp = maxHp;
        // ����������Ҫ���õ�״̬���ڴ����
        spriteRenderer.color = Color.white; // ������ɫΪ��ɫ
    }

    void Start()
    {
        
    }
     
    protected virtual void Update()
    {
        if (player == null) return;

        // ���㷽���ƶ�
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);


        /// ���²����ı���ʾ
        testText.text = $"{hp}/{maxHp}";
    }

    public virtual void TakeDamage(int damage)
    {
        hp -= damage;
        DamageTextPool.Instance.Show(damage, transform.position + Vector3.up * 1.2f);
        FlashSprite(); // ����ʱ��˸
        if (hp <= 0)
        {
            GameApplication.Instance.AddKill(); // ������֪ͨȫ��
            gameObject.SetActive(false); // ����ػ���
        }
    } 

    private void FlashSprite()
    {
        if (spriteRenderer == null) return;
        // ��ɱ��֮ǰ��tween���������
        spriteRenderer.DOKill();
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        spriteRenderer.DOColor(originalColor, 0.15f);
    }
}
