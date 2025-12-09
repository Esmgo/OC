using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinItem : MonoBehaviour,IPoolable
{
    protected bool isAttracted = false; // 是否被吸引
    protected Transform target; // 吸引目标
    protected float speed = 1f; // 吸引速度

    protected Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
    }
    private void Update()
    {
        if (isAttracted && target != null) 
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );
        }
    }

    public void Attract(Transform targetTransform, float speed) 
    {
        isAttracted = true;
        target = targetTransform;
        this.speed = speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnCollected();
    }

    protected virtual void OnCollected()
    {
        //ItemManager.Instance.Coin(1);
        ObjectPoolManager.Instance.ReturnObject(gameObject); // 将金币返回对象池
    }

    public void OnGetFromPool()
    {
    }

    public void OnReturnToPool()
    {
        isAttracted = false; // 重置吸引状态
        target = null; // 清除目标
        if(rb != null) rb.velocity = Vector2.zero; // 重置速度
    }
}
