using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemConfiguration : ScriptableObject
{
    [Header("基础信息")]
    [Tooltip("道具名称")]
    public string itemName;

    [Tooltip("道具描述")]
    [TextArea(2, 4)]
    public string description;

    [Tooltip("道具图标")]
    public Sprite icon;

    [Tooltip("道具稀有度")]
    public ItemRarity rarity = ItemRarity.Common;

    [Tooltip("道具来源")]
    public ItemSource source = ItemSource.Shop;

    [Tooltip("最大叠加数量")]
    public int maxStackCount = 1;

    [Header("商店相关")]
    [Tooltip("商店售价")]
    public int price = 0;

    [Header("运行时数据")]
    [Tooltip("当前叠加数量")]
    public int count = 0; 

    public virtual void OnGet() { }

    public virtual void OnDashEnd() { }

    //[Header("属性修改（获得时一次性触发）")]
    //[Tooltip("属性修改器列表")]
    //public List<AttributeEffect> attributeEffects = new();

    //[Header("机制效果（全局条件触发）")]
    //[Tooltip("机制效果列表")]
    //public List<MechanicEffect> mechanicEffects = new();
}

/// <summary>
/// 道具稀有度
/// </summary>
public enum ItemRarity
{
    Common,     // 普通 - 白色
    Uncommon,   // 非凡 - 绿色
    Rare,       // 稀有 - 蓝色
    Epic,       // 史诗 - 紫色
    Legendary   // 传说 - 橙色
}

/// <summary>
/// 道具来源
/// </summary>
public enum ItemSource
{
    Shop,           // 商店购买
    CombatDrop,     // 战斗掉落
    Both            // 两者都有
}

/// <summary>
/// 触发类型
/// </summary>
public enum TriggerType
{
    OnGet,         // 获得时触发
    OnCooldownEnd,   // 冷却结束时触发
    OnDamageTaken,   // 受到伤害时触发
    OnKill           // 击杀敌人时触发
}

/// <summary>
/// 机制效果
/// </summary>
[Serializable]
public class MechanicEffect
{
    [Tooltip("机制类型")]
    public MechanicType mechanicType;

    [Header("触发条件")]
    [Tooltip("触发类型")]
    public TriggerType triggerType = TriggerType.OnGet;

    [Tooltip("持续时间（-1表示永久）")]
    public float duration = -1f;

    [Tooltip("效果参数")]
    public MechanicParameters parameters;
}

/// <summary>
/// 机制类型
/// </summary>
public enum MechanicType
{
    BulletSplit,        // 子弹分裂
    BulletPierce,       // 子弹穿透
    BulletBounce,       // 子弹反弹
    MultiShot,          // 多重射击
    ChainLightning,     // 连锁闪电
    AreaDamage,         // 范围伤害
    LifeSteal,          // 生命偷取
    EnergySteal,        // 能量偷取
    CriticalHit,        // 暴击
    Freeze,             // 冰冻
    Burn,               // 燃烧
    Poison,             // 中毒
    Shield,             // 护盾
    Invisible,          // 隐身
    Teleport,           // 传送
    TimeStop,           // 时间停止
    ExperienceBoost,    // 经验加成
    MagnetRadius        // 磁铁范围
}

/// <summary>
/// 机制参数
/// </summary>
[Serializable]
public class MechanicParameters
{
    [Tooltip("整数参数1")]
    public int intParam1 = 0;

    [Tooltip("整数参数2")]
    public int intParam2 = 0;

    [Tooltip("浮点参数1")]
    public float floatParam1 = 0f;

    [Tooltip("浮点参数2")]
    public float floatParam2 = 0f;

    [Tooltip("布尔参数")]
    public bool boolParam = false;

    [Tooltip("字符串参数")]
    public string stringParam = "";
}
