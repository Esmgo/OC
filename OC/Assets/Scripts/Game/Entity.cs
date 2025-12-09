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
    [Header("通用属性")]
    protected int maxHealth;      //  最大生命
    protected int moveSpeed;      // 移动速度
    protected int physicalDamage;     // 物理伤害
    protected int manaDamage;     // 能量伤害
    protected int elementalDamage;    // 元素伤害
    protected float hitCoolDown;   // 受击冷却时间


    [Header("数值池(Value Pools)")]//这些用.currenValue 和 .maxValue 来访问谢谢喵
    protected ValuePool health;

    protected PropertyModifier localModifier;

    [Tooltip("配置文件")]
    protected EntityConfiguration config;

    // 这些是最终计算出的、供其他脚本使用的值
    public int currentMaxHealth { get { return (int)health.maxValue; } private set { } }
    public int currentHealth { get { return (int)health.currentValue; } private set { } }
    public float currentMoveSpeed { get; private set; }
    public float currentPhysicalDamage { get; private set; }
    public float currentManaDamage { get; private set; }
    public float currentElementalDamage { get; private set; }
    public float currentHitCoolDown { get; private set; }

    [Header("其他属性")]
    protected float lastHitTime = -999f;
    protected GlobalStatModifier gsm;

    protected void Init(EntityConfiguration config, GlobalStatModifier gsm)
    {
        this.config = config;

        maxHealth = config.maxHealth;
        moveSpeed = config.moveSpeed;
        physicalDamage = config.physicalDamage;
        manaDamage = config.manaDamage;
        elementalDamage = config.elementalDamage;
        hitCoolDown = config.hitCoolDown;

        // 初始化 ValuePool
        localModifier = new PropertyModifier();
        health = new ValuePool(maxHealth);

        this.gsm = gsm;
        gsm.OnGlobalPlayerModifierChanged += RecalculateAllStats;
    }

    private void OnDestroy()
    {
            gsm.OnGlobalPlayerModifierChanged -= RecalculateAllStats;
    }

    /// <summary>
    /// 对所有属性进行重新计算。
    /// 实现时请调用RecalculateBaseStats(PropertyModifier globalModifier)以确保基类属性被正确计算。
    /// </summary>
    protected abstract void RecalculateAllStats();
    /// <summary>
    /// 计算所有最终属性。
    /// </summary>
    protected void RecalculateBaseStats(PropertyModifier globalModifier)
    {
        if (globalModifier == null)
        {
            Debug.LogError("GlobalModifier is null in RecalculateAllStats!");
            return;
        }

        // 计算最终属性值
        float finalMaxHealth = (maxHealth + localModifier.GetModifier(ModifierType.MaxHealth_Add) + globalModifier.GetModifier(ModifierType.MaxHealth_Add)) * 
            (1 + localModifier.GetModifier(ModifierType.MaxHealth_Percent) + globalModifier.GetModifier(ModifierType.MaxHealth_Percent));
        health.SetMaxValue(Mathf.Max(finalMaxHealth,1), false);

        float _moveSpeed = (moveSpeed + localModifier.GetModifier(ModifierType.MoveSpeed_Add) + globalModifier.GetModifier(ModifierType.MoveSpeed_Add)) * 
            (1 + localModifier.GetModifier(ModifierType.MoveSpeed_Percent) + globalModifier.GetModifier(ModifierType.MoveSpeed_Percent));
        currentMoveSpeed = 12 * Mathf.Pow(1 + (_moveSpeed / 40), 0.7f);

        currentPhysicalDamage = (physicalDamage + localModifier.GetModifier(ModifierType.PhysicalDamage_Add) + globalModifier.GetModifier(ModifierType.PhysicalDamage_Add)) *
            (1 + localModifier.GetModifier(ModifierType.PhysicalDamage_Percent) + globalModifier.GetModifier(ModifierType.PhysicalDamage_Percent));
    
        currentManaDamage = (manaDamage + localModifier.GetModifier(ModifierType.ManaDamage_Add) + globalModifier.GetModifier(ModifierType.ManaDamage_Add)) *
            (1 + localModifier.GetModifier(ModifierType.ManaDamage_Percent) + globalModifier.GetModifier(ModifierType.ManaDamage_Percent));
    
        currentElementalDamage = (elementalDamage + localModifier.GetModifier(ModifierType.ElementalDamage_Add) + globalModifier.GetModifier(ModifierType.ElementalDamage_Add)) *
            (1 + localModifier.GetModifier(ModifierType.ElementalDamage_Percent) + globalModifier.GetModifier(ModifierType.ElementalDamage_Percent));

        OnStatChanged?.Invoke();
    }

    public void AddModifier(ModifierPack effect)
    {
        if (effect == null) return;
        localModifier.AddModifier(effect.modifierType, effect.value);
        RecalculateAllStats();
    }

    public abstract void TakeDamage(float physicalDamage, float energyDamage);
    public abstract void TakeHeal(int amount);

    #region 事件
    /// <summary>
    /// RecalculateBaseStats调用时同步触发或通过子类触发，通知属性改变
    /// </summary>
    public Action OnStatChanged;
    #endregion
}
