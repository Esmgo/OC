using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色配置数据，使用ScriptableObject存储角色属性
/// </summary>
[CreateAssetMenu(fileName = "New Role Configuration", menuName = "Game/Role Configuration")]
public class RoleConfiguration : ScriptableObject
{
    [Header("基础信息")]
    [Tooltip("角色名称")]
    public string roleName = "Default Role";
    [Tooltip("角色描述")]
    [TextArea(3, 5)]
    public string description = "角色描述...";
    [Tooltip("主题色")]
    public Color themeColor = Color.white;
    [Tooltip("角色预制体地址（Addressables）")]
    public string rolePrefabAddress = "Role";

    [Header("属性部分")]
    [Tooltip("最大生命值")]
    public int maxHealth = 100;
    [Tooltip("生命回复")]
    public int healthRegen = 0;
    [Tooltip("最大能量")]
    public int maxEnergy = 100;
    [Tooltip("能量回复")]
    public int energyRegen = 0;
    [Tooltip("冷却缩减百分比")]
    public float cooldownReductionPercent = 0f;
    [Tooltip("冲刺冷却时间")]
    public float dashCooldown = 0.5f;
    [Tooltip("移动速度")]
    public int moveSpeed = 5;
    [Tooltip("攻击间隔")]
    public float attackInterval = 0.3f;
    [Tooltip("攻击速度提升百分比")]
    public float attackSpeedReductionPercent = 0;
    [Tooltip("物理伤害")]
    public int physicalDamage = 0;
    [Tooltip("异能伤害")]
    public int energyDamage = 0;
    [Tooltip("元素伤害")]
    public int elementalDamage = 0;
    [Tooltip("受击冷却")]
    public float hitCoolDown = 0.5f;


    [Tooltip("冲刺速度")]
    public float dashSpeed = 12f;
    
    [Tooltip("攻击速度百分比加成")]
    public float attackSpeedPercent = 0f;
    
    [Tooltip("精神值")]
    public float sanity = 100f;
    [Tooltip("攻击目标层")]
    public LayerMask targetLayer;
    [Tooltip("击退力度")]
    public float knockBackForce = 5.0f;
    [Tooltip("每次攻击消耗能量")]
    public int energyCostPerAttack = 0;
    [Tooltip("攻击距离")]

    public float attackRange = 5.0f;
    [Tooltip("近战攻击角度(度)")]
    public float meleeAttackAngle = 45f;
    [Tooltip("近战攻击角度修正(度)")]
    public float meleeFixAngle = 0f;

    [Header("远程攻击属性")]
    [Tooltip("子弹AA地址")]
    public string bulletAddress;
    [Tooltip("子弹速度")]
    public float bulletSpeed = 10.0f;

    [Tooltip("暴击率")]
    public float criticalRate = 0.1f;

    [Header("其他")]
    [Tooltip("角色图标")]
    public Sprite roleIcon;
    [Tooltip("角色立绘")]
    public Sprite standImage;

    [Header("武器配置")]
    [Tooltip("武器配置文件")]
    public List<WeaponConfiguration> weaponConfig = new();
}