using UnityEngine;

/// <summary>
/// 池化对象组件，用于标识和管理池化对象
/// </summary>
public class PooledObject : MonoBehaviour
{
    [Header("池化信息")]
    [SerializeField] private string poolName;
    [SerializeField] private ObjectPool ownerPool;
    
    private bool isInitialized = false;

    /// <summary>
    /// 初始化池化对象
    /// </summary>
    /// <param name="pool">所属对象池</param>
    /// <param name="name">池名称</param>
    public void Initialize(ObjectPool pool, string name)
    {
        ownerPool = pool;
        poolName = name;
        isInitialized = true;
    }

    /// <summary>
    /// 返回到池中
    /// </summary>
    public void ReturnToPool()
    {
        if (isInitialized && ownerPool != null)
        {
            ownerPool.ReturnObject(gameObject);
        }
        else
        {
            Debug.LogWarning($"对象 {gameObject.name} 尝试返回未初始化的池");
        }
    }

    /// <summary>
    /// 自动返回到池中（延迟版本）
    /// </summary>
    /// <param name="delay">延迟时间</param>
    public void ReturnToPoolDelayed(float delay)
    {
        if (isInitialized)
        {
            Invoke(nameof(ReturnToPool), delay);
        }
    }

    /// <summary>
    /// 取消延迟返回
    /// </summary>
    public void CancelDelayedReturn()
    {
        CancelInvoke(nameof(ReturnToPool));
    }

    /// <summary>
    /// 获取池名称
    /// </summary>
    public string GetPoolName()
    {
        return poolName;
    }

    /// <summary>
    /// 获取所属对象池
    /// </summary>
    public ObjectPool GetOwnerPool()
    {
        return ownerPool;
    }

    void OnDisable()
    {
        // 取消所有延迟调用
        CancelInvoke();
    }
}