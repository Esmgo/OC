using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 池统计信息
/// </summary>
[System.Serializable]
public struct PoolStats
{
    public int totalObjects;
    public int activeObjects;
    public int availableObjects;
}

/// <summary>
/// 单个对象池实现
/// </summary>
public class ObjectPool : MonoBehaviour
{
    [Header("池配置")]
    [SerializeField] private string poolName;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize;
    [SerializeField] private bool allowGrowth = true; // 是否允许池动态扩容

    [Header("运行时信息")]
    [SerializeField] private int totalCreated;
    [SerializeField] private int activeCount;

    private Queue<GameObject> availableObjects = new Queue<GameObject>();
    private HashSet<GameObject> allObjects = new HashSet<GameObject>();
    private HashSet<GameObject> activeObjects = new HashSet<GameObject>();

    /// <summary>
    /// 初始化对象池
    /// </summary>
    public void Initialize(string name, GameObject objPrefab, int size)
    {
        poolName = name;
        prefab = objPrefab;
        initialSize = size;
        
        CreateInitialObjects();
    }

    private void CreateInitialObjects()
    {
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewObject();
        }
    }

    private GameObject CreateNewObject()
    {
        if (prefab == null)
        {
            Debug.LogError($"对象池 {poolName} 的预制体为空");
            return null;
        }

        GameObject obj = Instantiate(prefab, transform);
        obj.name = $"{prefab.name}_{totalCreated}";
        obj.SetActive(false);

        // 添加池对象标识
        PooledObject pooledComponent = obj.GetComponent<PooledObject>();
        if (pooledComponent == null)
        {
            pooledComponent = obj.AddComponent<PooledObject>();
        }
        pooledComponent.Initialize(this, poolName);

        availableObjects.Enqueue(obj);
        allObjects.Add(obj);
        totalCreated++;

        return obj;
    }

    /// <summary>
    /// 从池中获取对象
    /// </summary>
    public GameObject GetObject(Vector3 position = default, Quaternion rotation = default)
    {
        GameObject obj = null;

        // 从可用队列获取对象
        if (availableObjects.Count > 0)
        {
            obj = availableObjects.Dequeue();
        }
        // 如果允许扩容且没有可用对象，创建新对象
        else if (allowGrowth)
        {
            obj = CreateNewObject();
            if (obj != null)
            {
                availableObjects.Dequeue(); // 移除刚创建的对象
            }
        }

        if (obj != null)
        {
            // 设置位置和旋转
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            
            // 激活对象
            obj.SetActive(true);
            activeObjects.Add(obj);
            activeCount = activeObjects.Count;

            // 重置对象状态
            ResetObject(obj);
        }

        return obj;
    }

    /// <summary>
    /// 将对象返回池中
    /// </summary>
    public void ReturnObject(GameObject obj)
    {
        if (obj == null) return;

        if (activeObjects.Contains(obj))
        {
            obj.SetActive(false);
            activeObjects.Remove(obj);
            availableObjects.Enqueue(obj);
            activeCount = activeObjects.Count;

            // 重置对象状态
            ResetObject(obj);
        }
        else
        {
            Debug.LogWarning($"尝试返回不属于池 {poolName} 的对象: {obj.name}");
        }
    }

    /// <summary>
    /// 重置对象状态
    /// </summary>
    private void ResetObject(GameObject obj)
    {
        // 调用对象的重置方法（如果存在）
        var resetInterfaces = obj.GetComponents<IPoolable>();
        foreach (var resetInterface in resetInterfaces)
        {
            if (obj.activeInHierarchy)
                resetInterface.OnGetFromPool();
            else
                resetInterface.OnReturnToPool();
        }
    }

    /// <summary>
    /// 获取池统计信息
    /// </summary>
    public PoolStats GetStats()
    {
        return new PoolStats
        {
            totalObjects = allObjects.Count,
            activeObjects = activeObjects.Count,
            availableObjects = availableObjects.Count
        };
    }

    /// <summary>
    /// 清空池
    /// </summary>
    public void Clear()
    {
        foreach (var obj in allObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        availableObjects.Clear();
        allObjects.Clear();
        activeObjects.Clear();
        totalCreated = 0;
        activeCount = 0;
    }

    /// <summary>
    /// 预热池（创建指定数量的对象）
    /// </summary>
    public void Prewarm(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CreateNewObject();
        }
    }
}