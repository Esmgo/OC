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
    public PropertyModifier GlobalModifierForPlayer; // 测试用后面都换成属性
    public PropertyModifier GlobalModifierForEnemy;
    /// <summary>
    /// 当全局加成发生变化时触发的事件。
    /// Entity 类会订阅此事件以自动更新其属性。
    /// </summary>
    public event Action OnGlobalPlayerModifierChanged;
    public event Action OnGlobalEnemyModifierChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            GlobalModifierForPlayer = new PropertyModifier();
            GlobalModifierForEnemy = new PropertyModifier();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 应用一个全局属性效果。。
    /// </summary>
    /// <param name="effect">要应用的属性效果，包含类型、修改方式和值。</param>
    public void AddModifierForPlayer(ModifierPack effect)
    {
        if (effect == null) return;

        GlobalModifierForPlayer.AddModifier(effect.modifierType, effect.value);

        OnGlobalPlayerModifierChanged?.Invoke();

        Debug.Log($"全局加成已应用: {effect.modifierType} | {effect.value}");
    }

    public void AddModifierForEnemy(ModifierPack effect)
    {
        if (effect == null) return;

        GlobalModifierForEnemy.AddModifier(effect.modifierType, effect.value);

        OnGlobalEnemyModifierChanged?.Invoke();

        Debug.Log($"敌人全局加成已应用: {effect.modifierType} | {effect.value}");
    }

    /// <summary>
    /// 重置所有全局加成，用于开始新游戏。
    /// </summary>
    public void Reset()
    {
        GlobalModifierForPlayer = new PropertyModifier();
        OnGlobalPlayerModifierChanged?.Invoke();
        Debug.Log("所有全局加成已重置。");
    }
}
