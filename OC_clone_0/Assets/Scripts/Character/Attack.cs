using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Threading.Tasks;

public class Attack : MonoBehaviour
{
    [Header("��������")]
    public string bulletAddress = "PlayerBullet";    // Addressables�ӵ���ַ
    public float bulletSpeed = 10f;     // �ӵ��ٶ�
    public float attackInterval = 0.3f;     // ���������룩

    private float lastAttackTime = -999f;   // �ϴι���ʱ��
    private Camera cam;  // �����������

    // �����Ѽ��ص��ӵ�Ԥ����
    private GameObject cachedBulletPrefab;

    // �ⲿ��ʼ����ɻص�
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
    /// �ⲿ��ʼ�����첽�����ӵ�Ԥ����
    /// </summary>
    public async Task InitAsync(string bulletAddress, float bulletSpeed = 10f, float attackInterval = 0.3f, Action onComplete = null)
    {
        this.bulletAddress = bulletAddress;
        this.bulletSpeed = bulletSpeed;
        this.attackInterval = attackInterval;
        onInitComplete = onComplete;

        // �ͷžɵ� handle
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
            Debug.LogError($"�ӵ�����ʧ��: {bulletAddress}");
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

    // �ڽű�����ʱ�ͷ�
    void OnDestroy()
    {
        if (bulletHandle.HasValue && bulletHandle.Value.IsValid())
            Addressables.Release(bulletHandle.Value);
    }
}
