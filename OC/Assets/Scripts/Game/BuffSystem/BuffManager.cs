using GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Buff 管理器
/// </summary>
public class BuffManager : MonoBehaviour
{
    [Header("调试信息")]
    public List<Buff> activeBuffs = new List<Buff>();

    private PlayerAttributeModifier modifier;
    
    void Awake()
    {
        modifier = GetComponent<Character>().modifier;
    }
    
    void Update()
    {
        UpdateBuffs();
    }
    
    /// <summary>
    /// 更新所有Buff
    /// </summary>
    private void UpdateBuffs()
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            var buff = activeBuffs[i];
            if (buff.isActive)
            {
                buff.OnUpdate(Time.deltaTime);
            }
            else
            {
                RemoveBuff(buff);
            }
        }
    }
    
    /// <summary>
    /// 添加Buff
    /// </summary>
    public void AddBuff<T>(float duration = -1) where T : Buff, new()
    {
        var newBuff = new T();
        AddBuff(newBuff, duration);
        EventCenter.Publish<PlayerAttributeChangedEvent>();
    }
    
    /// <summary>
    /// 添加Buff
    /// </summary>
    public void AddBuff(Buff newBuff, float duration = -1)
    {
        if (newBuff == null || modifier == null) return;
        
        // 检查是否已存在相同类型的Buff
        var existingBuff = activeBuffs.FirstOrDefault(b => b.buffType == newBuff.buffType);
        
        if (existingBuff != null)
        {
            // 尝试叠加
            if (existingBuff.TryStack(newBuff))
            {
                return; // 叠加成功，不需要添加新的
            }
            else
            {
                // 不能叠加，覆盖已有buff的时间
                if (duration > 0)
                {
                    existingBuff.remainingTime = duration;
                }
                else if (newBuff.duration > 0)
                {
                    existingBuff.remainingTime = newBuff.duration;
                }
                return; // 覆盖完成，不需要添加新的buff
            }
        }
        
        // 初始化并添加新Buff
        newBuff.Initialize(modifier, duration);
        activeBuffs.Add(newBuff);
        newBuff.OnApply();
        
        Debug.Log($"添加Buff: {newBuff.buffName}");
    }
    
    /// <summary>
    /// 移除Buff
    /// </summary>
    public void RemoveBuff(Buff buff)
    {
        if (buff == null) return;
        
        buff.OnRemove();
        activeBuffs.Remove(buff);
        EventCenter.Publish<PlayerAttributeChangedEvent>();

        Debug.Log($"移除Buff: {buff.buffName}");
    }
    
    /// <summary>
    /// 移除指定类型的Buff
    /// </summary>
    public void RemoveBuffByType(BuffType buffType)
    {
        var buff = activeBuffs.FirstOrDefault(b => b.buffType == buffType);
        if (buff != null)
        {
            RemoveBuff(buff);
        }
    }
    
    /// <summary>
    /// 检查是否有指定类型的Buff
    /// </summary>
    public bool HasBuff(BuffType buffType)
    {
        return activeBuffs.Any(b => b.buffType == buffType && b.isActive);
    }
    
    /// <summary>
    /// 获取指定类型的Buff
    /// </summary>
    public Buff GetBuff(BuffType buffType)
    {
        return activeBuffs.FirstOrDefault(b => b.buffType == buffType && b.isActive);
    }
    
    /// <summary>
    /// 清除所有Buff
    /// </summary>
    public void ClearAllBuffs()
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            RemoveBuff(activeBuffs[i]);
        }
    }
    
    /// <summary>
    /// 获取Buff信息（用于UI显示）
    /// </summary>
    public List<BuffInfo> GetBuffInfos()
    {
        return activeBuffs.Where(b => b.isActive)
                         .Select(b => new BuffInfo
                         {
                             buffType = b.buffType,
                             name = b.buffName,
                             remainingTime = b.remainingTime,
                             stackCount = b.stackCount,
                             isPermanent = b.isPermanent
                         }).ToList();
    }
}

/// <summary>
/// Buff信息结构（用于UI显示）
/// </summary>
[Serializable]
public struct BuffInfo
{
    public BuffType buffType;
    public string name;
    public float remainingTime;
    public int stackCount;
    public bool isPermanent;
}