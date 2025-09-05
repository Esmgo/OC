using System;
using UnityEngine;

/// <summary>
/// Buff 基类
/// </summary>
[Serializable]
public abstract class Buff
{
    [Header("基础属性")]
    public BuffType buffType;
    public string buffName;
    public float duration;      // 持续时间
    public bool isPermanent;    // 是否永久
    public bool canStack;       // 是否可以叠加
    
    [Header("运行时数据")]
    public float remainingTime; // 剩余时间
    public int stackCount;      // 叠加层数
    public bool isActive;       // 是否激活
    

    protected PlayerAttributeModifier targetModifier; // 目标角色
    
    /// <summary>
    /// 初始化Buff
    /// </summary>
    public virtual void Initialize(PlayerAttributeModifier target, float duration = -1)
    {
        this.targetModifier = target;
        this.remainingTime = duration > 0 ? duration : this.duration;
        this.isActive = true;
        this.stackCount = 1;
    }
    
    /// <summary>
    /// 应用Buff效果
    /// </summary>
    public abstract void OnApply();
    
    /// <summary>
    /// 移除Buff效果
    /// </summary>
    public abstract void OnRemove();
    
    /// <summary>
    /// 更新Buff（每帧调用）
    /// </summary>
    public virtual void OnUpdate(float deltaTime)
    {
        if (!isActive || isPermanent) return;
        
        remainingTime -= deltaTime;
        if (remainingTime <= 0)
        {
            isActive = false;
        }
    }
    
    /// <summary>
    /// Buff效果（用于持续性效果，如中毒）
    /// </summary>
    public virtual void OnTick() { }
    
    /// <summary>
    /// 尝试叠加Buff
    /// </summary>
    public virtual bool TryStack(Buff newBuff)
    {
        if (!canStack) return false;
        
        stackCount++;
        remainingTime = newBuff.remainingTime; // 刷新持续时间
        return true;
    }
}