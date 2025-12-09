using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterWeaponComponent : MonoBehaviour
{
    public int physicalDamage { get; protected set; }
    public int energyDamage { get; protected set; }
    public int elementalDamage { get; protected set; }
    public float attackInterval { get; protected set; }
    public float attackCost { get; protected set; }
    public float lastAttackTime { get; protected set; }

    public void Init(CharacterConfiguration config)
    {
        physicalDamage = config.physicalDamage;
        energyDamage = config.energyDamage;
        elementalDamage = config.elementalDamage;
        attackInterval = config.attackInterval;
        attackCost = config.energyCostPerAttack;

        OnInit(config);
    }

    protected virtual void OnInit(CharacterConfiguration config) { }

    public void UpdateInfo(WeaponStats weaponStats)
    {
        physicalDamage = weaponStats.physicalDamage;
        energyDamage = weaponStats.energyDamage;
        elementalDamage = weaponStats.elementalDamage;
        attackInterval = weaponStats.attackInterval;
        attackCost = weaponStats.attackCost;
    }

    public void Attack() 
    { 
        lastAttackTime = Time.time;
        OnAttack();
    }

    protected virtual void OnAttack() { }

    public Action OnStartAttack;
    public Action OnStopAttack;

    public void StartAttack()
    {
        OnStartAttack?.Invoke();
    }

    public void StopAttack()
    {
        OnStopAttack?.Invoke();
    }
}

public struct WeaponStats
{
    public int physicalDamage;
    public int energyDamage;
    public int elementalDamage;
    public float attackInterval;
    public float attackCost;

    public WeaponStats(int physicalDamage, int energyDamage, int elementalDamage, float attackInterval, float attackCost)
    {
        this.physicalDamage = physicalDamage;
        this.energyDamage = energyDamage;
        this.elementalDamage = elementalDamage;
        this.attackInterval = attackInterval;
        this.attackCost = attackCost;
    }
}