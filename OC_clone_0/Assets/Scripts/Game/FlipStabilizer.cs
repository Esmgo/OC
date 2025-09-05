using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipStabilizer : MonoBehaviour
{
    private Vector3 _initialScale;

    void Start()
    {
        _initialScale = transform.localScale;
    }

    void LateUpdate()
    {
        // 获取父物体的当前缩放
        Vector3 parentScale = transform.parent.lossyScale;

        // 计算抵消缩放
        transform.localScale = new Vector3(
            _initialScale.x / parentScale.x,
            _initialScale.y / parentScale.y,
            _initialScale.z / parentScale.z
        );
    }
}
