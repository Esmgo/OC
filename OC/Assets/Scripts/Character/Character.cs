using BuffSystem;
using GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static Mirror.BouncyCastle.Math.EC.ECCurve;

public class Character //: Entity 
{
    //[Header("角色属性")]
    //protected int healthRegen;   // 生命回复
    //protected int maxEnergy;      //  最大能量
    //protected int energyRegen;    // 能量回复
    //protected float cooldownReductionPercent;    // 冷却缩减百分比
    //protected float dashCoolDown;   // 冲刺冷却时间
    //protected float attackInterval;    // 攻击间隔
    //protected float attackIntervalReductionPercent; // 攻击速度提升百分比
    //protected int energyCostPerAttack; // 每次攻击消耗的能量

    //protected ValuePool energy;

    //// 这些是最终计算出的、供其他脚本使用的值
    //public int currentMaxEnergy { get { return (int)energy.maxValue; } private set { } }
    //public int currentEnergy { get { return (int)energy.currentValue; } private set { } }
    //public float currentHealthRegenRate { get; private set; }
    //public float currentEnergyRegenRate { get; private set; }
    //public float currentCooldownReductionPercent { get; private set; }
    //public float currentDashCoolDown { get; private set; }
    //public float currentDashSpeed { get; private set; }
    //public float currentAttackIntevalReductionPercent { get; private set; }
    //public float currentAttackInterval { get; private set; }
    //public int currentEnergyCostPerAttack { get; private set; }

    ////运行时数据
    //[Tooltip("角色的UI画布")]
    //public Canvas canvas;
    //[Tooltip("角色的武器")]
    //private Weapon weaponComponent;
    //[Tooltip("角色的精灵图")]
    //private SpriteRenderer[] spriteRenderers;
    //[Tooltip("角色的移动组件")]
    //private MoveBase moveComponent;
    //[Tooltip("角色的buff管理器")]
    //private BuffManager buffManager;

    ////private void OnEnable()
    ////{
    ////    EventCenter.Subscribe<SetFlashEvent, float, Color>(SetFlash);
    ////}

    ////private void OnDestroy()
    ////{
    ////    EventCenter.Unsubscribe<SetFlashEvent, float, Color>(SetFlash);
    ////}

    //private void Update()
    //{
    //    //if (!buffManager.HasBuff(BuffType.EnergyWeak))
    //    //{
    //    //    energyRegenValue += energyRegenRate * Time.deltaTime;
    //    //    if (energyRegenValue >= 1f)
    //    //    {
    //    //        int energyToAdd = Mathf.FloorToInt(energyRegenValue);
    //    //        Energy(energyToAdd);
    //    //        energyRegenValue -= energyToAdd;
    //    //    }
    //    //}
    //    HandleAttackInput();
    //}

    //private void HandleAttackInput()
    //{
    //    // 当玩家按下鼠标左键
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        // 命令武器尝试攻击
    //        weaponComponent?.TryAttack();
    //    }
    //}

    ///// <summary>
    ///// 角色初始化
    ///// </summary>
    ///// <param name="roleConfig"></param>
    ///// <param name="weaponConfig"></param>
    //public void Init(CharacterConfiguration roleConfig, GlobalStatModifier gsm)
    //{
    //    healthRegen = roleConfig.healthRegen;
    //    maxEnergy = roleConfig.maxEnergy;
    //    energyRegen = roleConfig.energyRegen;
    //    cooldownReductionPercent = roleConfig.cooldownReductionPercent;
    //    dashCoolDown = roleConfig.dashCooldown;
    //    attackInterval = roleConfig.attackInterval;
    //    attackIntervalReductionPercent = roleConfig.attackSpeedReductionPercent;
    //    energyCostPerAttack = roleConfig.energyCostPerAttack;

    //    energy = new ValuePool(roleConfig.maxEnergy);

    //    base.Init(roleConfig, gsm);

    //    RecalculateAllStats();

    //    // 初始化武器组件
    //    weaponComponent = GetComponentInChildren<Weapon>();
    //    weaponComponent.Init(this, roleConfig.weaponConfig[0]);

    //    //获取画布
    //    canvas = GetComponentInChildren<Canvas>();
        
    //    spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

    //    //获取Buff管理器
    //    buffManager = GetComponent<BuffManager>();
    //}

    //protected override void RecalculateAllStats()
    //{
    //    if (GlobalStatModifier.Instance == null) return;

    //    var globalModifier = GlobalStatModifier.Instance.GlobalModifierForPlayer;

    //    base.RecalculateBaseStats(globalModifier);

    //    float finalMaxEnergy = (maxEnergy + localModifier.GetModifier(ModifierType.MaxEnergy_Add) + globalModifier.GetModifier(ModifierType.MaxEnergy_Add)) *
    //        (1 + localModifier.GetModifier(ModifierType.MaxEnergy_Percent) + globalModifier.GetModifier(ModifierType.MaxEnergy_Percent));
    //    energy.SetMaxValue(finalMaxEnergy, false);

    //    float healthRengen = (healthRegen + localModifier.GetModifier(ModifierType.HealthRegen_Add) + globalModifier.GetModifier(ModifierType.HealthRegen_Add)) *
    //        (1 + localModifier.GetModifier(ModifierType.HealthRegen_Percent) + globalModifier.GetModifier(ModifierType.HealthRegen_Percent));
    //    currentHealthRegenRate = healthRengen > 0 ? 2.0f * Mathf.Pow(healthRengen, 0.5f) + 0.8f * Mathf.Pow(healthRengen, 0.33f) : 0;

    //    float energyRengen = (energyRegen + localModifier.GetModifier(ModifierType.EnergyRegen_Add) + globalModifier.GetModifier(ModifierType.EnergyRegen_Add)) *
    //        (1 + localModifier.GetModifier(ModifierType.EnergyRegen_Percent) + globalModifier.GetModifier(ModifierType.EnergyRegen_Percent));
    //    currentEnergyRegenRate = energyRengen > 0 ? 2.0f * Mathf.Pow(energyRengen, 0.5f) + 0.8f * Mathf.Pow(energyRengen, 0.33f) : 0;

    //    currentCooldownReductionPercent = cooldownReductionPercent + localModifier.GetModifier(ModifierType.CooldownReduction_Percent) + globalModifier.GetModifier(ModifierType.CooldownReduction_Percent);

    //    currentDashCoolDown = dashCoolDown / Mathf.Pow(2, currentCooldownReductionPercent);

    //    currentDashSpeed = currentMoveSpeed * 2.5f;

    //    currentAttackIntevalReductionPercent = attackIntervalReductionPercent + localModifier.GetModifier(ModifierType.AttackIntervalReduction_Percent) + globalModifier.GetModifier(ModifierType.AttackIntervalReduction_Percent);

    //    currentAttackInterval = attackInterval / Mathf.Pow(2, currentAttackIntevalReductionPercent);
    //}

    //private void SetFlash(float speed, Color color)
    //{
    //    foreach (var sr in spriteRenderers)
    //    {
    //        Material mat = sr.material;
    //        mat.SetFloat("_Speed", speed);
    //        mat.SetColor("_Color", color);
    //    }
    //}

    //public override void TakeDamage(float physicalDamage, float energyDamage)
    //{
        
    //}
    //public override void TakeHeal(int amount)
    //{
        
    //}

    ///// <summary>
    ///// 能量值操作
    ///// </summary>
    ///// <param name="amount"></param>
    //public void Energy(int amount)
    //{
    //}

    /////// <summary>
    /////// 当能量耗尽时调用的方法。
    /////// </summary>
    ////private void OnEnergyDepleted()
    ////{
    ////    if (buffManager != null && !buffManager.HasBuff(BuffType.EnergyWeak))
    ////    {
    ////        buffManager.AddBuff<EnergyWeak>();
    ////        var t = ObjectPoolManager.Instance.GetObject<TextPopUp>("TextPopUp", transform.position, Quaternion.identity);
    ////        if (t != null && canvas != null)
    ////        {
    ////            t.transform.SetParent(canvas.transform);
    ////            t.Show("能量耗尽！", 2, Color.white, 0.5f, true);
    ////        }
    ////    }
    ////}
}
