using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
[CreateAssetMenu(fileName = "New Weapon Configuration", menuName = "Game/Weapon Configuration")]
public class WeaponConfiguration : ScriptableObject
{
    [Header("通用武器属性")]
    [Tooltip("名字")]
    public string weaponName = "Default Weapon";

    [Tooltip("攻击间隔(秒)")]
    public float attackInterval = 1.0f;

    [Tooltip("攻击目标层")]
    public LayerMask targetLayer;

    [Tooltip("击退力度")]
    public float knockBackForce = 5.0f;

    [Tooltip("攻击伤害(小数百分比)")]
    public float damagePercent = 1;

    [Tooltip("每次攻击消耗能量")]
    public int energyCostPerAttack = 0;

    [Header("近战攻击属性")]
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

    [Header("伤害")]
    [Tooltip("物理伤害")]
    public float physicalDamage = 0;

    [Tooltip("异能伤害")]
    public float energyDamage = 0;
}
