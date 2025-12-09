using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRotateComponent : MonoBehaviour
{
    [Tooltip("旋转中心")]
    [SerializeField] private Transform rotatePivot;
    [Tooltip("角度偏移")]
    [SerializeField] private float angleOffset = 0f;
    [Header("角色翻转检测")]
    [SerializeField] private Transform characterTransform;

    [Header("平滑设置")]
    public bool useSmoothing = true;
    public float smoothSpeed = 10f;

    private bool isFacingLeft => characterTransform != null && characterTransform.localScale.x < 0;
    private float currentAngle = 0f;
    private bool isRotatePaused = false;


    void Awake()
    {
        if (rotatePivot == null)
            rotatePivot = transform;

        if (characterTransform == null)
            characterTransform = transform.root; // 默认使用根物体作为角色Transform
    }

    void Update()
    {
        if (rotatePivot == null || isRotatePaused) return;

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
        transform.position = rotatePivot.position; // 保证武器始终围绕旋转中心
        if (isFacingLeft)
        {
            transform.rotation = Quaternion.Euler(0, 0, currentAngle + angleOffset - 180);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, currentAngle - angleOffset);
        }
    }

    public void PauseRotate(bool value)
    {
        isRotatePaused = value;
    }
}
