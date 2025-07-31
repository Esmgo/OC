using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ͨ�ö���ع����������Զ�̬�����͹���������͵Ķ����
/// </summary>
public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [Header("���������")]
    public bool createPoolContainers = true; // �Ƿ�Ϊÿ���ش�������
    public int defaultPoolSize = 20; // Ĭ�ϳش�С

    // �洢���ж����
    private Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();
    
    // ����������ĸ�����
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
        // �����������ĸ�����
        GameObject container = new GameObject("Object Pools");
        container.transform.SetParent(transform);
        poolsContainer = container.transform;
    }

    /// <summary>
    /// ��ȡ�򴴽�ָ�����͵Ķ����
    /// </summary>
    /// <param name="poolName">������</param>
    /// <param name="prefab">Ԥ����</param>
    /// <param name="initialSize">��ʼ��С</param>
    /// <returns>�����ʵ��</returns>
    public ObjectPool GetOrCreatePool(string poolName, GameObject prefab, int initialSize = -1)
    {
        if (pools.TryGetValue(poolName, out ObjectPool existingPool))
        {
            return existingPool;
        }

        // �����µĶ����
        int size = initialSize > 0 ? initialSize : defaultPoolSize;
        ObjectPool newPool = CreatePool(poolName, prefab, size);
        pools[poolName] = newPool;
        
        Debug.Log($"���������: {poolName}, ��С: {size}");
        return newPool;
    }

    /// <summary>
    /// �����µĶ����
    /// </summary>
    private ObjectPool CreatePool(string poolName, GameObject prefab, int size)
    {
        // Ϊÿ���ش�������������
        GameObject poolContainer = new GameObject($"Pool_{poolName}");
        poolContainer.transform.SetParent(poolsContainer);

        ObjectPool pool = poolContainer.AddComponent<ObjectPool>();
        pool.Initialize(poolName, prefab, size);
        
        return pool;
    }

    /// <summary>
    /// ��ָ���ػ�ȡ����
    /// </summary>
    public GameObject GetObject(string poolName, Vector3 position = default, Quaternion rotation = default)
    {
        if (pools.TryGetValue(poolName, out ObjectPool pool))
        {
            return pool.GetObject(position, rotation);
        }

        Debug.LogWarning($"����� {poolName} ������");
        return null;
    }

    /// <summary>
    /// ��ָ���ػ�ȡ���󣨷��Ͱ汾��
    /// </summary>
    public T GetObject<T>(string poolName, Vector3 position = default, Quaternion rotation = default) where T : Component
    {
        GameObject obj = GetObject(poolName, position, rotation);
        return obj?.GetComponent<T>();
    }

    /// <summary>
    /// �����󷵻ص�����
    /// </summary>
    public void ReturnObject(string poolName, GameObject obj)
    {
        if (pools.TryGetValue(poolName, out ObjectPool pool))
        {
            pool.ReturnObject(obj);
        }
        else
        {
            Debug.LogWarning($"�޷��ҵ������ {poolName} �����ض���");
        }
    }

    /// <summary>
    /// ��������Ƿ����
    /// </summary>
    public bool HasPool(string poolName)
    {
        return pools.ContainsKey(poolName);
    }

    /// <summary>
    /// ��ȡ�ص�ͳ����Ϣ
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
    /// ���ָ����
    /// </summary>
    public void ClearPool(string poolName)
    {
        if (pools.TryGetValue(poolName, out ObjectPool pool))
        {
            pool.Clear();
        }
    }

    /// <summary>
    /// ������г�
    /// </summary>
    public void ClearAllPools()
    {
        foreach (var pool in pools.Values)
        {
            pool.Clear();
        }
    }

    /// <summary>
    /// �Ƴ�ָ����
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

    void OnDestroy()
    {
        ClearAllPools();
        pools.Clear();
    }

    #region Debug����
    
    /// <summary>
    /// ��ӡ���гص�״̬
    /// </summary>
    [ContextMenu("Print All Pool Stats")]
    public void PrintAllPoolStats()
    {
        Debug.Log("=== �����״̬ ===");
        foreach (var kvp in pools)
        {
            var stats = kvp.Value.GetStats();
            Debug.Log($"��: {kvp.Key} - ����: {stats.totalObjects}, ��Ծ: {stats.activeObjects}, ����: {stats.availableObjects}");
        }
    }
    
    #endregion
}