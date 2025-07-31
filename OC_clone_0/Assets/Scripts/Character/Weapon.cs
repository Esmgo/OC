using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Character role;
    public float damage => role.physicalDamage * physicalDamageRatio + 
                          role.elementalDamage * elementalDamageRatio + 
                          role.energyDamage * energyDamageRatio;

    [Header("��������")]

    [Tooltip("�������")]
    public float attackInterval = 1.0f;

    [Header("��������")]
    [Tooltip("�����˺�(�ٷֱ�)")]
    public float damagePercent = 1;

    [Tooltip("����״̬�������")]
    public float weakenedAttackInterval = 1.0f;

    [Tooltip("����״̬�˺�")]
    public float weakenedDamage = 0.2f;

    [Tooltip("�ӵ�Ԥ����")]
    public GameObject bulletPrefab;

    [Header("�˺�ռ��")]
    [Tooltip("�����˺�ռ��")]
    public float physicalDamageRatio = 0;

    [Tooltip("Ԫ���˺�ռ��")]
    public float elementalDamageRatio = 0;

    [Tooltip("�����˺�ռ��")]
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