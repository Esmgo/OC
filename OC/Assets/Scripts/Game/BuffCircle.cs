using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BuffSystem;
using UnityEngine.VFX;

public class BuffCircle : MonoBehaviour, IPoolable
{
    [Header("基本设置")]
    [Tooltip("给予间隔(秒)")]
    [SerializeField] private float giveInterval = 0.9f;
    [Tooltip("buff类型")]
    [SerializeField] private BuffType buffType = BuffType.Berserk;
    [Tooltip("范围")]
    [SerializeField] private float radius = 5f;
    [Tooltip("持续时间")]
    [SerializeField] private float activeDuration = 10f;

    private HashSet<BuffManager> managers = new();
    private CircleCollider2D circleZoneCollider;
    private float timer = 0f;
    private float recycleTimer = 0f;
    private PooledObject pooledObject;
    private bool isActive = false;

    private void Awake()
    {
        circleZoneCollider = GetComponentInChildren<CircleCollider2D>();
        circleZoneCollider.isTrigger = true;
        circleZoneCollider.radius = radius;
        
    }

    private void Update()
    {
        if(timer >= giveInterval)
        {
            foreach(var manager in managers)
            {
                if(manager != null)
                {
                    AddBuff(manager, 1);
                }
            }
            timer = 0f;
        }
        else
        {
            timer += Time.deltaTime;
        }
        if(recycleTimer >= activeDuration)
        {
            recycleTimer = 0f;
            ReturnToPool();
            isActive = false;
        }
        else
        {
            recycleTimer += Time.deltaTime;
        }
    }

    private void ReturnToPool()
    {
        if (pooledObject != null)
        {
            pooledObject.ReturnToPool();
        }
        else
        {
            Debug.LogWarning($"ParticleRecycler: {gameObject.name} 没有 PooledObject 组件，直接销毁");
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BuffManager manager = other.GetComponent<BuffManager>();
        if(manager != null && !managers.Contains(manager))
        {
            managers.Add(manager);
            AddBuff(manager,1);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        BuffManager manager = collision.GetComponent<BuffManager>();
        if (manager != null && managers.Contains(manager))
        {
            AddBuff(manager, 1);
            managers.Remove(manager);
        }
    }

    private void AddBuff(BuffManager manager, float duration)
    {
        switch (buffType)
        {
            case BuffType.Berserk:
                manager.AddBuff<BerserkBuff>(duration);
                break;
        }
    }

    public void OnGetFromPool()
    {
        isActive = true;
        pooledObject = GetComponent<PooledObject>();
    }

    public void OnReturnToPool()
    {
        isActive = false;
    }
}
