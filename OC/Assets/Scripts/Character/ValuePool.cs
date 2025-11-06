using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <summary>
/// 管理一个动态资源池（如生命、能量）。
/// </summary>
public class ValuePool
{
    [Tooltip("数值变化时调用，参数（当前值， 最大值）")]
    public Action<float, float> OnValueChanged;

    [Tooltip("耗尽时调用")]
    public Action OnDepleted;

    public float currentValue { get; private set; }
    public float maxValue { get; private set; }

    public ValuePool(float maxValue)
    {
        maxValue = Mathf.Max(0, maxValue);
        currentValue = maxValue;
    }

    public void SetMaxValue(float value, bool resetToFull)
    {
        maxValue = Mathf.Max(0, value);
        if (resetToFull)
        {
            currentValue = maxValue;
        }
        else
        {
            currentValue = Mathf.Min(currentValue, maxValue);
        }
    }

    /// <summary>
    /// 修改资源池的当前值。
    /// 正数表示增加（治疗/恢复），负数表示减少（伤害/消耗）。
    /// </summary>
    /// <param name="value">要修改的量</param>
    public void Value(float value)
    {
        if (value == 0) return;

        currentValue += value;
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
        OnValueChanged?.Invoke(currentValue, maxValue);

        if (value < 0 && currentValue <= 0)
        {
            OnDepleted?.Invoke();
        }
    }
}
