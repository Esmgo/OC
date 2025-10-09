using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// 对象池预加载器，在游戏开始时预创建常用对象池
/// </summary>
public class PoolPreloader : MonoBehaviour
{
    [Header("池配置")]
    [Tooltip("对象池配置列表")]
    public List<PoolConfig> poolConfigs = new List<PoolConfig>();
    
    [Header("Addressable标签配置")]
    [Tooltip("要加载的AA标签列表")]
    public List<string> addressableLabels = new List<string>();
    
    [Header("默认池大小")]
    [Tooltip("当AA资源没有指定大小时使用的默认池大小")]
    public int defaultPoolSize = 10;

    [Header("预加载设置")]
    public bool preloadOnStart = true;
    public bool preloadOnAwake = false;
    
    [Header("调试设置")]
    public bool showDebugLogs = true;

    private List<AsyncOperationHandle> assetHandles = new List<AsyncOperationHandle>();

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

        if (showDebugLogs)
            Debug.Log("开始预加载对象池...");

        StartCoroutine(PreloadPoolsCoroutine());
    }

    /// <summary>
    /// 预加载对象池的协程
    /// </summary>
    private System.Collections.IEnumerator PreloadPoolsCoroutine()
    {
        // 1. 预加载直接配置的池
        yield return StartCoroutine(PreloadConfiguredPoolsCoroutine());

        // 2. 预加载AA标签资源池
        yield return StartCoroutine(PreloadAddressablePoolsCoroutine());

        if (showDebugLogs)
            Debug.Log("对象池预加载完成！");
    }

    /// <summary>
    /// 预加载直接配置的池
    /// </summary>
    private System.Collections.IEnumerator PreloadConfiguredPoolsCoroutine()
    {
        foreach (var config in poolConfigs)
        {
            if (config.prefab != null && !string.IsNullOrEmpty(config.poolName))
            {
                try
                {
                    var pool = ObjectPoolManager.Instance.GetOrCreatePool(
                        config.poolName, 
                        config.prefab, 
                        config.size
                    );
                    
                    // 预热池
                    pool.Prewarm(config.size);
                    
                    if (showDebugLogs)
                        Debug.Log($"创建池: {config.poolName}, 大小: {config.size}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"创建池 {config.poolName} 失败: {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"跳过无效的池配置: {config.poolName}");
            }
            
            // 每个池创建后让出一帧，避免卡顿
            yield return null;
        }
    }

    /// <summary>
    /// 预加载AA标签资源池
    /// </summary>
    private System.Collections.IEnumerator PreloadAddressablePoolsCoroutine()
    {
        foreach (string label in addressableLabels)
        {
            if (string.IsNullOrEmpty(label)) continue;

            if (showDebugLogs)
                Debug.Log($"开始加载AA标签: {label}");

            // 创建加载任务但使用协程等待
            var loadTask = Tools.LoadAddressablesByLabel<GameObject>(label, true);

            // 等待任务完成
            while (!loadTask.IsCompleted)
            {
                yield return null;
            }
            var prefabs = loadTask.Result;

            if (prefabs != null && prefabs.Count > 0)
            {
                foreach (var prefab in prefabs)
                {
                    if (prefab != null)
                    {
                        string poolName = prefab.name;

                        if (IsPoolAlreadyConfigured(poolName))
                        {
                            if (showDebugLogs)
                                Debug.Log($"跳过已配置的池: {poolName}");
                            continue;
                        }

                        int poolSize = GetSuggestedPoolSize(prefab);

                        var pool = ObjectPoolManager.Instance.GetOrCreatePool(
                            poolName,
                            prefab,
                            poolSize
                        );

                        pool.Prewarm(poolSize);

                        if (showDebugLogs)
                            Debug.Log($"从AA标签 {label} 创建池: {poolName}, 大小: {poolSize}");
                    }
                    yield return null;
                    }
                }
            else
            {
            Debug.LogWarning($"AA标签 {label} 没有加载到任何资源");
            }
        yield return null;
        }
    }

    /// <summary>
    /// 检查池是否已经在配置中存在
    /// </summary>
    private bool IsPoolAlreadyConfigured(string poolName)
    {
        foreach (var config in poolConfigs)
        {
            if (config.poolName == poolName)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 从预制体获取建议的池大小
    /// </summary>
    private int GetSuggestedPoolSize(GameObject prefab)
    {
        // 根据预制体类型给出不同的默认大小
        if (prefab.GetComponent<BulletBase>() != null)
            return 20; // 子弹池较大
        else if (prefab.GetComponent<TextPopUp>() != null)
            return 15; // 文本弹出池中等
        else if (prefab.GetComponent<ParticleRecycler>() != null)
            return 10; // 特效池中等
        else if (prefab.GetComponent<CoinItem>() != null)
            return 25; // 物品池较大
        else if (prefab.GetComponent<EnemyBase>() != null)
            return 30; // 敌人池较大

        return defaultPoolSize;
    }

    /// <summary>
    /// 添加池配置
    /// </summary>
    public void AddPoolConfig(string poolName, GameObject prefab, int size)
    {
        poolConfigs.Add(new PoolConfig
        {
            poolName = poolName,
            prefab = prefab,
            size = size
        });
    }

    /// <summary>
    /// 添加AA标签
    /// </summary>
    public void AddAddressableLabel(string label)
    {
        if (!addressableLabels.Contains(label))
        {
            addressableLabels.Add(label);
        }
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

    /// <summary>
    /// 重新加载所有池
    /// </summary>
    [ContextMenu("Reload All Pools")]
    public void ReloadAllPools()
    {
        ClearAllPools();
        ReleaseAssetHandles();
        PreloadPools();
    }

    /// <summary>
    /// 释放AA资源句柄
    /// </summary>
    private void ReleaseAssetHandles()
    {
        // 使用Tools类的清理方法来释放所有缓存的Addressable资源
        Tools.ClearAddressableCache();
        
        // 清空本地句柄列表
        assetHandles.Clear();
    }

    void OnDestroy()
    {
        ReleaseAssetHandles();
    }

    /// <summary>
    /// 对象池配置结构体
    /// </summary>
    [Serializable]
    public struct PoolConfig
    {
        [Tooltip("池名称")]
        public string poolName;
        [Tooltip("预制体")]
        public GameObject prefab;
        [Tooltip("池大小")]
        public int size;

        public PoolConfig(string name, GameObject prefab, int size)
        {
            this.poolName = name;
            this.prefab = prefab;
            this.size = size;
        }
    }
}
