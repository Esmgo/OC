using UnityEngine;
using DG.Tweening;

/// <summary>
/// 挂到相机的父物体上用来移动
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("跟随参数")]
    [SerializeField]private Transform target; // 跟随目标（角色）
    [SerializeField]private float followSpeed = 5f; // 跟随速度
    [SerializeField]private float maxOffsetDistance = 3f; // 角色到屏幕中心最大距离（世界单位）

    private Camera cam;
    void Awake()
    {
        // 从子物体获取相机组件
        cam = GetComponentInChildren<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
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
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        Vector3 aimPoint = (target.position + mouseWorldPos) / 2f;

        Vector3 camToTarget = target.position - aimPoint;
        if (camToTarget.magnitude > maxOffsetDistance)
        {
            aimPoint = target.position - camToTarget.normalized * maxOffsetDistance;
        }

        // 保持z轴不变
        aimPoint.z = transform.position.z;

        return aimPoint;
    }
    
    /// <summary>
    /// 设置跟随目标
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}