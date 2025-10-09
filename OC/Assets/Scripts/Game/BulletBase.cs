using UnityEngine;
using Mirror;

public class BulletBase : MonoBehaviour, IPoolable
{
    [Header("子弹属性")]
    [Tooltip("子弹速度")]
    [SerializeField] protected float speed = 10f;
    
    [Tooltip("子弹生命周期")]
    [SerializeField] protected float lifetime = 10f;
    
    [Tooltip("子弹伤害")]
    [SerializeField] protected int damage = 0;
    
    [Tooltip("目标层级")]
    [SerializeField] protected LayerMask targetLayers = -1;

    [Tooltip("击退力度")]
    [SerializeField] protected float knockBackForce = 5f;

    [Header("效果")]
    public GameObject hitEffectPrefab;

    private Rigidbody2D rb; // 子弹的Rigidbody2D组件
    private float timer;    // 计时器，用于跟踪子弹生命周期
    private Vector2 direction;  // 子弹飞行方向
    private bool isActive;  // 子弹是否处于激活状态

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            ReturnToPool();
        }
    }

    /// <summary>
    /// 初始化子弹
    /// </summary>
    /// <param name="dir">方向</param>
    /// <param name="bulletDamage">伤害</param>
    public virtual void Initialize(Vector2 direction, int damage, float speed, LayerMask targetLayers, float knockBackForce)
    {
        this.direction = direction.normalized;
        this.damage = damage;
        this.speed = speed;
        this.targetLayers = targetLayers;
        this.knockBackForce = knockBackForce;

        timer = 0f;
        isActive = true;

        BulletMoveBase();
    }

    protected virtual void BulletMoveBase()
    {
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
        else
        {
            Debug.LogWarning("Bullet Rigidbody is not assigned. Using transform for movement.");
        }
    }

    public void ResetState()
    {
        rb.velocity = Vector3.zero;

        timer = 0f;
        isActive = true;
    }

    private void ReturnToPool()
    {
        isActive = false;
        ObjectPoolManager.Instance.ReturnObject(GetComponent<PooledObject>().GetPoolName(), gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        // 检查目标层级
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            // 造成伤害
            var enemy = other.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, 0);

                Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                enemy.ApplyKnockback(knockbackDirection, knockBackForce);
            }
            else
            {
                Debug.LogWarning($"Bullet hit {other.name}, but no Enemy component found.");
            }

                //// 创建击中效果
                //if (hitEffectPrefab != null)
                //{
                //    if (ObjectPoolManager.Instance != null && ObjectPoolManager.Instance.HasPool("HitEffect"))
                //    {
                //        var effect = ObjectPoolManager.Instance.GetObject("HitEffect", transform.position, Quaternion.identity);
                //        var pooledObj = effect.GetComponent<PooledObject>();
                //        if (pooledObj != null)
                //        {
                //            pooledObj.ReturnToPoolDelayed(1f);
                //        }
                //    }
                //    else
                //    {
                //        var effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                //        Destroy(effect, 1f);
                //    }
                //}

            ReturnToPool();
        }
    }

    #region 池对象接口实现

    public void OnGetFromPool()
    {
        ResetState();
    }

    public void OnReturnToPool()
    {
        isActive = false;
        rb.velocity = Vector3.zero;
    }
    
    #endregion
}