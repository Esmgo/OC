using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Threading.Tasks;

public class Attack : MonoBehaviour
{
    [Header("攻击参数")]
    public string bulletAddress = "PlayerBullet";    // Addressables子弹地址
    public float bulletSpeed = 10f;     // 子弹速度
    public float attackInterval = 0.3f;     // 射击间隔（秒）

    private float lastAttackTime = -999f;   // 上次攻击时间
    private Camera cam;  // 主摄像机引用

    // 缓存已加载的子弹预制体
    private GameObject cachedBulletPrefab;

    // 外部初始化完成回调
    private Action onInitComplete;

    private ObjectPool objectPool;

    private AsyncOperationHandle<GameObject>? bulletHandle = null;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= lastAttackTime + attackInterval && cachedBulletPrefab != null)
        {
            Shoot();
            lastAttackTime = Time.time;
        }
    }

    /// <summary>
    /// 外部初始化，异步加载子弹预制体
    /// </summary>
    public async Task InitAsync(string bulletAddress, float bulletSpeed = 10f, float attackInterval = 0.3f, Action onComplete = null)
    {
        this.bulletAddress = bulletAddress;
        this.bulletSpeed = bulletSpeed;
        this.attackInterval = attackInterval;
        onInitComplete = onComplete;

        // 释放旧的 handle
        if (bulletHandle.HasValue && bulletHandle.Value.IsValid())
            Addressables.Release(bulletHandle.Value);

        bulletHandle = Addressables.LoadAssetAsync<GameObject>(bulletAddress);
        await bulletHandle.Value.Task;
        if (bulletHandle.Value.Status == AsyncOperationStatus.Succeeded)
        {
            cachedBulletPrefab = bulletHandle.Value.Result;
            onInitComplete?.Invoke();
        }
        else
        {
            Debug.LogError($"子弹加载失败: {bulletAddress}");
        }

        objectPool = ObjectPoolManager.Instance.GetOrCreatePool("bulletTest", cachedBulletPrefab, 10);
    }

    void Shoot()
    {
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Vector2 dir = (mouseWorldPos - transform.position).normalized;

        GameObject bullet = objectPool.GetObject(transform.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = dir * bulletSpeed;
        }
        bullet.transform.right = dir;
    }

    // 在脚本销毁时释放
    void OnDestroy()
    {
        if (bulletHandle.HasValue && bulletHandle.Value.IsValid())
            Addressables.Release(bulletHandle.Value);
    }
}
