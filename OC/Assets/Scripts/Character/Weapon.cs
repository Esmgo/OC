using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Weapon : MonoBehaviour
{
    [Header("武器属性")]
    [Tooltip("攻击目标层")]
    [SerializeField] protected LayerMask targetLayer;
    [Tooltip("击退力度")]
    [SerializeField] protected float knockBackForce = 5.0f;
    [Tooltip("每次攻击消耗能量")]
    [SerializeField] protected int energyCostPerAttack = 0;

    protected float lastAttackTime = -999f; // 上次攻击时间
    protected Entity owner;

    public void Init(Character owner, RoleConfiguration config)
    {
        this.owner = owner;

        targetLayer = config.targetLayer;
        energyCostPerAttack = config.energyCostPerAttack;
        knockBackForce = config.knockBackForce;

    }

    public void TryAttack()
    {
        if(Time.time < lastAttackTime + owner.currentAttackInterval)
        {
            return;
        }

        if(owner.currentEnergy < energyCostPerAttack)
        {
            return;
        }
    }

    protected abstract void Attack();
}