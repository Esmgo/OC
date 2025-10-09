using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeaponBase : Weapon
{
    [Header("RangedWeaponBase设置")]
    [Tooltip("子弹发射点")]
    [SerializeField] protected Transform firePoint;

    protected float weakTimer = 0f; // 虚弱状态计时器  

    private void Start()
    {
        if (firePoint == null)
        {
            Debug.LogError("RangedWeaponBase: FirePoint is not assigned!");
        }
    }

    protected virtual void Update()
    {
        if (Input.GetMouseButton(0) && (Time.time >= lastAttackTime + attackInterval))
        {
             BaseAttack();
             lastAttackTime = Time.time;
        }
    }

    protected virtual void BaseAttack()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = firePoint.position.z;
        Vector3 direction = (mouseWorldPos - firePoint.position).normalized;
        // 从对象池获取子弹并初始化
        BulletBase bullet = ObjectPoolManager.Instance.GetObject<BulletBase>(bulletAddress, firePoint.position, firePoint.rotation);
        if (bullet != null)
        {
            bullet.Initialize(direction, (int)GetPhysicalDamage(), bulletSpeed, targetLayer, knockBackForce);
        }

        EventCenter.Publish<EnergyChangeEvent, int>(-energyCostPerAttack);
    }
}
