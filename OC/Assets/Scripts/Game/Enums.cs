using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 这里定义游戏中使用的各种枚举类型
/// </summary>
#region 道具相关
/// <summary>
/// 道具稀有度
/// </summary>
public enum ItemRarity
{
    White,     
    Green,   
    Blue,      
    Purple,      
    Golden,
    Red,
    Black
}

/// <summary>
/// 道具来源
/// </summary>
public enum ItemSource
{
    Shop,           // 商店购买
    CombatDrop,     // 战斗掉落
    Both            // 两者都有
}
#endregion