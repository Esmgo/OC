using UnityEngine;
using DG.Tweening;

/// <summary>
/// 相机震动脚本，挂载在相机子物体上
/// </summary>
public class CameraShake : MonoBehaviour
{
    [Header("震动参数")]
    public bool enableCameraShake = true; // 是否启用相机震动

    private Tween shakeTween; // 震动动画引用

    void Awake()
    {
        // 确保相机在本地原点
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    /// <summary>
    /// 触发相机震动（本地坐标震动）
    /// </summary>
    /// <param name="duration">震动持续时间</param>
    /// <param name="strength">震动强度</param>
    /// <param name="vibrato">震动频率</param>
    /// <param name="randomness">随机性</param>
    public void TriggerShake(float duration = 0.1f, float strength = 0.5f, int vibrato = 10, float randomness = 90f)
    {
        if (!enableCameraShake) return;

        // 停止当前的震动
        StopShake();

        // 确保从本地原点开始震动
        transform.localPosition = Vector2.zero;

        // 对相机进行本地坐标震动
        shakeTween = transform.DOShakePosition(duration, strength, vibrato, randomness, false, true);
        
    }

    /// <summary>
    /// 停止相机震动
    /// </summary>
    public void StopShake()
    {
        if (shakeTween != null && shakeTween.IsActive())
        {
            shakeTween.Kill();
        }

        // 重置相机本地位置
        transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// 检查是否正在震动
    /// </summary>
    public bool IsShaking()
    {
        return shakeTween != null && shakeTween.IsActive();
    }

    void OnDestroy()
    {
        StopShake();
    }

    void OnDisable()
    {
        StopShake();
    }
}