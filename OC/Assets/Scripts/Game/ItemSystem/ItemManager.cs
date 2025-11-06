using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局道具管理器，负责管理玩家持有的所有道具及其数量。
/// </summary>
public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    /// <summary>
    /// 存储玩家当前持有的所有道具及其叠加数量的字典。
    /// Key: 道具的ScriptableObject定义。
    /// Value: 当前持有的数量。
    /// </summary>
    private readonly Dictionary<ItemConfiguration, int> _inventory = new Dictionary<ItemConfiguration, int>();

    /// <summary>
    /// 当一个道具被添加或其数量更新时触发。
    /// 参数1: 添加的道具, 参数2: 该道具新的总数量。
    /// </summary>
    public event Action<ItemConfiguration, int> OnItemUpdated;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        ResetInventory();
    }

    /// <summary>
    /// 为指定实体添加一个道具。
    /// 这是外部系统（如商店、掉落物）与道具系统交互的主要入口。
    /// </summary>
    /// <param name="itemToAdd">要添加的道具的ScriptableObject。</param>
    /// <param name="entity">获取该道具的实体。</param>
    public void AddItem(ItemConfiguration itemToAdd, Entity entity)
    {
        if (itemToAdd == null || entity == null) return;

        // 检查库存中是否已有此道具，并获取当前数量
        _inventory.TryGetValue(itemToAdd, out int currentCount);

        // 如果当前数量小于最大叠加数，则可以添加
        if (currentCount < itemToAdd.maxStackCount)
        {
            // 增加数量
            currentCount++;
            _inventory[itemToAdd] = currentCount;

            Debug.Log($"添加道具: {itemToAdd.itemName}，当前数量: {currentCount}");

            // 关键：调用道具自身的 OnGet 方法来触发其效果！
            itemToAdd.OnGet(entity); // 假设 OnGet 现在不需要 entity 参数

            // 触发事件，通知UI等系统进行更新
            OnItemUpdated?.Invoke(itemToAdd, currentCount);
        }
        else
        {
            Debug.Log($"道具 {itemToAdd.itemName} 已达到最大叠加数量 ({itemToAdd.maxStackCount})，无法继续添加。");
        }
    }

    /// <summary>
    /// 查询指定道具的当前持有数量。
    /// </summary>
    /// <param name="item">要查询的道具。</param>
    /// <returns>持有的数量，如果从未获得过则为0。</returns>
    public int GetItemCount(ItemConfiguration item)
    {
        _inventory.TryGetValue(item, out int count);
        return count;
    }

    /// <summary>
    /// 检查是否持有至少一个指定的道具。
    /// </summary>
    public bool HasItem(ItemConfiguration item)
    {
        return GetItemCount(item) > 0;
    }

    /// <summary>
    /// 重置所有道具库存，用于开始新游戏。
    /// </summary>
    public void ResetInventory()
    {
        _inventory.Clear();
        Debug.Log("玩家道具库存已重置。");
    }
}
