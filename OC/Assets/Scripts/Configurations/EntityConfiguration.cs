using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityConfiguration : ScriptableObject
{
    [Header("通用属性部分")]
    [Tooltip("最大生命值")]
    public int maxHealth = 100;
    [Tooltip("移动速度")]
    public int moveSpeed = 5;
    [Tooltip("物理伤害")]
    public int physicalDamage = 0;
    [Tooltip("异能伤害")]
    public int manaDamage = 0;
    [Tooltip("元素伤害")]
    public int elementalDamage = 0;
    [Tooltip("受击冷却")]
    public float hitCoolDown = 0.5f;
}
