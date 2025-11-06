using GameEvents;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

/// <summary>
/// Buff 类型枚举,所有的buff类型都在这里定义
/// </summary>
public enum BuffType
{
    Berserk,    // 狂暴
    EnergyWeak, //能量虚弱
    //Poison,     // 中毒
    // 在这里添加新的buff类型
}

/// <summary>
/// 所有Buff的实现都在这个脚本里
/// </summary>
namespace BuffSystem
{
    /// <summary>
    /// 狂暴Buff：攻击间隔减少 + 移动速度加快 + 伤害增加
    /// </summary>
    public class BerserkBuff : Buff
    {
        public BerserkBuff()
        {
            buffType = BuffType.Berserk;
            buffName = "狂暴";
            duration = 10f;
            canStack = false;
            isPermanent = false;
        }

        public override void OnApply()
        {
            if (targetModifier == null) return;

            targetModifier.AddModifier(ModifierType.MoveSpeed_Percent, 0.5f);
            targetModifier.AddModifier(ModifierType.AttackIntervalReduction_Percent, -0.5f);
            targetModifier.AddModifier(ModifierType.PhysicalDamage_Percent, 2f);
            targetModifier.AddModifier(ModifierType.ManaDamage_Percent, 2f);
            targetModifier.AddModifier(ModifierType.ElementalDamage_Percent, 2f);

            EventCenter.Publish<SetFlashEvent, float, Color>(duration, new Color(1, 0, 0.8f));
        }

        public override void OnRemove()
        {
            targetModifier.AddModifier(ModifierType.MoveSpeed_Percent, -0.5f);
            targetModifier.AddModifier(ModifierType.AttackIntervalReduction_Percent, 0.5f);
            targetModifier.AddModifier(ModifierType.PhysicalDamage_Percent, -2f);
            targetModifier.AddModifier(ModifierType.ManaDamage_Percent, -2f);
            targetModifier.AddModifier(ModifierType.ElementalDamage_Percent, -2f);

            EventCenter.Publish<SetFlashEvent, float, Color>(duration, Color.black);
        }
    }

    /// <summary>
    /// 能量耗尽的特殊虚弱
    /// </summary>
    public class EnergyWeak : Buff
    {
        public EnergyWeak()
        {
            buffType = BuffType.EnergyWeak;
            buffName = "能量虚弱";
            duration = 5f;
            canStack = false;
            isPermanent = false;
        }
        public override void OnApply()
        {
            if (targetModifier == null) return;

            targetModifier.AddModifier(ModifierType.AttackIntervalReduction_Percent, 0.5f);
            targetModifier.AddModifier(ModifierType.PhysicalDamage_Percent, -0.5f);
            targetModifier.AddModifier(ModifierType.ManaDamage_Percent, -0.5f);
            targetModifier.AddModifier(ModifierType.ElementalDamage_Percent, -0.5f);
            //移速增加
        }

        public override void OnRemove()
        {
            targetModifier.AddModifier(ModifierType.AttackIntervalReduction_Percent, -0.5f);
            targetModifier.AddModifier(ModifierType.PhysicalDamage_Percent, 0.5f);
            targetModifier.AddModifier(ModifierType.ManaDamage_Percent, 0.5f);
            targetModifier.AddModifier(ModifierType.ElementalDamage_Percent, 0.5f);

            EventCenter.Publish<WeakOverEvent>();
        }
    }

    ///// <summary>
    ///// 中毒Buff：每隔一定时间受到伤害
    ///// </summary>
    //[System.Serializable]
    //public class PoisonBuff : Buff
    //{
    //    [Header("中毒效果")]
    //    public int damagePerTick = 2;      // 每次伤害
    //    public float tickInterval = 1f;    // 伤害间隔

    //    private float nextTickTime;
    //    private bool hasApplied = false;

    //    public PoisonBuff()
    //    {
    //        buffType = BuffType.Poison;
    //        buffName = "中毒";
    //        duration = 8f;
    //        canStack = true;  // 中毒可以叠加
    //        isPermanent = false;
    //    }

    //    public override void OnApply()
    //    {
    //        if (target == null) return;

    //        Debug.Log($"{target.roleName} 中毒了！");
    //        nextTickTime = Time.time + tickInterval;
    //        hasApplied = true;
    //    }

    //    public override void OnRemove()
    //    {
    //        if (target == null || !hasApplied) return;

    //        Debug.Log($"{target.roleName} 中毒效果结束");
    //        hasApplied = false;
    //    }

    //    public override void OnUpdate(float deltaTime)
    //    {
    //        base.OnUpdate(deltaTime);

    //        if (!isActive || !hasApplied) return;

    //        // 检查是否到了伤害时间
    //        if (Time.time >= nextTickTime)
    //        {
    //            OnTick();
    //            nextTickTime = Time.time + tickInterval;
    //        }
    //    }

    //    public override void OnTick()
    //    {
    //        if (target == null) return;

    //        // 计算总伤害（基于叠加层数）
    //        int totalDamage = damagePerTick * stackCount;

    //        // 发布伤害事件
    //        EventCenter.Publish<HPChangeEvent, int>(-totalDamage);

    //        // 显示中毒伤害文本
    //        var dt = ObjectPoolManager.Instance.GetObject<TextPopUp>("TextPopUp", target.transform.position);
    //        if (dt != null)
    //        {
    //            dt.transform.SetParent(target.canvas.transform);
    //            dt.Show($"-{totalDamage}", 2f, Color.green, 0.4f, false);
    //        }

    //        Debug.Log($"{target.roleName} 受到中毒伤害: {totalDamage}");
    //    }

    //    public override bool TryStack(Buff newBuff)
    //    {
    //        if (base.TryStack(newBuff))
    //        {
    //            Debug.Log($"{target.roleName} 中毒层数增加到 {stackCount} 层");
    //            return true;
    //        }
    //        return false;
    //    }
    //}

    ///// <summary>
    ///// 护盾Buff：增加临时护盾值
    ///// </summary>
    //[System.Serializable]
    //public class ShieldBuff : Buff
    //{
    //    [Header("护盾效果")]
    //    public int shieldAmount = 50;     // 护盾值

    //    private int originalShield;
    //    private bool hasApplied = false;

    //    public ShieldBuff()
    //    {
    //        //buffType = BuffType.Shield;
    //        buffName = "护盾";
    //        duration = 15f;
    //        canStack = false;
    //        isPermanent = false;
    //    }

    //    public override void OnApply()
    //    {
    //        if (target == null || hasApplied) return;

    //        Debug.Log($"{target.roleName} 获得护盾！");

    //        // 保存原始护盾值并增加
    //        originalShield = target.shield;
    //        target.shield += shieldAmount;

    //        hasApplied = true;
    //    }

    //    public override void OnRemove()
    //    {
    //        if (target == null || !hasApplied) return;

    //        Debug.Log($"{target.roleName} 护盾效果结束");

    //        // 恢复原始护盾值
    //        target.shield = originalShield;

    //        hasApplied = false;
    //    }
    //}

    ///// <summary>
    ///// 加速Buff：提升移动速度
    ///// </summary>
    //[System.Serializable]
    //public class SpeedBuff : Buff
    //{
    //    [Header("加速效果")]
    //    public float speedMultiplier = 1.5f;  // 速度倍数

    //    private float originalMoveSpeed;
    //    private bool hasApplied = false;

    //    public SpeedBuff()
    //    {
    //        //buffType = BuffType.Speed;
    //        buffName = "加速";
    //        duration = 8f;
    //        canStack = false;
    //        isPermanent = false;
    //    }

    //    public override void OnApply()
    //    {
    //        if (target == null || hasApplied) return;

    //        Debug.Log($"{target.roleName} 获得加速效果！");

    //        // 保存原始移动速度并应用倍数
    //        originalMoveSpeed = target.moveSpeed;
    //        target.moveSpeed *= speedMultiplier;

    //        hasApplied = true;
    //    }

    //    public override void OnRemove()
    //    {
    //        if (target == null || !hasApplied) return;

    //        Debug.Log($"{target.roleName} 加速效果结束");

    //        // 恢复原始移动速度
    //        target.moveSpeed = originalMoveSpeed;

    //        hasApplied = false;
    //    }
    //}

    ///// <summary>
    ///// 回血Buff：持续回复生命值
    ///// </summary>
    //[System.Serializable]
    //public class RegenBuff : Buff
    //{
    //    [Header("回血效果")]
    //    public int healPerTick = 3;        // 每次回复量
    //    public float tickInterval = 2f;    // 回复间隔

    //    private float nextTickTime;
    //    private bool hasApplied = false;

    //    public RegenBuff()
    //    {
    //        //buffType = BuffType.Regeneration; 
    //        buffName = "回血";
    //        duration = 12f;
    //        canStack = true;  // 可以叠加
    //        isPermanent = false;
    //    }

    //    public override void OnApply()
    //    {
    //        if (target == null) return;

    //        Debug.Log($"{target.roleName} 开始回血！");
    //        nextTickTime = Time.time + tickInterval;
    //        hasApplied = true;
    //    }

    //    public override void OnRemove()
    //    {
    //        if (target == null || !hasApplied) return;

    //        Debug.Log($"{target.roleName} 回血效果结束");
    //        hasApplied = false;
    //    }

    //    public override void OnUpdate(float deltaTime)
    //    {
    //        base.OnUpdate(deltaTime);

    //        if (!isActive || !hasApplied) return;

    //        // 检查是否到了回复时间
    //        if (Time.time >= nextTickTime)
    //        {
    //            OnTick();
    //            nextTickTime = Time.time + tickInterval;
    //        }
    //    }

    //    public override void OnTick()
    //    {
    //        if (target == null) return;

    //        // 计算总回复量（基于叠加层数）
    //        int totalHeal = healPerTick * stackCount;

    //        // 发布治疗事件
    //        EventCenter.Publish<HPChangeEvent, int>(totalHeal);

    //        // 显示治疗文本
    //        var dt = ObjectPoolManager.Instance.GetObject<TextPopUp>("TextPopUp", target.transform.position);
    //        if (dt != null)
    //        {
    //            dt.transform.SetParent(target.canvas.transform);
    //            dt.Show($"+{totalHeal}", 2f, Color.green, 0.4f, false);
    //        }

    //        Debug.Log($"{target.roleName} 回复生命值: {totalHeal}");
    //    }

    //    public override bool TryStack(Buff newBuff)
    //    {
    //        if (base.TryStack(newBuff))
    //        {
    //            Debug.Log($"{target.roleName} 回血层数增加到 {stackCount} 层");
    //            return true;
    //        }
    //        return false;
    //    }
    //}
}