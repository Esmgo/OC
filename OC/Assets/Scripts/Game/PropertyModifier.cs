using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 一个通用的属性修饰容器，存储各种类型的属性修正值。
/// </summary>
[Serializable]
public class PropertyModifier
{
    private readonly Dictionary<ModifierType, float> _modifiers = new Dictionary<ModifierType, float>();

    /// <summary>
    /// 获取指定属性类型的修正值。
    /// </summary>
    /// <param name="type">要查询的属性类型。</param>
    /// <returns>如果存在则返回修正值，否则返回0。</returns>
    public float GetModifier(ModifierType type)
    {
        _modifiers.TryGetValue(type, out float value);
        return value;
    }

    /// <summary>
    /// 修改一个属性的修正值。
    /// </summary>
    /// <param name="type">要修改的属性类型。</param>
    /// <param name="value">要增加的值。</param>
    public void AddModifier(ModifierType type, float value)
    {
        if (_modifiers.ContainsKey(type))
        {
            // 如果字典中已存在该属性，累加其值
            _modifiers[type] += value;
        }
        else
        {
            // 如果是第一次为该属性添加修正，则直接将值添加到字典中
            _modifiers[type] = value;
        }
    }

    /// <summary>
    /// 清空所有修正值。
    /// </summary>
    public void Clear()
    {
        _modifiers.Clear();
    }
}

/// <summary>
/// 修改操作类型枚举
/// </summary>
public enum ModifierType
{
    MaxHealth_Add,          
    MaxHealth_Percent,    
    
    HealthRegen_Add,        
    HealthRegen_Percent,    

    MaxEnergy_Add,
    MaxEnergy_Percent,

    EnergyRegen_Add,
    EnergyRegen_Percent,

    CooldownReduction_Percent,

    MoveSpeed_Add,
    MoveSpeed_Percent,

    AttackIntervalReduction_Percent,

    PhysicalDamage_Add,
    PhysicalDamage_Percent,
    ManaDamage_Add,
    ManaDamage_Percent,
    ElementalDamage_Add,
    ElementalDamage_Percent,
}

/// <summary>
/// 属性修改效果的数据结构，用于在Inspector中配置。
/// </summary>
[Serializable]
public class ModifierEffect
{
    [Tooltip("属性类型")]
    public ModifierType ModifierType;

    [Tooltip("修改值")]
    public float value = 0f;
}

