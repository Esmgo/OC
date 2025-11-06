using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;



/////////////弃用的////////////////////////////////////////////////////////////////////////
/// <summary>
/// 属性基类 - 现在作为局部修正值的容器
/// </summary>
public class Stat
{
    [Tooltip("基础值")]
    private readonly float baseValue;

    [Tooltip("属性修正值列表")]
    protected readonly List<StatModifier> statModifiers;
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;

    //新增：用于关联全局属性
    //public readonly AttributeType addAttributeType;
    //public readonly AttributeType percentAttributeType;

    private bool isModifierDirty = true;
    private float lastAddModifier = 0;
    private float lastPercentModifier = 0;
    private float lastOverrideModifier = 0;

    public Stat(float baseValue)
    {
        this.baseValue = baseValue;
        statModifiers = new List<StatModifier>();
        StatModifiers = statModifiers.AsReadOnly();
    }

    /// <summary>
    /// 获取所有“Flat”类型修正的总和。
    /// </summary>
    //public float GetModifier(StatModifier modifierType)
    //{
    //    if (isModifierDirty)
    //    {

    //    }
    //    else
    //    {
    //        switch(modifierType)
    //        {
    //            case ModifierType.
    //        }
    //    }
    //}

    /// <summary>
    /// 获取所有“Percent”类型修正的总和。
    /// </summary>
    //public float GetPercentageModifier()
    //{
    //    float total = 0;
    //    for (int i = 0; i < statModifiers.Count; i++)
    //    {
    //        if (statModifiers[i].modifierType == ModifierType.Percent)
    //        {
    //            total += statModifiers[i].value;
    //        }
    //    }
    //    return total;
    //}

    public void AddModifier(StatModifier mod)
    {
        statModifiers.Add(mod);
    }

    public void RemoveModifierAll()
    {
        statModifiers.Clear();
    }

    public bool RemoveModifier(string source)
    {
        bool didRemove = false;
        for (int i = statModifiers.Count - 1; i >= 0; i--)
        {
            if (statModifiers[i].source == source)
            {
                statModifiers.RemoveAt(i);
                didRemove = true;
            }
        }
        return didRemove;
    }

    public float GetBaseValue()
    {
        return baseValue;
    }
}
