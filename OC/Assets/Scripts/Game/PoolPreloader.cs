using UnityEngine;

/// <summary>
/// �����Ԥ������������Ϸ��ʼʱԤ�������ö����
/// </summary>
public class PoolPreloader : MonoBehaviour
{
    [Header("Ԥ��������")]
    public GameObject bulletPrefab;
    public GameObject damageTextPrefab;
    public GameObject hitEffectPrefab;
    public GameObject enemyPrefab;

    [Header("�ش�С����")]
    public int bulletPoolSize = 50;
    public int damageTextPoolSize = 20;
    public int hitEffectPoolSize = 30;
    public int enemyPoolSize = 15;

    [Header("Ԥ��������")]
    public bool preloadOnStart = true;
    public bool preloadOnAwake = false;

    void Awake()
    {
        if (preloadOnAwake)
        {
            PreloadPools();
        }
    }

    void Start()
    {
        if (preloadOnStart && !preloadOnAwake)
        {
            PreloadPools();
        }
    }

    [ContextMenu("Preload All Pools")]
    public void PreloadPools()
    {
        if (ObjectPoolManager.Instance == null)
        {
            Debug.LogError("ObjectPoolManager.Instance Ϊ�գ��޷�Ԥ���ض����");
            return;
        }

        Debug.Log("��ʼԤ���ض����...");

        // Ԥ�����ӵ���
        if (bulletPrefab != null)
        {
            var bulletPool = ObjectPoolManager.Instance.GetOrCreatePool("Bullet", bulletPrefab, bulletPoolSize);
            bulletPool.Prewarm(bulletPoolSize);
            Debug.Log($"�ӵ���Ԥ�������: {bulletPoolSize} ������");
        }

        // Ԥ�����˺��ı���
        if (damageTextPrefab != null)
        {
            var damagePool = ObjectPoolManager.Instance.GetOrCreatePool("DamageText", damageTextPrefab, damageTextPoolSize);
            damagePool.Prewarm(damageTextPoolSize);
            Debug.Log($"�˺��ı���Ԥ�������: {damageTextPoolSize} ������");
        }

        // Ԥ���ػ���Ч����
        if (hitEffectPrefab != null)
        {
            var hitEffectPool = ObjectPoolManager.Instance.GetOrCreatePool("HitEffect", hitEffectPrefab, hitEffectPoolSize);
            hitEffectPool.Prewarm(hitEffectPoolSize);
            Debug.Log($"����Ч����Ԥ�������: {hitEffectPoolSize} ������");
        }

        // Ԥ���ص��˳�
        if (enemyPrefab != null)
        {
            var enemyPool = ObjectPoolManager.Instance.GetOrCreatePool("Enemy", enemyPrefab, enemyPoolSize);
            enemyPool.Prewarm(enemyPoolSize);
            Debug.Log($"���˳�Ԥ�������: {enemyPoolSize} ������");
        }

        Debug.Log("�����Ԥ������ɣ�");
    }

    /// <summary>
    /// �������г�
    /// </summary>
    [ContextMenu("Clear All Pools")]
    public void ClearAllPools()
    {
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ClearAllPools();
            Debug.Log("���ж����������");
        }
    }

    /// <summary>
    /// ��ӡ��״̬
    /// </summary>
    [ContextMenu("Print Pool Stats")]
    public void PrintPoolStats()
    {
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.PrintAllPoolStats();
        }
    }
}