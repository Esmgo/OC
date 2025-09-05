using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class ParticleRecycler : MonoBehaviour, IPoolable
{
    [Header("回收设置")]
    public float recycleDelay = 3f; // 总回收时间

    [Header("VFX设置")]
    [Tooltip("自动播放VFX")]
    public bool autoPlayVFX = true;
    [Tooltip("VFX播放事件名称")]
    public string vfxPlayEventName = "OnPlay";
    [Tooltip("VFX停止事件名称")]
    public string vfxStopEventName = "OnStop";

    private VisualEffect[] allVFXs; // VFX组件数组
    private PooledObject pooledObject; // 对象池组件引用

    public void OnGetFromPool()
    {
        // 重置状态（如果需要的话）
    }

    public void OnReturnToPool()
    {
        // 重置状态（如果需要的话）
    }

    private void OnEnable()
    {
        // 获取所有VFX组件
        allVFXs = GetComponentsInChildren<VisualEffect>();
        pooledObject = GetComponent<PooledObject>();

        // 播放VFX效果
        if (autoPlayVFX)
        {
            PlayAllVFX();
        }

        // 开始回收倒计时
        StartCoroutine(RecycleAfterDelay());
    }

    /// <summary>
    /// 播放所有VFX
    /// </summary>
    private void PlayAllVFX()
    {
        if (allVFXs != null)
        {
            foreach (var vfx in allVFXs)
            {
                if (vfx != null)
                {
                    // 重置并播放VFX
                    vfx.Reinit();
                    vfx.Play();

                    // 如果有指定的播放事件，发送事件
                    if (!string.IsNullOrEmpty(vfxPlayEventName))
                    {
                        vfx.SendEvent(vfxPlayEventName);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 停止所有VFX
    /// </summary>
    private void StopAllVFX()
    {
        if (allVFXs != null)
        {
            foreach (var vfx in allVFXs)
            {
                if (vfx != null)
                {
                    // 如果有指定的停止事件，发送事件
                    if (!string.IsNullOrEmpty(vfxStopEventName))
                    {
                        vfx.SendEvent(vfxStopEventName);
                    }

                    // 停止VFX
                    vfx.Stop();
                }
            }
        }
    }

    /// <summary>
    /// 延迟回收到对象池
    /// </summary>
    private IEnumerator RecycleAfterDelay()
    {
        yield return new WaitForSeconds(recycleDelay);
        RecycleToPool();
    }

    /// <summary>
    /// 立即回收到对象池
    /// </summary>
    public void RecycleToPool()
    {
        // 停止所有VFX
        StopAllVFX();

        // 返回到对象池
        if (pooledObject != null)
        {
            pooledObject.ReturnToPool();
        }
        else
        {
            Debug.LogWarning($"ParticleRecycler: {gameObject.name} 没有 PooledObject 组件，直接销毁");
            Destroy(gameObject);
        }
    }

    void OnDisable()
    {
        // 当对象被禁用时取消所有协程
        StopAllCoroutines();
    }
}