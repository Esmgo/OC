using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Character role;
    public float damage => role.physicalDamage * physicalDamageRatio + 
                          role.elementalDamage * elementalDamageRatio + 
                          role.energyDamage * energyDamageRatio;

    [Header("武器属性")]

    [Tooltip("攻击间隔")]
    public float attackInterval = 1.0f;

    [Header("攻击属性")]
    [Tooltip("攻击伤害(百分比)")]
    public float damagePercent = 1;

    [Tooltip("虚弱状态攻击间隔")]
    public float weakenedAttackInterval = 1.0f;

    [Tooltip("虚弱状态伤害")]
    public float weakenedDamage = 0.2f;

    [Tooltip("子弹预制体")]
    public GameObject bulletPrefab;

    [Header("伤害占比")]
    [Tooltip("物理伤害占比")]
    public float physicalDamageRatio = 0;

    [Tooltip("元素伤害占比")]
    public float elementalDamageRatio = 0;

    [Tooltip("异能伤害占比")]
    public float energyDamageRatio = 0;

    public void Init(Character role, WeaponConfiguration weaponConfig)
    {
        this.role = role;
        attackInterval = weaponConfig.attackInterval;
        physicalDamageRatio = weaponConfig.physicalDamageRatio;
        elementalDamageRatio = weaponConfig.elementalDamageRatio;
        energyDamageRatio = weaponConfig.energyDamageRatio;
        weakenedAttackInterval = attackInterval * (100 + weaponConfig.weakenedAttackInterval)/100;
        //weakenedDamage = ;
    }

    public virtual void Attack() { }
}