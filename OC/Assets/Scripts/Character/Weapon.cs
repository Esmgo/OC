using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 武器抽象基类
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    [Header("运行时数据")]
    [Tooltip("武器的持有者")]
    protected Character owner;
    [Tooltip("上次攻击的时间")]
    protected float lastAttackTime = -999f;

    /// <summary>
    /// 初始化武器
    /// </summary>
    /// <param name="owner">武器的持有者</param>
    /// <param name="config">武器的配置 ScriptableObject</param>
    public virtual void Init(Character owner, WeaponConfiguration weaponConfiguration)
    {
        this.owner = owner;
    }

    /// <summary>
    /// 尝试执行一次攻击。这是外部调用的主要入口。
    /// </summary>
    public void TryAttack()
    {
        // 检查攻击间隔和能量消耗
        if (!CanAttack())
        {
            return;
        }

        lastAttackTime = Time.time;
        //owner.Energy(owner.currentEnergyCostPerAttack); // 建议在Entity中添加一个消耗能量的方法

        // 执行具体的攻击逻辑
        Attack();
    }

    /// <summary>
    /// 检查是否满足攻击条件
    /// </summary>
    /// <returns>如果可以攻击，返回 true</returns>
    protected virtual bool CanAttack()
    {
        // 检查攻击间隔
        //if (Time.time < lastAttackTime + owner.currentAttackInterval)
        //{
        //    return false;
        //}

        //// 检查能量是否足够
        //if (owner.currentEnergy < owner.currentEnergyCostPerAttack)
        //{
        //    return false;
        //}

        return true;
    }

    /// <summary>
    /// 抽象的攻击方法，由子类（如远程、近战武器）实现具体逻辑。
    /// </summary>
    protected abstract void Attack();
}