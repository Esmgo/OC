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
    [Tooltip("攻击目标层")]
    public LayerMask targetLayer;
    [Tooltip("每次攻击消耗能量")]
    public int energyCostPerAttack = 0;

    [Header("近战武器属性")]
    [Tooltip("攻击范围（半径）")]
    public float attackRange = 1.5f;
    [Tooltip("攻击角度（扇形）")]
    [Range(0, 360)]
    public float attackAngle = 90f;


    [Header("远程攻击属性")]
    [Tooltip("子弹AA地址")]
    public string bulletAddress;
    [Tooltip("子弹速度")]
    public float bulletSpeed = 10.0f;
}
