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

    [Title("�ƶ�����")]
    public float moveSpeed = 2f;

    [Title("����")]
    public int maxHp = 3;
    private int hp;

    private Transform player; 
    public SpriteRenderer spriteRenderer; // ����

    void Awake()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>(); // ��ȡSpriteRenderer

        // �Զ����ҳ����е���ң�����Move����Ķ���
        var moveObj = FindObjectOfType<Move>();
        if (moveObj != null)
            player = moveObj.transform;

        hp = maxHp;
    }

    void OnEnable()
    {
        // ÿ����������ʱ����Ѫ����״̬
        hp = maxHp;
        // ����������Ҫ���õ�״̬���ڴ����
    }

    void Start()
    {
        
    }
     
    void Update()
    {
        if (player == null) return;

        // ���㷽���ƶ�
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);


        /// ���²����ı���ʾ
        testText.text = $"{hp}/{maxHp}";
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
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
