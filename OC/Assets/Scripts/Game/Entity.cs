using BuffSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 实体基类
/// </summary>
public abstract class Entity : MonoBehaviour
{ 
    [Header("属性")]
    protected int maxHealth;      //  最大生命
    protected int healthRegen;   // 生命回复
    protected int maxEnergy;      //  最大能量
    protected int energyRegen;    // 能量回复
    protected float cooldownReductionPercent;    // 冷却缩减百分比
    protected float dashCoolDown;   // 冲刺冷却时间
    protected int moveSpeed;      // 移动速度
    protected float attackInterval;    // 攻击间隔
    protected float attackIntervalReductionPercent; // 攻击速度提升百分比
    protected int physicalDamage;     // 物理伤害
    protected int manaDamage;     // 能量伤害
    protected int elementalDamage;    // 元素伤害

    [Header("数值池(Value Pools)")]//这些用.currenValue 和 .maxValue 来访问谢谢喵
    protected ValuePool health;
    protected ValuePool energy;

    protected PropertyModifier localModifier;

    // 这些是最终计算出的、供其他脚本使用的值
    public int currentMaxHealth { get { return (int)health.maxValue; } private set { } }
    public int currentHealth { get { return (int)health.currentValue; } private set { } }
    public int currentMaxEnergy { get { return (int)energy.maxValue; } private set { } }
    public int currentEnergy { get { return (int)energy.currentValue; } private set { } }
    public float currentHealthRegenRate { get; private set; }
    public float currentEnergyRegenRate { get; private set; }
    public float currentCooldownReductionPercent { get; private set; }
    public float currentDashCoolDown { get; private set; }
    public float currentMoveSpeed { get; private set; }
    public float currentDashSpeed { get; private set; }
    public float currentAttackIntevalReductionPercent { get; private set; }
    public float currentAttackInterval { get; private set; }
    public float currentPhysicalDamage { get; private set; }
    public float currentManaDamage { get; private set; }
    public float currentElementalDamage { get; private set; }

    [Header("其他属性")]
    protected float hitCoolDown;
    protected float lastHitTime = -999f;

    #region 生命周期与事件订阅
    protected virtual void OnEnable()
    {
        //订阅全局加成变化事件
        if (GlobalStatModifier.Instance != null)
        {
            GlobalStatModifier.Instance.OnGlobalBonusesChanged += RecalculateAllStats;
        }
    }

    protected virtual void OnDisable()
    {
        if (GlobalStatModifier.Instance != null)
        {
            GlobalStatModifier.Instance.OnGlobalBonusesChanged -= RecalculateAllStats;
        }
    }
    #endregion

    public virtual void Init(RoleConfiguration config)
    {
        localModifier = new();

        maxHealth = config.maxHealth;
        healthRegen = config.healthRegen;
        maxEnergy = config.maxEnergy;
        energyRegen = config.energyRegen;
        cooldownReductionPercent = config.cooldownReductionPercent;
        dashCoolDown = config.dashCooldown;
        moveSpeed = config.moveSpeed;
        attackInterval = config.attackInterval;
        attackIntervalReductionPercent = config.attackSpeedReductionPercent;
        physicalDamage = config.physicalDamage;
        manaDamage = config.energyDamage;
        elementalDamage = config.elementalDamage;
        hitCoolDown = config.hitCoolDown;

        // 初始化 ValuePool
        health = new ValuePool(maxHealth);
        energy = new ValuePool(maxEnergy);

        RecalculateAllStats();
    }

    /// <summary>
    /// 计算所有最终属性。
    /// </summary>
    protected virtual void RecalculateAllStats()
    {
        if (GlobalStatModifier.Instance == null) return;

        var global = GlobalStatModifier.Instance.GlobalModifierForPlayer;

        float finalMaxHealth = (maxHealth + localModifier.GetModifier(ModifierType.MaxHealth_Add) + global.GetModifier(ModifierType.MaxHealth_Add)) * 
            (1 + localModifier.GetModifier(ModifierType.MaxHealth_Percent) + global.GetModifier(ModifierType.MaxHealth_Percent));
        health.SetMaxValue(Mathf.Max(finalMaxHealth,1), false);

        float finalMaxEnergy = (maxEnergy + localModifier.GetModifier(ModifierType.MaxEnergy_Add) + global.GetModifier(ModifierType.MaxEnergy_Add)) *
            (1 + localModifier.GetModifier(ModifierType.MaxEnergy_Percent) + global.GetModifier(ModifierType.MaxEnergy_Percent));
        energy.SetMaxValue(finalMaxEnergy, false);

        float healthRengen = (healthRegen + localModifier.GetModifier(ModifierType.HealthRegen_Add) + global.GetModifier(ModifierType.HealthRegen_Add)) * 
            (1 + localModifier.GetModifier(ModifierType.HealthRegen_Percent) + global.GetModifier(ModifierType.HealthRegen_Percent));
        currentHealthRegenRate = healthRengen > 0 ? 2.0f * Mathf.Pow(healthRengen, 0.5f) + 0.8f * Mathf.Pow(healthRengen, 0.33f) : 0;
    
        float energyRengen = (energyRegen + localModifier.GetModifier(ModifierType.EnergyRegen_Add) + global.GetModifier(ModifierType.EnergyRegen_Add)) *
            (1 + localModifier.GetModifier(ModifierType.EnergyRegen_Percent) + global.GetModifier(ModifierType.EnergyRegen_Percent));
        currentEnergyRegenRate = energyRengen > 0 ? 2.0f * Mathf.Pow(energyRengen, 0.5f) + 0.8f * Mathf.Pow(energyRengen, 0.33f) : 0;
    
        currentCooldownReductionPercent = cooldownReductionPercent + localModifier.GetModifier(ModifierType.CooldownReduction_Percent) + global.GetModifier(ModifierType.CooldownReduction_Percent);
    
        currentDashCoolDown = dashCoolDown / Mathf.Pow(2, currentCooldownReductionPercent);

        float _moveSpeed = (moveSpeed + localModifier.GetModifier(ModifierType.MoveSpeed_Add) + global.GetModifier(ModifierType.MoveSpeed_Add)) * 
            (1 + localModifier.GetModifier(ModifierType.MoveSpeed_Percent) + global.GetModifier(ModifierType.MoveSpeed_Percent));
        currentMoveSpeed = 12 * Mathf.Pow(1 + (_moveSpeed / 40), 0.7f);

        currentDashSpeed = currentMoveSpeed * 2.5f;

        currentAttackIntevalReductionPercent = attackIntervalReductionPercent + localModifier.GetModifier(ModifierType.AttackIntervalReduction_Percent) + global.GetModifier(ModifierType.AttackIntervalReduction_Percent);
    
        currentAttackInterval = attackInterval / Mathf.Pow(2, currentAttackIntevalReductionPercent);

        currentPhysicalDamage = (physicalDamage + localModifier.GetModifier(ModifierType.PhysicalDamage_Add) + global.GetModifier(ModifierType.PhysicalDamage_Add)) *
            (1 + localModifier.GetModifier(ModifierType.PhysicalDamage_Percent) + global.GetModifier(ModifierType.PhysicalDamage_Percent));
    
        currentManaDamage = (manaDamage + localModifier.GetModifier(ModifierType.ManaDamage_Add) + global.GetModifier(ModifierType.ManaDamage_Add)) *
            (1 + localModifier.GetModifier(ModifierType.ManaDamage_Percent) + global.GetModifier(ModifierType.ManaDamage_Percent));
    
        currentElementalDamage = (elementalDamage + localModifier.GetModifier(ModifierType.ElementalDamage_Add) + global.GetModifier(ModifierType.ElementalDamage_Add)) *
            (1 + localModifier.GetModifier(ModifierType.ElementalDamage_Percent) + global.GetModifier(ModifierType.ElementalDamage_Percent));


        OnStatChanged?.Invoke();
    }

    public void AddModifier(ModifierEffect effect)
    {
        if (effect == null) return;
        localModifier.AddModifier(effect.ModifierType, effect.value);
        RecalculateAllStats();
    }

    public abstract void TakeDamage(float amount);

    #region 事件
    public Action OnStatChanged;
    #endregion
}
