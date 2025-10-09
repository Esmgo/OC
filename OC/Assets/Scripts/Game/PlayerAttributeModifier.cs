using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 属性修饰符类，用于存储各种属性的修正值
/// </summary>
[Serializable]
public class PlayerAttributeModifier 
{
    [Header("生命相关")]
    [Tooltip("最大生命修正(加)")]
    public float maxHealthModifier = 0;
    [Tooltip("最大生命修正(百分比)")]
    public float maxHealthModifierPercent = 0;

    [Tooltip("生命回复修正")]
    public float healthRegenModifier = 0;

    [Header("能量相关")]
    [Tooltip("最大能量修正(加)")]
    public float maxEnergyModifier = 0;
    [Tooltip("最大能量修正(百分比)")]
    public float maxEnergyModifierPercent = 0;
    [Tooltip("能量回复修正")]
    public float energyRegenModifier = 0;

    [Header("移动相关")]
    [Tooltip("移动速度修正(加)")]
    public float moveSpeedModifier = 0;
    [Tooltip("移动速度修正(百分比)")]
    public float moveSpeedModifierPercent = 0;
    [Tooltip("冲刺速度修正")]
    public float dashSpeedModifier = 0;
    [Tooltip("冲刺冷却修正")]
    public float dashCooldownModifier = 0;

    [Header("攻击相关")]
    [Tooltip("攻击间隔修正")]
    public float attackIntervalModifier = 0;
    [Tooltip("攻击间隔修正（百分比）")]
    public float attackIntervalModifierPercent = 0;

    [Tooltip("伤害倍率修正")]
    public float damagePercentModifier = 0;
    [Tooltip("物理伤害修正")]
    public float physicalDamageModifier = 0;
    [Tooltip("物理伤害修正(百分比)")]
    public float physicalDamageModifierPercent = 0;
    [Tooltip("异能伤害修正")]
    public float energyDamageModifier = 0;
    [Tooltip("异能伤害修正(百分比)")]
    public float energyDamageModifierPercent = 0;
    [Tooltip("攻击范围修正")]
    public float attackRangeModifier = 0;
    [Tooltip("击退力修正")]
    public float knockbackForceModifier = 0;

    [Header("防御相关")]
    [Tooltip("护盾修正")]
    public float shieldModifier = 0;
    [Tooltip("理智修正")]
    public float sanityModifier = 0;

    // 属性修改器委托字典
    private static readonly Dictionary<(AttributeType, ModificationType), Action<PlayerAttributeModifier, float>> ModifierActions 
        = new()
        {
        // MaxHealth
        { (AttributeType.maxHealthModifier, ModificationType.Add), (m, v) => m.maxHealthModifier += (int)v },
        { (AttributeType.maxHealthModifier, ModificationType.Multiply), (m, v) => m.maxHealthModifier = (int)(m.maxHealthModifier * v) },
        { (AttributeType.maxHealthModifier, ModificationType.Override), (m, v) => m.maxHealthModifier = (int)v },

        // MaxHealthPercent
        { (AttributeType.maxHealthModifierPercent, ModificationType.Add), (m, v) => m.maxHealthModifierPercent += v },
        { (AttributeType.maxHealthModifierPercent, ModificationType.Multiply), (m, v) => m.maxHealthModifierPercent *= v },
        { (AttributeType.maxHealthModifierPercent, ModificationType.Override), (m, v) => m.maxHealthModifierPercent = v },

        // HealthRegen
        { (AttributeType.healthRegenModifier, ModificationType.Add), (m, v) => m.healthRegenModifier += v },
        { (AttributeType.healthRegenModifier, ModificationType.Multiply), (m, v) => m.healthRegenModifier *= v },
        { (AttributeType.healthRegenModifier, ModificationType.Override), (m, v) => m.healthRegenModifier = v },

        // MaxEnergy
        { (AttributeType.maxEnergyModifier, ModificationType.Add), (m, v) => m.maxEnergyModifier += (int)v },
        { (AttributeType.maxEnergyModifier, ModificationType.Multiply), (m, v) => m.maxEnergyModifier = (int)(m.maxEnergyModifier * v) },
        { (AttributeType.maxEnergyModifier, ModificationType.Override), (m, v) => m.maxEnergyModifier = (int)v },

        // MaxEnergyPercent
        { (AttributeType.maxEnergyModifierPercent, ModificationType.Add), (m, v) => m.maxEnergyModifierPercent += v },
        { (AttributeType.maxEnergyModifierPercent, ModificationType.Multiply), (m, v) => m.maxEnergyModifierPercent *= v },
        { (AttributeType.maxEnergyModifierPercent, ModificationType.Override), (m, v) => m.maxEnergyModifierPercent = v },

        // EnergyRegen
        { (AttributeType.energyRegenModifier, ModificationType.Add), (m, v) => m.energyRegenModifier += v },
        { (AttributeType.energyRegenModifier, ModificationType.Multiply), (m, v) => m.energyRegenModifier *= v },
        { (AttributeType.energyRegenModifier, ModificationType.Override), (m, v) => m.energyRegenModifier = v },

        // MoveSpeed
        { (AttributeType.moveSpeedModifier, ModificationType.Add), (m, v) => m.moveSpeedModifier += v },
        { (AttributeType.moveSpeedModifier, ModificationType.Multiply), (m, v) => m.moveSpeedModifier *= v },
        { (AttributeType.moveSpeedModifier, ModificationType.Override), (m, v) => m.moveSpeedModifier = v },

        // MoveSpeedPercent
        { (AttributeType.moveSpeedModifierPercent, ModificationType.Add), (m, v) => m.moveSpeedModifierPercent += v },
        { (AttributeType.moveSpeedModifierPercent, ModificationType.Multiply), (m, v) => m.moveSpeedModifierPercent *= v },
        { (AttributeType.moveSpeedModifierPercent, ModificationType.Override), (m, v) => m.moveSpeedModifierPercent = v },

        // DashSpeed
        { (AttributeType.dashSpeedModifier, ModificationType.Add), (m, v) => m.dashSpeedModifier += v },
        { (AttributeType.dashSpeedModifier, ModificationType.Multiply), (m, v) => m.dashSpeedModifier *= v },
        { (AttributeType.dashSpeedModifier, ModificationType.Override), (m, v) => m.dashSpeedModifier = v },

        // DashCooldown
        { (AttributeType.dashCooldownModifier, ModificationType.Add), (m, v) => m.dashCooldownModifier += v },
        { (AttributeType.dashCooldownModifier, ModificationType.Multiply), (m, v) => m.dashCooldownModifier *= v },
        { (AttributeType.dashCooldownModifier, ModificationType.Override), (m, v) => m.dashCooldownModifier = v },

        // AttackInterval
        { (AttributeType.attackIntervalModifier, ModificationType.Add), (m, v) => m.attackIntervalModifier += v },
        { (AttributeType.attackIntervalModifier, ModificationType.Multiply), (m, v) => m.attackIntervalModifier *= v },
        { (AttributeType.attackIntervalModifier, ModificationType.Override), (m, v) => m.attackIntervalModifier = v },

        // AttackIntervalPercent
        { (AttributeType.attackIntervalModifierPercent, ModificationType.Add), (m, v) => m.attackIntervalModifierPercent += v },
        { (AttributeType.attackIntervalModifierPercent, ModificationType.Multiply), (m, v) => m.attackIntervalModifierPercent *= v },
        { (AttributeType.attackIntervalModifierPercent, ModificationType.Override), (m, v) => m.attackIntervalModifierPercent = v },

        // DamagePercent
        { (AttributeType.damagePercentModifier, ModificationType.Add), (m, v) => m.damagePercentModifier += v },
        { (AttributeType.damagePercentModifier, ModificationType.Multiply), (m, v) => m.damagePercentModifier *= v },
        { (AttributeType.damagePercentModifier, ModificationType.Override), (m, v) => m.damagePercentModifier = v },

        // PhysicalDamage
        { (AttributeType.physicalDamageModifier, ModificationType.Add), (m, v) => m.physicalDamageModifier += v },
        { (AttributeType.physicalDamageModifier, ModificationType.Multiply), (m, v) => m.physicalDamageModifier *= v },
        { (AttributeType.physicalDamageModifier, ModificationType.Override), (m, v) => m.physicalDamageModifier = v },

        // PhysicalDamagePercent
        { (AttributeType.physicalDamageModifierPercent, ModificationType.Add), (m, v) => m.physicalDamageModifierPercent += v },
        { (AttributeType.physicalDamageModifierPercent, ModificationType.Multiply), (m, v) => m.physicalDamageModifierPercent *= v },
        { (AttributeType.physicalDamageModifierPercent, ModificationType.Override), (m, v) => m.physicalDamageModifierPercent = v },

        // EnergyDamage
        { (AttributeType.energyDamageModifier, ModificationType.Add), (m, v) => m.energyDamageModifier += v },
        { (AttributeType.energyDamageModifier, ModificationType.Multiply), (m, v) => m.energyDamageModifier *= v },
        { (AttributeType.energyDamageModifier, ModificationType.Override), (m, v) => m.energyDamageModifier = v },

        { (AttributeType.energyDamageModifierPercent, ModificationType.Add), (m, v) => m.energyDamageModifierPercent += v },
        { (AttributeType.energyDamageModifierPercent, ModificationType.Multiply), (m, v) => m.energyDamageModifierPercent *= v },
        { (AttributeType.energyDamageModifierPercent, ModificationType.Override), (m, v) => m.energyDamageModifierPercent = v },

        // AttackRange
        { (AttributeType.attackRangeModifier, ModificationType.Add), (m, v) => m.attackRangeModifier += v },
        { (AttributeType.attackRangeModifier, ModificationType.Multiply), (m, v) => m.attackRangeModifier *= v },
        { (AttributeType.attackRangeModifier, ModificationType.Override), (m, v) => m.attackRangeModifier = v },

        // KnockbackForce
        { (AttributeType.knockbackForceModifier, ModificationType.Add), (m, v) => m.knockbackForceModifier += v },
        { (AttributeType.knockbackForceModifier, ModificationType.Multiply), (m, v) => m.knockbackForceModifier *= v },
        { (AttributeType.knockbackForceModifier, ModificationType.Override), (m, v) => m.knockbackForceModifier = v },

        // Shield
        { (AttributeType.shieldModifier, ModificationType.Add), (m, v) => m.shieldModifier += (int)v },
        { (AttributeType.shieldModifier, ModificationType.Multiply), (m, v) => m.shieldModifier = (int)(m.shieldModifier * v) },
        { (AttributeType.shieldModifier, ModificationType.Override), (m, v) => m.shieldModifier = (int)v },

        // Sanity
        { (AttributeType.sanityModifier, ModificationType.Add), (m, v) => m.sanityModifier += v },
        { (AttributeType.sanityModifier, ModificationType.Multiply), (m, v) => m.sanityModifier *= v },
        { (AttributeType.sanityModifier, ModificationType.Override), (m, v) => m.sanityModifier = v },
    };

    public void AddModifier(AttributeType type, ModificationType modificationType, 
                            float value = 0f)
    {
        var key = (type, modificationType);
        
        if (ModifierActions.TryGetValue(key, out var action))
        {
            action(this, value);
        }
        else
        {
            Debug.LogWarning($"未实现的属性修改器: {type} + {modificationType}");
        }
    }
}

/// <summary>
/// 修改目标属性类型枚举
/// </summary>
public enum AttributeType
{
    [Header("生命相关")]
    [Tooltip("最大生命修正(加)")]
    maxHealthModifier,          
    [Tooltip("最大生命修正(乘)")]
    maxHealthModifierPercent,
    [Tooltip("生命回复修正")]
    healthRegenModifier,

    [Header("能量相关")]
    [Tooltip("最大能量修正(加)")]
    maxEnergyModifier,
    [Tooltip("最大能量修正(百分比)")]
    maxEnergyModifierPercent,
    [Tooltip("能量回复修正")]
    energyRegenModifier,

    [Header("移动相关")]
    [Tooltip("移动速度修正(加)")]
    moveSpeedModifier,
    [Tooltip("移动速度修正(百分比)")]
    moveSpeedModifierPercent,
    [Tooltip("冲刺速度修正")]
    dashSpeedModifier,
    [Tooltip("冲刺冷却修正")]
    dashCooldownModifier,

    [Header("攻击相关")]
    [Tooltip("攻击间隔修正")]
    attackIntervalModifier,
    [Tooltip("攻击间隔修正（百分比）")]
    attackIntervalModifierPercent,
    [Tooltip("伤害倍率")]
    damagePercentModifier,
    [Tooltip("物理伤害修正")]
    physicalDamageModifier,
    [Tooltip("物理伤害修正(百分比)")]
    physicalDamageModifierPercent,
    [Tooltip("异能伤害修正")]
    energyDamageModifier,
    [Tooltip("异能伤害修正(百分比)")]
    energyDamageModifierPercent,
    [Tooltip("攻击范围修正")]
    attackRangeModifier,
    [Tooltip("击退力修正")]
    knockbackForceModifier,

    [Header("防御相关")]
    [Tooltip("护盾修正")]
    shieldModifier,
    [Tooltip("理智修正")]
    sanityModifier,
}

/// <summary>
/// 修改类型
/// </summary>
public enum ModificationType
{
    Add,        // 加法
    Multiply,   // 乘法
    Override    // 覆盖
}

/// <summary>
/// 属性修改效果
/// </summary>
[Serializable]
public class AttributeEffect
{
    [Tooltip("属性类型")]
    public AttributeType attributeType;

    [Tooltip("修改类型")]
    public ModificationType modificationType;

    [Tooltip("修改值")]
    public float value = 0f;
}

