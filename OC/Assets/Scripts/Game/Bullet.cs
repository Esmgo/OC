using UnityEngine;
using Mirror;

public class Bullet : MonoBehaviour, IPoolable
{
    [Header("子弹属性")]
    public float speed = 10f;
    public float lifetime = 5f;
    public int damage = 10;
    public LayerMask targetLayers = -1;

    [Header("效果")]
    public GameObject hitEffectPrefab;

    private Rigidbody rb;
    private float timer;
    private Vector3 direction;
    private bool isActive;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
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

    public void Initialize(Vector3 dir, int bulletDamage = -1)
    {
        direction = dir.normalized;
        if (bulletDamage > 0)
            damage = bulletDamage;
        
        timer = 0f;
        isActive = true;

        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
        else
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    public void ResetState()
    {
        timer = 0f;
        isActive = true;
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void ReturnToPool()
    {
        isActive = false;
        
        // 优先使用通用对象池管理器
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ReturnObject("bulletTest", gameObject);
        }
        // 回退到专用子弹池
        else if (BulletPool.Instance != null)
        {
            BulletPool.Instance.ReturnBullet(gameObject);
        }
        else
        {
            // 如果没有池，直接销毁
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        // 检查目标层级
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            // 造成伤害
            var character = other.GetComponent<Character>();
            if (character != null)
            {
                //character.TakeDamage(damage);
            }

            // 创建击中效果
            if (hitEffectPrefab != null)
            {
                if (ObjectPoolManager.Instance != null && ObjectPoolManager.Instance.HasPool("HitEffect"))
                {
                    var effect = ObjectPoolManager.Instance.GetObject("HitEffect", transform.position, Quaternion.identity);
                    var pooledObj = effect.GetComponent<PooledObject>();
                    if (pooledObj != null)
                    {
                        pooledObj.ReturnToPoolDelayed(1f);
                    }
                }
                else
                {
                    var effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                    Destroy(effect, 1f);
                }
            }

            ReturnToPool();
        }
    }

    #region IPoolable Implementation
    
    public void OnGetFromPool()
    {
        ResetState();
    }

    public void OnReturnToPool()
    {
        isActive = false;
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
    
    #endregion
}