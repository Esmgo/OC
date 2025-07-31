using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ��ͳ����Ϣ
/// </summary>
[System.Serializable]
public struct PoolStats
{
    public int totalObjects;
    public int activeObjects;
    public int availableObjects;
}

/// <summary>
/// ���������ʵ��
/// </summary>
public class ObjectPool : MonoBehaviour
{
    [Header("������")]
    [SerializeField] private string poolName;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize;
    [SerializeField] private bool allowGrowth = true; // �Ƿ�����ض�̬����

    [Header("����ʱ��Ϣ")]
    [SerializeField] private int totalCreated;
    [SerializeField] private int activeCount;

    private Queue<GameObject> availableObjects = new Queue<GameObject>();
    private HashSet<GameObject> allObjects = new HashSet<GameObject>();
    private HashSet<GameObject> activeObjects = new HashSet<GameObject>();

    /// <summary>
    /// ��ʼ�������
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
            Debug.LogError($"����� {poolName} ��Ԥ����Ϊ��");
            return null;
        }

        GameObject obj = Instantiate(prefab, transform);
        obj.name = $"{prefab.name}_{totalCreated}";
        obj.SetActive(false);

        // ��ӳض����ʶ
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
    /// �ӳ��л�ȡ����
    /// </summary>
    public GameObject GetObject(Vector3 position = default, Quaternion rotation = default)
    {
        GameObject obj = null;

        // �ӿ��ö��л�ȡ����
        if (availableObjects.Count > 0)
        {
            obj = availableObjects.Dequeue();
        }
        // �������������û�п��ö��󣬴����¶���
        else if (allowGrowth)
        {
            obj = CreateNewObject();
            if (obj != null)
            {
                availableObjects.Dequeue(); // �Ƴ��մ����Ķ���
            }
        }

        if (obj != null)
        {
            // ����λ�ú���ת
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            
            // �������
            obj.SetActive(true);
            activeObjects.Add(obj);
            activeCount = activeObjects.Count;

            // ���ö���״̬
            ResetObject(obj);
        }

        return obj;
    }

    /// <summary>
    /// �����󷵻س���
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

            // ���ö���״̬
            ResetObject(obj);
        }
        else
        {
            Debug.LogWarning($"���Է��ز����ڳ� {poolName} �Ķ���: {obj.name}");
        }
    }

    /// <summary>
    /// ���ö���״̬
    /// </summary>
    private void ResetObject(GameObject obj)
    {
        // ���ö�������÷�����������ڣ�
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
    /// ��ȡ��ͳ����Ϣ
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
    /// ��ճ�
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
    /// Ԥ�ȳأ�����ָ�������Ķ���
    /// </summary>
    public void Prewarm(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CreateNewObject();
        }
    }
}