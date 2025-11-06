using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/////////////弃用的////////////////////////////////////////////////////////////////////////

/// <summary>
/// 属性修饰类
/// </summary>
public class StatModifier 
{
    [Tooltip("修正值")]
    public float value;
    [Tooltip("修正类型")]
    public ModifierType modifierType;
    [Tooltip("修正来源")]
    public readonly string source;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="value">修改值</param>
    /// <param name="statModType">修改类型</param>
    /// <param name="source">来源</param>
    public StatModifier(float value, ModifierType statModType, string source)
    {
        this.value = value;
        this.modifierType = statModType;
        this.source = source;
    }
}

//public enum ModifierType
//{
//    Add,       //相加
//    Percent, //百分比
//    Override //覆盖
//}
