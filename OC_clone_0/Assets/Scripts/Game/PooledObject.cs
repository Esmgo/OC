using UnityEngine;

/// <summary>
/// �ػ�������������ڱ�ʶ�͹���ػ�����
/// </summary>
public class PooledObject : MonoBehaviour
{
    [Header("�ػ���Ϣ")]
    [SerializeField] private string poolName;
    [SerializeField] private ObjectPool ownerPool;
    
    private bool isInitialized = false;

    /// <summary>
    /// ��ʼ���ػ�����
    /// </summary>
    /// <param name="pool">���������</param>
    /// <param name="name">������</param>
    public void Initialize(ObjectPool pool, string name)
    {
        ownerPool = pool;
        poolName = name;
        isInitialized = true;
    }

    /// <summary>
    /// ���ص�����
    /// </summary>
    public void ReturnToPool()
    {
        if (isInitialized && ownerPool != null)
        {
            ownerPool.ReturnObject(gameObject);
        }
        else
        {
            Debug.LogWarning($"���� {gameObject.name} ���Է���δ��ʼ���ĳ�");
        }
    }

    /// <summary>
    /// �Զ����ص����У��ӳٰ汾��
    /// </summary>
    /// <param name="delay">�ӳ�ʱ��</param>
    public void ReturnToPoolDelayed(float delay)
    {
        if (isInitialized)
        {
            Invoke(nameof(ReturnToPool), delay);
        }
    }

    /// <summary>
    /// ȡ���ӳٷ���
    /// </summary>
    public void CancelDelayedReturn()
    {
        CancelInvoke(nameof(ReturnToPool));
    }

    /// <summary>
    /// ��ȡ������
    /// </summary>
    public string GetPoolName()
    {
        return poolName;
    }

    /// <summary>
    /// ��ȡ���������
    /// </summary>
    public ObjectPool GetOwnerPool()
    {
        return ownerPool;
    }

    void OnDisable()
    {
        // ȡ�������ӳٵ���
        CancelInvoke();
    }
}