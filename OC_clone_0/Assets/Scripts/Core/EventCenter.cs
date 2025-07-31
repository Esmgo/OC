using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件中心 - 基于C#事件系统的全局事件管理器
/// </summary>
public class EventCenter : MonoBehaviour
{
    public static EventCenter Instance { get; private set; }

    // 事件存储字典 - 使用弱引用避免内存泄漏
    private readonly Dictionary<Type, Delegate> eventDictionary = new Dictionary<Type, Delegate>();
    
    // 事件订阅计数器 - 用于调试
    private readonly Dictionary<Type, int> eventCounts = new Dictionary<Type, int>();

    #region 生命周期

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("EventCenter 初始化成功");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // 清理所有事件
        eventDictionary.Clear();
        eventCounts.Clear();
    }

    #endregion

    #region 事件订阅

    /// <summary>
    /// 订阅事件（无参数）
    /// </summary>
    public static void Subscribe<T>(Action handler) where T : IGameEvent
    {
        Instance?.SubscribeInternal<T>(handler);
    }

    /// <summary>
    /// 订阅事件（1个参数）
    /// </summary>
    public static void Subscribe<T, TArg>(Action<TArg> handler) where T : IGameEvent
    {
        Instance?.SubscribeInternal<T>(handler);
    }

    /// <summary>
    /// 订阅事件（2个参数）
    /// </summary>
    public static void Subscribe<T, TArg1, TArg2>(Action<TArg1, TArg2> handler) where T : IGameEvent
    {
        Instance?.SubscribeInternal<T>(handler);
    }

    /// <summary>
    /// 订阅事件（3个参数）
    /// </summary>
    public static void Subscribe<T, TArg1, TArg2, TArg3>(Action<TArg1, TArg2, TArg3> handler) where T : IGameEvent
    {
        Instance?.SubscribeInternal<T>(handler);
    }

    /// <summary>
    /// 内部订阅实现
    /// </summary>
    private void SubscribeInternal<T>(Delegate handler) where T : IGameEvent
    {
        Type eventType = typeof(T);
        
        if (eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType] = Delegate.Combine(eventDictionary[eventType], handler);
        }
        else
        {
            eventDictionary[eventType] = handler;
        }

        // 更新计数
        eventCounts[eventType] = eventCounts.GetValueOrDefault(eventType, 0) + 1;

        Debug.Log($"订阅事件: {eventType.Name}, 当前订阅数: {eventCounts[eventType]}");
    }

    #endregion

    #region 事件取消订阅

    /// <summary>
    /// 取消订阅事件（无参数）
    /// </summary>
    public static void Unsubscribe<T>(Action handler) where T : IGameEvent
    {
        Instance?.UnsubscribeInternal<T>(handler);
    }

    /// <summary>
    /// 取消订阅事件（1个参数）
    /// </summary>
    public static void Unsubscribe<T, TArg>(Action<TArg> handler) where T : IGameEvent
    {
        Instance?.UnsubscribeInternal<T>(handler);
    }

    /// <summary>
    /// 取消订阅事件（2个参数）
    /// </summary>
    public static void Unsubscribe<T, TArg1, TArg2>(Action<TArg1, TArg2> handler) where T : IGameEvent
    {
        Instance?.UnsubscribeInternal<T>(handler);
    }

    /// <summary>
    /// 取消订阅事件（3个参数）
    /// </summary>
    public static void Unsubscribe<T, TArg1, TArg2, TArg3>(Action<TArg1, TArg2, TArg3> handler) where T : IGameEvent
    {
        Instance?.UnsubscribeInternal<T>(handler);
    }

    /// <summary>
    /// 内部取消订阅实现
    /// </summary>
    private void UnsubscribeInternal<T>(Delegate handler) where T : IGameEvent
    {
        Type eventType = typeof(T);
        
        if (eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType] = Delegate.Remove(eventDictionary[eventType], handler);
            
            // 如果没有订阅者了，移除事件
            if (eventDictionary[eventType] == null)
            {
                eventDictionary.Remove(eventType);
                eventCounts.Remove(eventType);
            }
            else
            {
                eventCounts[eventType] = Math.Max(0, eventCounts[eventType] - 1);
            }

            Debug.Log($"取消订阅事件: {eventType.Name}, 剩余订阅数: {eventCounts.GetValueOrDefault(eventType, 0)}");
        }
    }

    #endregion

    #region 事件发布

    /// <summary>
    /// 发布事件（无参数）
    /// </summary>
    public static void Publish<T>() where T : IGameEvent
    {
        Instance?.PublishInternal<T>();
    }

    /// <summary>
    /// 发布事件（1个参数）
    /// </summary>
    public static void Publish<T, TArg>(TArg arg) where T : IGameEvent
    {
        Instance?.PublishInternal<T, TArg>(arg);
    }

    /// <summary>
    /// 发布事件（2个参数）
    /// </summary>
    public static void Publish<T, TArg1, TArg2>(TArg1 arg1, TArg2 arg2) where T : IGameEvent
    {
        Instance?.PublishInternal<T, TArg1, TArg2>(arg1, arg2);
    }

    /// <summary>
    /// 发布事件（3个参数）
    /// </summary>
    public static void Publish<T, TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3) where T : IGameEvent
    {
        Instance?.PublishInternal<T, TArg1, TArg2, TArg3>(arg1, arg2, arg3);
    }

    /// <summary>
    /// 内部发布实现（无参数）
    /// </summary>
    private void PublishInternal<T>() where T : IGameEvent
    {
        Type eventType = typeof(T);
        
        if (eventDictionary.TryGetValue(eventType, out Delegate eventDelegate))
        {
            try
            {
                (eventDelegate as Action)?.Invoke();
                Debug.Log($"发布事件: {eventType.Name}");
            }
            catch (Exception e)
            {
                Debug.LogError($"发布事件 {eventType.Name} 时发生异常: {e.Message}\n{e.StackTrace}");
            }
        }
    }

    /// <summary>
    /// 内部发布实现（1个参数）
    /// </summary>
    private void PublishInternal<T, TArg>(TArg arg) where T : IGameEvent
    {
        Type eventType = typeof(T);
        
        if (eventDictionary.TryGetValue(eventType, out Delegate eventDelegate))
        {
            try
            {
                (eventDelegate as Action<TArg>)?.Invoke(arg);
            }
            catch (Exception e)
            {
                Debug.LogError($"发布事件 {eventType.Name} 时发生异常: {e.Message}\n{e.StackTrace}");
            }
        }
    }

    /// <summary>
    /// 内部发布实现（2个参数）
    /// </summary>
    private void PublishInternal<T, TArg1, TArg2>(TArg1 arg1, TArg2 arg2) where T : IGameEvent
    {
        Type eventType = typeof(T);
        
        if (eventDictionary.TryGetValue(eventType, out Delegate eventDelegate))
        {
            try
            {
                (eventDelegate as Action<TArg1, TArg2>)?.Invoke(arg1, arg2);
                Debug.Log($"发布事件: {eventType.Name}, 参数: {arg1}, {arg2}");
            }
            catch (Exception e)
            {
                Debug.LogError($"发布事件 {eventType.Name} 时发生异常: {e.Message}\n{e.StackTrace}");
            }
        }
    }

    /// <summary>
    /// 内部发布实现（3个参数）
    /// </summary>
    private void PublishInternal<T, TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3) where T : IGameEvent
    {
        Type eventType = typeof(T);
        
        if (eventDictionary.TryGetValue(eventType, out Delegate eventDelegate))
        {
            try
            {
                (eventDelegate as Action<TArg1, TArg2, TArg3>)?.Invoke(arg1, arg2, arg3);
                Debug.Log($"发布事件: {eventType.Name}, 参数: {arg1}, {arg2}, {arg3}");
            }
            catch (Exception e)
            {
                Debug.LogError($"发布事件 {eventType.Name} 时发生异常: {e.Message}\n{e.StackTrace}");
            }
        }
    }

    #endregion

    #region 工具方法

    /// <summary>
    /// 检查事件是否有订阅者
    /// </summary>
    public static bool HasSubscribers<T>() where T : IGameEvent
    {
        return Instance != null && Instance.eventDictionary.ContainsKey(typeof(T));
    }

    /// <summary>
    /// 获取事件订阅者数量
    /// </summary>
    public static int GetSubscriberCount<T>() where T : IGameEvent
    {
        if (Instance == null) return 0;
        return Instance.eventCounts.GetValueOrDefault(typeof(T), 0);
    }

    /// <summary>
    /// 清除指定类型的所有订阅者
    /// </summary>
    public static void ClearSubscribers<T>() where T : IGameEvent
    {
        Instance?.ClearSubscribersInternal<T>();
    }

    private void ClearSubscribersInternal<T>() where T : IGameEvent
    {
        Type eventType = typeof(T);
        eventDictionary.Remove(eventType);
        eventCounts.Remove(eventType);
        Debug.Log($"清除事件所有订阅者: {eventType.Name}");
    }

    /// <summary>
    /// 清除所有事件订阅者
    /// </summary>
    public static void ClearAllSubscribers()
    {
        if (Instance != null)
        {
            Instance.eventDictionary.Clear();
            Instance.eventCounts.Clear();
            Debug.Log("清除所有事件订阅者");
        }
    }

    #endregion

    #region 调试方法

    /// <summary>
    /// 打印所有事件信息
    /// </summary>
    [ContextMenu("Print All Events")]
    public void PrintAllEvents()
    {
        Debug.Log("=== 事件中心状态 ===");
        foreach (var kvp in eventCounts)
        {
            Debug.Log($"事件: {kvp.Key.Name}, 订阅者数量: {kvp.Value}");
        }
    }

    /// <summary>
    /// 获取事件统计信息
    /// </summary>
    public EventCenterStats GetStats()
    {
        // 计算总订阅者数量
        int totalSubs = 0;
        foreach (var count in eventCounts.Values)
        {
            totalSubs += count;
        }

        // 创建事件详情字典
        var details = new Dictionary<string, int>();
        foreach (var kvp in eventCounts)
        {
            details[kvp.Key.Name] = kvp.Value;
        }

        return new EventCenterStats
        {
            totalEventTypes = eventDictionary.Count,
            totalSubscribers = totalSubs,
            eventDetails = details
        };
    }

    #endregion
}

/// <summary>
/// 事件中心统计信息
/// </summary>
[System.Serializable]
public struct EventCenterStats
{
    public int totalEventTypes;
    public int totalSubscribers;
    public Dictionary<string, int> eventDetails;
}