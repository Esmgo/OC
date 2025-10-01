using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Weapon : MonoBehaviour
{
    [Header("通用武器属性")]
    [Tooltip("名字")]
    [SerializeField] protected string weaponName = "Default Weapon";

    [Tooltip("攻击间隔(秒)")]
    [SerializeField] protected float attackInterval = 1.0f;

    [Tooltip("攻击目标层")]
    [SerializeField] protected LayerMask targetLayer;

    [Tooltip("击退力度")]
    [SerializeField] protected float knockBackForce = 5.0f;

    [Tooltip("伤害倍率")]
    [SerializeField] protected float damagePercent = 1;

    [Tooltip("每次攻击消耗能量")]
    [SerializeField] protected int energyCostPerAttack = 0;

    [Header("近战攻击属性")]
    [Tooltip("攻击距离")]
    [SerializeField] protected float attackRange = 5.0f;

    [Tooltip("近战攻击角度(度)")]
    [SerializeField] protected float meleeAttackAngle = 45f;

    [Tooltip("近战攻击角度修正(度)")]
    [SerializeField] protected float meleeFixAngle = 0f;

    [Header("远程攻击属性")]
    [Tooltip("子弹AA地址")]
    [SerializeField] protected string bulletAddress;

    [Tooltip("子弹速度")]
    [SerializeField] protected float bulletSpeed = 10.0f;

    [Header("伤害")]
    [Tooltip("物理伤害")]
    [SerializeField] protected float physicalDamage = 0;

    [Tooltip("异能伤害")]
    [SerializeField] protected float energyDamage = 0;

    protected float lastAttackTime = -999f; // 上次攻击时间

    protected Character role;

    protected float damage => (physicalDamage + energyDamage) * damagePercent;

    public void Init(Character role, WeaponConfiguration weaponConfig)
    {
        this.role = role;

        weaponName = weaponConfig.weaponName;
        attackInterval = weaponConfig.attackInterval;
        targetLayer = weaponConfig.targetLayer;
        knockBackForce = weaponConfig.knockBackForce;
        energyCostPerAttack = weaponConfig.energyCostPerAttack;
        damagePercent = weaponConfig.damagePercent;

        attackRange = weaponConfig.attackRange;
        meleeAttackAngle = weaponConfig.meleeAttackAngle;
        meleeFixAngle = weaponConfig.meleeFixAngle;

        bulletAddress = weaponConfig.bulletAddress;
        bulletSpeed = weaponConfig.bulletSpeed;
        
        physicalDamage = weaponConfig.physicalDamage;
        energyDamage = weaponConfig.energyDamage;
    }

    public void UpdateData(float attackInterval, float damagePercent)
    {
        this.attackInterval = attackInterval;
        this.damagePercent = damagePercent;
    }

    protected float GetPhysicalDamage()
    {
        return physicalDamage;
    }

    protected float GetEnergyDamage()
    {
        return energyDamage;
    }
}