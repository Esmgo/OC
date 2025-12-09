using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Role Configuration", menuName = "Game/Config/Role Configuration")]
public class CharacterConfiguration : ScriptableObject
{
    //[Tooltip("")]
    [Header("基础信息")]
    [Tooltip("名字")]
    public string roleName = "Default Role";
    [Tooltip("角色描述")]
    [TextArea(3, 5)]
    public string description = "角色描述...";
    [Tooltip("主题色")]
    public Color themeColor = Color.white;
    [Tooltip("角色预制体地址（Addressables）")]
    public string characterPrefabAddress;

    [Header("属性数值配置")]
    [Tooltip("最大生命")]
    public int maxHealth = 100;
    public float healthRegen = 0;
    public int maxEnergy = 100;
    public float energyRegen = 0;

    public float moveSpeed = 10f;
    public float dashSpeed = 20f;
    public float dashCooldown = 0.5f;

    public int physicalDamage = 10;
    public int energyDamage = 0;
    public int elementalDamage = 0;
    public float attackInterval = 0.3f;
    public int energyCostPerAttack = 0;


    //[Tooltip("冷却缩减百分比")]
    //public float cooldownReductionPercent = 0f;
    //[Tooltip("冲刺冷却时间")]
    //public float dashCooldown = 0.5f;
    //[Tooltip("攻击间隔")]
    //public float attackInterval = 0.3f;
    //[Tooltip("攻击速度提升百分比")]
    //public float attackSpeedReductionPercent = 0;
    //[Tooltip("每次攻击消耗能量")]
    //public int energyCostPerAttack = 0;
}
