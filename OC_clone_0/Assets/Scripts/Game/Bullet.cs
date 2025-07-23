using Mirror;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public int damage = 1;
    public float lifeTime = 2f;
    private bool hasHit = false;
    private float timer = 0f;
    public float speed = 10f;

    //private  NetworkIdentity netIdentity;
    private NetworkTransformReliable netTransform;

    void Awake()
    {
        // 获取组件引用
        //netIdentity = GetComponent<NetworkIdentity>();
        netTransform = GetComponent<NetworkTransformReliable>();
        if (GameNetworkManager.IsLANGame)
        {
            EnableNetworkComponents();
        }
        else
        {
            DisableNetworkComponents();
        }
    }

    void OnEnable()
    {
        hasHit = false;
        timer = 0f;

        // 根据是否为局域网游戏动态启用或禁用网络组件
        if (GameNetworkManager.IsLANGame)
        {
            EnableNetworkComponents();
        }
        else
        {
            DisableNetworkComponents();
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            BulletPool.Instance.ReturnBullet(gameObject);
        }

        if (isServer) // 仅在服务器上处理逻辑
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            hasHit = true;
            enemy.TakeDamage(damage);
            HitEffectPool.Instance.PlayEffect(transform.position);
            BulletPool.Instance.ReturnBullet(gameObject);
        }
    }

    public void ResetState()
    {
        hasHit = false;
        timer = 0f;
    }

    [Server]
    public void Initialize(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    private void EnableNetworkComponents()
    {
        if (netIdentity != null) netIdentity.enabled = true;
        if (netTransform != null) netTransform.enabled = true;
    }

    private void DisableNetworkComponents()
    {
        if (netIdentity != null) netIdentity.enabled = false;
        if (netTransform != null) netTransform.enabled = false;
    }
}