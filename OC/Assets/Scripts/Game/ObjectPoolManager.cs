using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 通用对象池管理器，可以动态创建和管理多种类型的对象池
/// </summary>
public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [Header("对象池配置")]
    public bool createPoolContainers = true; // 是否为每个池创建容器
    public int defaultPoolSize = 20; // 默认池大小

    // 存储所有对象池
    private Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();
    
    // 对象池容器的父物体
    private Transform poolsContainer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeManager()
    {
        // 创建池容器的父物体
        GameObject container = new GameObject("Object Pools");
        container.transform.SetParent(transform);
        poolsContainer = container.transform;
    }

    /// <summary>
    /// 获取或创建指定类型的对象池
    /// </summary>
    /// <param name="poolName">池名称</param>
    /// <param name="prefab">预制体</param>
    /// <param name="initialSize">初始大小</param>
    /// <returns>对象池实例</returns>
    public ObjectPool GetOrCreatePool(string poolName, GameObject prefab, int initialSize = -1)
    {
        if (pools.TryGetValue(poolName, out ObjectPool existingPool))
        {
            return existingPool;
        }

        // 创建新的对象池
        int size = initialSize > 0 ? initialSize : defaultPoolSize;
        ObjectPool newPool = CreatePool(poolName, prefab, size);
        pools[poolName] = newPool;
        
        Debug.Log($"创建对象池: {poolName}, 大小: {size}");
        return newPool;
    }

    /// <summary>
    /// 创建新的对象池
    /// </summary>
    private ObjectPool CreatePool(string poolName, GameObject prefab, int size)
    {
        // 为每个池创建独立的容器
        GameObject poolContainer = new GameObject($"Pool_{poolName}");
        poolContainer.transform.SetParent(poolsContainer);

        ObjectPool pool = poolContainer.AddComponent<ObjectPool>();
        pool.Initialize(poolName, prefab, size);
        
        return pool;
    }

    /// <summary>
    /// 从指定池获取对象
    /// </summary>
    public GameObject GetObject(string poolName, Vector3 position = default, Quaternion rotation = default)
    {
        if (pools.TryGetValue(poolName, out ObjectPool pool))
        {
            return pool.GetObject(position, rotation);
        }

        Debug.LogWarning($"对象池 {poolName} 不存在");
        return null;
    }

    /// <summary>
    /// 从指定池获取对象（泛型版本）
    /// </summary>
    public T GetObject<T>(string poolName, Vector3 position = default, Quaternion rotation = default) where T : Component
    {
        GameObject obj = GetObject(poolName, position, rotation);
        return obj?.GetComponent<T>();
    }

    /// <summary>
    /// 将对象返回到池中
    /// </summary>
    public void ReturnObject(string poolName, GameObject obj)
    {
        if (pools.TryGetValue(poolName, out ObjectPool pool))
        {
            pool.ReturnObject(obj);
        }
        else
        {
            Debug.LogWarning($"无法找到对象池 {poolName} 来返回对象");
        }
    }

    /// <summary>
    /// 检查对象池是否存在
    /// </summary>
    public bool HasPool(string poolName)
    {
        return pools.ContainsKey(poolName);
    }

    /// <summary>
    /// 获取池的统计信息
    /// </summary>
    public PoolStats GetPoolStats(string poolName)
    {
        if (pools.TryGetValue(poolName, out ObjectPool pool))
        {
            return pool.GetStats();
        }
        return new PoolStats();
    }

    /// <summary>
    /// 清空指定池
    /// </summary>
    public void ClearPool(string poolName)
    {
        if (pools.TryGetValue(poolName, out ObjectPool pool))
        {
            pool.Clear();
        }
    }

    /// <summary>
    /// 清空所有池
    /// </summary>
    public void ClearAllPools()
    {
        foreach (var pool in pools.Values)
        {
            pool.Clear();
        }
    }

    /// <summary>
    /// 移除指定池
    /// </summary>
    public void RemovePool(string poolName)
    {
        if (pools.TryGetValue(poolName, out ObjectPool pool))
        {
            pool.Clear();
            Destroy(pool.gameObject);
            pools.Remove(poolName);
        }
    }

    /// <summary>
    /// 回收指定池的所有活跃对象
    /// </summary>
    /// <param name="poolName">池名称</param>
    public void RecycleAllActiveObjects(string poolName)
    {
        if (pools.TryGetValue(poolName, out ObjectPool pool))
        {
            pool.RecycleAllActiveObjects();
        }
        else
        {
            Debug.LogWarning($"对象池 {poolName} 不存在，无法回收活跃对象");
        }
    }

    /// <summary>
    /// 强制回收指定池的所有活跃对象
    /// </summary>
    /// <param name="poolName">池名称</param>
    public void ForceRecycleAllActiveObjects(string poolName)
    {
        if (pools.TryGetValue(poolName, out ObjectPool pool))
        {
            pool.ForceRecycleAllActiveObjects();
        }
        else
        {
            Debug.LogWarning($"对象池 {poolName} 不存在，无法强制回收活跃对象");
        }
    }

    /// <summary>
    /// 回收所有池的所有活跃对象
    /// </summary>
    public void RecycleAllActiveObjectsInAllPools()
    {
        Debug.Log("开始回收所有池的活跃对象");
        
        foreach (var kvp in pools)
        {
            kvp.Value.RecycleAllActiveObjects();
        }
        
        Debug.Log("所有池的活跃对象回收完成");
    }

    /// <summary>
    /// 强制回收所有池的所有活跃对象
    /// </summary>
    public void ForceRecycleAllActiveObjectsInAllPools()
    {
        Debug.Log("开始强制回收所有池的活跃对象");
        
        foreach (var kvp in pools)
        {
            kvp.Value.ForceRecycleAllActiveObjects();
        }
        
        Debug.Log("所有池的活跃对象强制回收完成");
    }

    /// <summary>
    /// 获取指定池的所有活跃对象
    /// </summary>
    /// <param name="poolName">池名称</param>
    /// <returns>活跃对象集合</returns>
    public IReadOnlyCollection<GameObject> GetActiveObjects(string poolName)
    {
        if (pools.TryGetValue(poolName, out ObjectPool pool))
        {
            return pool.GetActiveObjects();
        }
        
        return new List<GameObject>();
    }

    /// <summary>
    /// 获取所有池的活跃对象总数
    /// </summary>
    /// <returns>总活跃对象数</returns>
    public int GetTotalActiveObjectsCount()
    {
        int total = 0;
        foreach (var pool in pools.Values)
        {
            total += pool.GetStats().activeObjects;
        }
        return total;
    }

    void OnDestroy()
    {
        ClearAllPools();
        pools.Clear();
    }

    #region Debug方法
    
    /// <summary>
    /// 打印所有池的状态
    /// </summary>
    [ContextMenu("Print All Pool Stats")]
    public void PrintAllPoolStats()
    {
        Debug.Log("=== 对象池状态 ===");
        foreach (var kvp in pools)
        {
            var stats = kvp.Value.GetStats();
            Debug.Log($"池: {kvp.Key} - 总数: {stats.totalObjects}, 活跃: {stats.activeObjects}, 可用: {stats.availableObjects}");
        }
    }
    
    #endregion
}