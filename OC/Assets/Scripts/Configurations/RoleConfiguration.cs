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

    [Header("人物属性")]
    [Tooltip("最大生命值")]
    public int maxHealth = 100;
   
    [Tooltip("生命回复(每秒)")]
    public float healthRegenRate = 0f;

    [Tooltip("能量")]
    public float energy = 100f;

    [Tooltip("能量回复(每秒)")]
    public float energyRegenRate = 0f;

    [Tooltip("角色图标")]
    public Sprite roleIcon;

    [Tooltip("角色立绘")]
    public Sprite standImage;

    [Header("移动属性")]
    [Tooltip("移动速度")]
    public float moveSpeed = 5f;

    [Tooltip("冲刺速度")]
    public float dashSpeed = 12f;

    [Tooltip("冲刺冷却时间")]
    public float dashCooldown = 0.5f;

    [Tooltip("攻击间隔")]
    public float attackInterval = 0.3f;

    [Tooltip("物理伤害")]
    public float physicalDamage = 0f;

    [Tooltip("元素伤害")]
    public float elementalDamage = 0f;

    [Tooltip("异能伤害")]
    public float energyDamage = 0f;

    [Header("能力")]
    [Tooltip("护甲值")]
    public int shield = 0;

    [Tooltip("精神值")]
    public float sanity = 100f;

    [Header("武器配置")]
    [Tooltip("武器配置文件")]
    public List<WeaponConfiguration> weaponConfig = new();
}