using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有道具 ScriptableObject 的抽象基类。
/// </summary>
public abstract class ItemConfiguration : ScriptableObject
{
    [Header("基础信息")]
    [Tooltip("道具名称")]
    public string itemName;

    [Tooltip("道具描述")]
    [TextArea(3, 5)]
    public string description;

    [Tooltip("道具图标")]
    public Sprite icon;

    [Tooltip("道具稀有度")]
    public ItemRarity rarity = ItemRarity.White;

    [Tooltip("最大叠加数量")]
    [Range(1, 9999)]
    public int maxStackCount = 1;

    [Header("商店相关")]
    [Tooltip("商店售价")]
    public int price = 0;

    /// <summary>
    /// 当实体获取此道具时调用的核心方法。
    /// </summary>
    public abstract void OnGet(Entity entity);
}

