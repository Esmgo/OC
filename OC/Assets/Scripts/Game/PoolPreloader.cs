using UnityEngine;

/// <summary>
/// 对象池预加载器，在游戏开始时预创建常用对象池
/// </summary>
public class PoolPreloader : MonoBehaviour
{
    [Header("预制体配置")]
    public GameObject bulletPrefab;
    public GameObject damageTextPrefab;
    public GameObject hitEffectPrefab;
    public GameObject enemyPrefab;

    [Header("池大小配置")]
    public int bulletPoolSize = 50;
    public int damageTextPoolSize = 20;
    public int hitEffectPoolSize = 30;
    public int enemyPoolSize = 15;

    [Header("预加载设置")]
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
            Debug.LogError("ObjectPoolManager.Instance 为空，无法预加载对象池");
            return;
        }

        Debug.Log("开始预加载对象池...");

        // 预加载子弹池
        if (bulletPrefab != null)
        {
            var bulletPool = ObjectPoolManager.Instance.GetOrCreatePool("Bullet", bulletPrefab, bulletPoolSize);
            bulletPool.Prewarm(bulletPoolSize);
            Debug.Log($"子弹池预加载完成: {bulletPoolSize} 个对象");
        }

        // 预加载伤害文本池
        if (damageTextPrefab != null)
        {
            var damagePool = ObjectPoolManager.Instance.GetOrCreatePool("DamageText", damageTextPrefab, damageTextPoolSize);
            damagePool.Prewarm(damageTextPoolSize);
            Debug.Log($"伤害文本池预加载完成: {damageTextPoolSize} 个对象");
        }

        // 预加载击中效果池
        if (hitEffectPrefab != null)
        {
            var hitEffectPool = ObjectPoolManager.Instance.GetOrCreatePool("HitEffect", hitEffectPrefab, hitEffectPoolSize);
            hitEffectPool.Prewarm(hitEffectPoolSize);
            Debug.Log($"击中效果池预加载完成: {hitEffectPoolSize} 个对象");
        }

        // 预加载敌人池
        if (enemyPrefab != null)
        {
            var enemyPool = ObjectPoolManager.Instance.GetOrCreatePool("Enemy", enemyPrefab, enemyPoolSize);
            enemyPool.Prewarm(enemyPoolSize);
            Debug.Log($"敌人池预加载完成: {enemyPoolSize} 个对象");
        }

        Debug.Log("对象池预加载完成！");
    }

    /// <summary>
    /// 清理所有池
    /// </summary>
    [ContextMenu("Clear All Pools")]
    public void ClearAllPools()
    {
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ClearAllPools();
            Debug.Log("所有对象池已清理");
        }
    }

    /// <summary>
    /// 打印池状态
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