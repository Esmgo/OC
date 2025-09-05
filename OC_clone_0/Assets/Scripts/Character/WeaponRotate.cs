using UnityEngine;

/// <summary>
/// 挂载在武器上，让武器跟随鼠标旋转，并可限制旋转角度和设置旋转中心
/// </summary>
public class WeaponRotate : MonoBehaviour
{
    [Header("旋转参数")]
    public Transform rotationCenter; // 旋转中心（可拖拽角色Transform或自定义点）
    public float fixAngle = 0f;      // 偏移角度修正
    
    [Header("角色翻转检测")]
    public Transform characterTransform; // 用于检测角色翻转的Transform（通常是角色根物体）
    
    [Header("平滑设置")]
    public bool useSmoothing = true;
    public float smoothSpeed = 10f;
    
    private bool isFacingLeft => characterTransform != null && characterTransform.localScale.x < 0;
    private float currentAngle = 0f; // 当前映射角度，用于平滑过渡

    void Awake()
    {
        if (rotationCenter == null)
            rotationCenter = transform;
        
        if (characterTransform == null)
            characterTransform = transform.root; // 默认使用根物体作为角色Transform
    }

    void Update()
    {
        if (rotationCenter == null) return;

        float targetAngle = Tools.GetMouseAngle(transform);

        //平滑过渡到目标角度
        if (useSmoothing)
        {
            currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, smoothSpeed * Time.deltaTime);
        }
        else
        {
            currentAngle = targetAngle;
        }

        // 应用旋转
        transform.position = rotationCenter.position; // 保证武器始终围绕旋转中心
        if (isFacingLeft)
        {
            transform.rotation = Quaternion.Euler(0, 0, currentAngle + fixAngle - 180);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, currentAngle - fixAngle);
        }
    }
}