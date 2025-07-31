using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
[CreateAssetMenu(fileName = "New Weapon Configuration", menuName = "Game/Weapon Configuration")]
public class WeaponConfiguration : ScriptableObject
{
    [Header("武器属性")]
    [Tooltip("名字")]
    public string weaponName = "Default Weapon";

    [Tooltip("攻击间隔")]
    public float attackInterval = 1.0f;

    [Tooltip("虚弱状态攻击间隔增加（百分比）")]
    public float weakenedAttackInterval = 0.2f;

    [Tooltip("虚弱状态伤害衰减（百分比）")]
    public float weakenedDamageReduction = 0.2f;

    [Tooltip("子弹预制体")]
    public GameObject bulletPrefab;

    [Header("伤害占比")]
    [Tooltip("物理伤害占比")]
    public float physicalDamageRatio = 0;

    [Tooltip("元素伤害占比")]
    public float elementalDamageRatio = 0;

    [Tooltip("异能伤害占比")]
    public float energyDamageRatio = 0;
}
