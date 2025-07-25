using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Threading.Tasks;
using Mirror;

public class Attack : NetworkBehaviour
{
    [Header("攻击参数")]
    public string bulletAddress = "PlayerBullet"; // Addressables子弹地址
    public float bulletSpeed = 10f; // 子弹速度
    public float attackInterval = 0.3f; // 射击间隔（秒）

    private float lastAttackTime = -999f;   // 上次攻击时间
    private Camera cam; // 主摄像机引用

    // 缓存已加载的子弹预制体
    private GameObject cachedBulletPrefab;

    // 外部初始化完成回调
    private Action onInitComplete;

    private AsyncOperationHandle<GameObject>? bulletHandle = null;

    public GameObject bulletPrefab;

    void Awake()
    {
        cam = Camera.main;
    }

    void OnEnable()
    {
        //if (GameApplication.Instance != null)
        //    GameApplication.Instance.OnKillMilestone += OnKillMilestone;
    }

    void OnDisable()
    {
        //if (GameApplication.Instance != null)
        //    GameApplication.Instance.OnKillMilestone -= OnKillMilestone;
    }

    void Update()
    {
        if (isLocalPlayer && Input.GetMouseButton(0))
        {
            Vector3 spawnPosition = transform.position + transform.forward;
            Quaternion spawnRotation = transform.rotation;

            if (GameNetworkManager.IsLANGame)
            {
                CmdShoot(spawnPosition, spawnRotation); // 局域网模式下通过服务器同步
            }
            else
            {
                Shoot(spawnPosition, spawnRotation); // 非局域网模式下直接本地生成
            }
        }

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
    }

    void Shoot()
    {
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Vector2 dir = (mouseWorldPos - transform.position).normalized;

        GameObject bullet = Instantiate(cachedBulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = dir * bulletSpeed;
        }
        bullet.transform.right = dir;
    }

    private void Shoot(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = BulletPool.Instance.GetBullet(position, rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = transform.forward * bulletSpeed;
    }

    [Command] // 从客户端调用，服务器执行
    public void CmdShoot(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = BulletPool.Instance.GetBullet(position, rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = transform.forward * bulletSpeed;
    }

    private void OnKillMilestone(int milestone)
    {
        attackInterval *= 0.75f;
        attackInterval = Mathf.Max(attackInterval, 0.05f);
    }

    // 在脚本销毁时释放
    void OnDestroy()
    {
        if (bulletHandle.HasValue && bulletHandle.Value.IsValid())
            Addressables.Release(bulletHandle.Value);
    }
}
