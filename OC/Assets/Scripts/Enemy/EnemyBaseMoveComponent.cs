using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBaseMoveComponent : MonoBehaviour
{
    public float moveSpeed { get; protected set; }

    private Rigidbody2D rb;

    private Transform target;
    private float SelectTargetInterval = 1.0f;
    private float lastSelectTargetTime = 0.0f;

    private bool isDead = false;

    public void Init(EnemyConfiguration config)
    {
        moveSpeed = config.moveSpeed;
        rb = GetComponent<Rigidbody2D>();
        OnInit(config);
    }

    protected void Update()
    {
        if(lastSelectTargetTime + SelectTargetInterval < Time.time)
        {
            GetTarget();
            lastSelectTargetTime = Time.time;
        }
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {
        if (target == null || isDead) return; // 死亡后停止移动

        // 计算方向并用物理方式移动
        Vector2 direction = Tools.GetDir(transform, target);
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }
    protected virtual void GetTarget()
    {
        target = CharacterManager.Instance.currentCharacter.transform;
    }

    protected virtual void OnInit(EnemyConfiguration config) { }
}
