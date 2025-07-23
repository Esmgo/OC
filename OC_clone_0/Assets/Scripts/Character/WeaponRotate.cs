using UnityEngine;

/// <summary>
/// 挂载在武器上，让武器跟随鼠标旋转，并可限制旋转角度和设置旋转中心
/// </summary>
public class WeaponRotate : MonoBehaviour
{
    [Header("旋转参数")]
    public Transform rotationCenter; // 旋转中心（可拖拽角色Transform或自定义点）
    public float minAngle = -60f;    // 最小旋转角度（相对于初始朝向）
    public float maxAngle = 60f;     // 最大旋转角度（相对于初始朝向）
    
    [Header("角色翻转检测")]
    public Transform characterTransform; // 用于检测角色翻转的Transform（通常是角色根物体）
    
    private Camera cam;
    private bool isFacingLeft = false;

    void Awake()
    {
        cam = Camera.main;
        if (rotationCenter == null)
            rotationCenter = transform.parent; // 默认以父物体为旋转中心
        
        if (characterTransform == null)
            characterTransform = transform.root; // 默认使用根物体作为角色Transform
    }

    void Update()
    {
        if (cam == null || rotationCenter == null) return;

        // 检测角色是否朝左
        isFacingLeft = characterTransform != null && characterTransform.localScale.x < 0;
        
        // 获取鼠标在世界坐标的位置
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = rotationCenter.position.z;

        // 计算武器中心到鼠标的方向
        Vector2 dir = mouseWorldPos - rotationCenter.position;

        // 计算目标角度（以rotationCenter为原点，朝右为0度）
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        
        // 标准化角度到0-360范围
        float normalizedAngle = targetAngle;
        if (normalizedAngle < 0) normalizedAngle += 360f;
        
        // 角度映射
        float mappedAngle;
        
        if (isFacingLeft)
        {
            // 当角色朝左时，前半圆是90-270度
            if (normalizedAngle >= 90f && normalizedAngle <= 270f)
            {
                // 映射90->270到minAngle->maxAngle（反向，因为朝左时角度增加是顺时针）
                mappedAngle = Mathf.Lerp(-minAngle, -maxAngle, (normalizedAngle - 90f) / 180f);
            }
            else
            {
                // 超出前半圆范围，固定在边界
                if (normalizedAngle > 270f && normalizedAngle < 360f || normalizedAngle >= 0f && normalizedAngle < 90f)
                {
                    // 在270-90度范围（经过0度）
                    mappedAngle = (normalizedAngle > 270f) ? -minAngle : -maxAngle;
                }
                else
                {
                    // 安全默认值
                    mappedAngle = (normalizedAngle - 180f > 0) ? -minAngle : -maxAngle;
                }
            }
        }
        else
        {
            // 当角色朝右时，前半圆是-90到90度
            if (normalizedAngle >= 270f || normalizedAngle <= 90f)
            {
                // 需要将270-360和0-90度区间映射到minAngle->maxAngle
                float mappingAngle = normalizedAngle;
                if (mappingAngle >= 270f) mappingAngle -= 360f; // 转换到-90到90范围
                
                // 将-90->90映射到minAngle->maxAngle
                mappedAngle = Mathf.Lerp(minAngle, maxAngle, (mappingAngle + 90f) / 180f);
            }
            else
            {
                // 超出前半圆范围，固定在边界
                mappedAngle = (normalizedAngle < 180f) ? maxAngle : minAngle;
            }
        }

        // 应用旋转
        transform.position = rotationCenter.position; // 保证武器始终围绕旋转中心
        if (isFacingLeft)
        {
            transform.rotation = Quaternion.Euler(0, 0, -1*mappedAngle);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, mappedAngle);
        }
    }
}