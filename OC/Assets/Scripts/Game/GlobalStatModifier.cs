using GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理所有全局属性加成（例如，通过商店购买的永久升级）。
/// </summary>
public class GlobalStatModifier : MonoBehaviour
{
    public static GlobalStatModifier Instance { get; private set; }

    /// <summary>
    /// 存储所有全局加成数值的容器。
    /// </summary>
    //public PropertyModifier GlobalModifierForPlayer { get; private set; }
    public PropertyModifier GlobalModifierForPlayer; // 测试用
    /// <summary>
    /// 当全局加成发生变化时触发的事件。
    /// Entity 类会订阅此事件以自动更新其属性。
    /// </summary>
    public event Action OnGlobalBonusesChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            GlobalModifierForPlayer = new PropertyModifier();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 应用一个全局属性效果。
    /// 这是从外部（如商店UI）调用以添加永久升级的唯一入口。
    /// </summary>
    /// <param name="mod">要应用的属性效果，包含类型、修改方式和值。</param>
    public void AddModifier(ModifierEffect mod)
    {
        if (mod == null) return;

        // 使用 PropertyModifier 内部的方法来更新加成值
        GlobalModifierForPlayer.AddModifier(mod.ModifierType, mod.value);

        // 关键步骤：触发事件，通知所有订阅者（即所有Entity）
        // “？”表示如果没有任何对象订阅此事件，则不执行Invoke，避免空引用异常。
        OnGlobalBonusesChanged?.Invoke();

        Debug.Log($"全局加成已应用: {mod.ModifierType} | {mod.value}");
    }

    /// <summary>
    /// （可选）重置所有全局加成，用于开始新游戏。
    /// </summary>
    public void ResetGlobalBonuses()
    {
        GlobalModifierForPlayer = new PropertyModifier();
        OnGlobalBonusesChanged?.Invoke();
        Debug.Log("所有全局加成已重置。");
    }
}
