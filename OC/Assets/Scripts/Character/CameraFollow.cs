using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随参数")]
    [SerializeField]private Transform target; // 跟随目标（角色）
    [SerializeField]private float followSpeed = 5f; // 跟随速度
    [SerializeField]private float maxOffsetDistance = 3f; // 角色到屏幕中心最大距离（世界单位）

    [Header("震动参数")]
    [SerializeField]private bool enableCameraShake = true; // 是否启用相机震动

    private Camera cam;
    private Tween shakeTween; // 震动动画引用

    void Awake()
    {
        // 从子物体获取相机组件
        cam = GetComponentInChildren<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (target == null)
        {
            var moveObj = FindObjectOfType<MoveBase>();
            if (moveObj != null)
                target = moveObj.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null || cam == null) return;

        // 计算目标位置
        Vector3 targetPosition = CalculateTargetPosition();
        
        // 使用Lerp平滑移动
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 计算目标位置
    /// </summary>
    private Vector3 CalculateTargetPosition()
    {
        // 获取鼠标在世界坐标的位置
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        // 计算角色与鼠标的中点
        Vector3 midPoint = (target.position + mouseWorldPos) / 2f;

        // 限制摄像头偏移，保证角色到摄像头中心距离不超过最大距离
        Vector3 camToTarget = target.position - midPoint;
        if (camToTarget.magnitude > maxOffsetDistance)
        {
            midPoint = target.position - camToTarget.normalized * maxOffsetDistance;
        }

        // 保持z轴不变
        midPoint.z = transform.position.z;

        return midPoint;
    }

    /// <summary>
    /// 触发相机震动（使用相对震动）
    /// </summary>
    /// <param name="duration">震动持续时间</param>
    /// <param name="strength">震动强度</param>
    /// <param name="vibrato">震动频率</param>
    /// <param name="randomness">随机性</param>
    public void TriggerCameraShake(float duration = 0.1f, float strength = 0.5f, int vibrato = 10, float randomness = 90f)
    {
        if (!enableCameraShake) return;

        // 停止当前的震动
        StopCameraShake();

        // 使用相对震动，这样不会影响跟随逻辑
        shakeTween = transform.DOShakePosition(duration, strength, vibrato, randomness, false, true)
            .SetRelative(true); // 关键：设置为相对震动
    }

    /// <summary>
    /// 停止相机震动
    /// </summary>
    public void StopCameraShake()
    {
        if (shakeTween != null && shakeTween.IsActive())
        {
            shakeTween.Kill();
        }
    }

    /// <summary>
    /// 检查是否正在震动
    /// </summary>
    public bool IsShaking()
    {
        return shakeTween != null && shakeTween.IsActive();
    }

    /// <summary>
    /// 强制设置相机位置
    /// </summary>
    public void SetCameraPosition(Vector3 position)
    {
        StopCameraShake();
        transform.position = position;
    }

    /// <summary>
    /// 设置跟随目标
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void OnDestroy()
    {
        StopCameraShake();
    }
}