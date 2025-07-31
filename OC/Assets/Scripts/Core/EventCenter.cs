using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �¼����� - ����C#�¼�ϵͳ��ȫ���¼�������
/// </summary>
public class EventCenter : MonoBehaviour
{
    public static EventCenter Instance { get; private set; }

    // �¼��洢�ֵ� - ʹ�������ñ����ڴ�й©
    private readonly Dictionary<Type, Delegate> eventDictionary = new Dictionary<Type, Delegate>();
    
    // �¼����ļ����� - ���ڵ���
    private readonly Dictionary<Type, int> eventCounts = new Dictionary<Type, int>();

    #region ��������

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("EventCenter ��ʼ���ɹ�");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // ���������¼�
        eventDictionary.Clear();
        eventCounts.Clear();
    }

    #endregion

    #region �¼�����

    /// <summary>
    /// �����¼����޲�����
    /// </summary>
    public static void Subscribe<T>(Action handler) where T : IGameEvent
    {
        Instance?.SubscribeInternal<T>(handler);
    }

    /// <summary>
    /// �����¼���1��������
    /// </summary>
    public static void Subscribe<T, TArg>(Action<TArg> handler) where T : IGameEvent
    {
        Instance?.SubscribeInternal<T>(handler);
    }

    /// <summary>
    /// �����¼���2��������
    /// </summary>
    public static void Subscribe<T, TArg1, TArg2>(Action<TArg1, TArg2> handler) where T : IGameEvent
    {
        Instance?.SubscribeInternal<T>(handler);
    }

    /// <summary>
    /// �����¼���3��������
    /// </summary>
    public static void Subscribe<T, TArg1, TArg2, TArg3>(Action<TArg1, TArg2, TArg3> handler) where T : IGameEvent
    {
        Instance?.SubscribeInternal<T>(handler);
    }

    /// <summary>
    /// �ڲ�����ʵ��
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

        // ���¼���
        eventCounts[eventType] = eventCounts.GetValueOrDefault(eventType, 0) + 1;

        Debug.Log($"�����¼�: {eventType.Name}, ��ǰ������: {eventCounts[eventType]}");
    }

    #endregion

    #region �¼�ȡ������

    /// <summary>
    /// ȡ�������¼����޲�����
    /// </summary>
    public static void Unsubscribe<T>(Action handler) where T : IGameEvent
    {
        Instance?.UnsubscribeInternal<T>(handler);
    }

    /// <summary>
    /// ȡ�������¼���1��������
    /// </summary>
    public static void Unsubscribe<T, TArg>(Action<TArg> handler) where T : IGameEvent
    {
        Instance?.UnsubscribeInternal<T>(handler);
    }

    /// <summary>
    /// ȡ�������¼���2��������
    /// </summary>
    public static void Unsubscribe<T, TArg1, TArg2>(Action<TArg1, TArg2> handler) where T : IGameEvent
    {
        Instance?.UnsubscribeInternal<T>(handler);
    }

    /// <summary>
    /// ȡ�������¼���3��������
    /// </summary>
    public static void Unsubscribe<T, TArg1, TArg2, TArg3>(Action<TArg1, TArg2, TArg3> handler) where T : IGameEvent
    {
        Instance?.UnsubscribeInternal<T>(handler);
    }

    /// <summary>
    /// �ڲ�ȡ������ʵ��
    /// </summary>
    private void UnsubscribeInternal<T>(Delegate handler) where T : IGameEvent
    {
        Type eventType = typeof(T);
        
        if (eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType] = Delegate.Remove(eventDictionary[eventType], handler);
            
            // ���û�ж������ˣ��Ƴ��¼�
            if (eventDictionary[eventType] == null)
            {
                eventDictionary.Remove(eventType);
                eventCounts.Remove(eventType);
            }
            else
            {
                eventCounts[eventType] = Math.Max(0, eventCounts[eventType] - 1);
            }

            Debug.Log($"ȡ�������¼�: {eventType.Name}, ʣ�ඩ����: {eventCounts.GetValueOrDefault(eventType, 0)}");
        }
    }

    #endregion

    #region �¼�����

    /// <summary>
    /// �����¼����޲�����
    /// </summary>
    public static void Publish<T>() where T : IGameEvent
    {
        Instance?.PublishInternal<T>();
    }

    /// <summary>
    /// �����¼���1��������
    /// </summary>
    public static void Publish<T, TArg>(TArg arg) where T : IGameEvent
    {
        Instance?.PublishInternal<T, TArg>(arg);
    }

    /// <summary>
    /// �����¼���2��������
    /// </summary>
    public static void Publish<T, TArg1, TArg2>(TArg1 arg1, TArg2 arg2) where T : IGameEvent
    {
        Instance?.PublishInternal<T, TArg1, TArg2>(arg1, arg2);
    }

    /// <summary>
    /// �����¼���3��������
    /// </summary>
    public static void Publish<T, TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3) where T : IGameEvent
    {
        Instance?.PublishInternal<T, TArg1, TArg2, TArg3>(arg1, arg2, arg3);
    }

    /// <summary>
    /// �ڲ�����ʵ�֣��޲�����
    /// </summary>
    private void PublishInternal<T>() where T : IGameEvent
    {
        Type eventType = typeof(T);
        
        if (eventDictionary.TryGetValue(eventType, out Delegate eventDelegate))
        {
            try
            {
                (eventDelegate as Action)?.Invoke();
                Debug.Log($"�����¼�: {eventType.Name}");
            }
            catch (Exception e)
            {
                Debug.LogError($"�����¼� {eventType.Name} ʱ�����쳣: {e.Message}\n{e.StackTrace}");
            }
        }
    }

    /// <summary>
    /// �ڲ�����ʵ�֣�1��������
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
                Debug.LogError($"�����¼� {eventType.Name} ʱ�����쳣: {e.Message}\n{e.StackTrace}");
            }
        }
    }

    /// <summary>
    /// �ڲ�����ʵ�֣�2��������
    /// </summary>
    private void PublishInternal<T, TArg1, TArg2>(TArg1 arg1, TArg2 arg2) where T : IGameEvent
    {
        Type eventType = typeof(T);
        
        if (eventDictionary.TryGetValue(eventType, out Delegate eventDelegate))
        {
            try
            {
                (eventDelegate as Action<TArg1, TArg2>)?.Invoke(arg1, arg2);
                Debug.Log($"�����¼�: {eventType.Name}, ����: {arg1}, {arg2}");
            }
            catch (Exception e)
            {
                Debug.LogError($"�����¼� {eventType.Name} ʱ�����쳣: {e.Message}\n{e.StackTrace}");
            }
        }
    }

    /// <summary>
    /// �ڲ�����ʵ�֣�3��������
    /// </summary>
    private void PublishInternal<T, TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3) where T : IGameEvent
    {
        Type eventType = typeof(T);
        
        if (eventDictionary.TryGetValue(eventType, out Delegate eventDelegate))
        {
            try
            {
                (eventDelegate as Action<TArg1, TArg2, TArg3>)?.Invoke(arg1, arg2, arg3);
                Debug.Log($"�����¼�: {eventType.Name}, ����: {arg1}, {arg2}, {arg3}");
            }
            catch (Exception e)
            {
                Debug.LogError($"�����¼� {eventType.Name} ʱ�����쳣: {e.Message}\n{e.StackTrace}");
            }
        }
    }

    #endregion

    #region ���߷���

    /// <summary>
    /// ����¼��Ƿ��ж�����
    /// </summary>
    public static bool HasSubscribers<T>() where T : IGameEvent
    {
        return Instance != null && Instance.eventDictionary.ContainsKey(typeof(T));
    }

    /// <summary>
    /// ��ȡ�¼�����������
    /// </summary>
    public static int GetSubscriberCount<T>() where T : IGameEvent
    {
        if (Instance == null) return 0;
        return Instance.eventCounts.GetValueOrDefault(typeof(T), 0);
    }

    /// <summary>
    /// ���ָ�����͵����ж�����
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
        Debug.Log($"����¼����ж�����: {eventType.Name}");
    }

    /// <summary>
    /// ��������¼�������
    /// </summary>
    public static void ClearAllSubscribers()
    {
        if (Instance != null)
        {
            Instance.eventDictionary.Clear();
            Instance.eventCounts.Clear();
            Debug.Log("��������¼�������");
        }
    }

    #endregion

    #region ���Է���

    /// <summary>
    /// ��ӡ�����¼���Ϣ
    /// </summary>
    [ContextMenu("Print All Events")]
    public void PrintAllEvents()
    {
        Debug.Log("=== �¼�����״̬ ===");
        foreach (var kvp in eventCounts)
        {
            Debug.Log($"�¼�: {kvp.Key.Name}, ����������: {kvp.Value}");
        }
    }

    /// <summary>
    /// ��ȡ�¼�ͳ����Ϣ
    /// </summary>
    public EventCenterStats GetStats()
    {
        // �����ܶ���������
        int totalSubs = 0;
        foreach (var count in eventCounts.Values)
        {
            totalSubs += count;
        }

        // �����¼������ֵ�
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
/// �¼�����ͳ����Ϣ
/// </summary>
[System.Serializable]
public struct EventCenterStats
{
    public int totalEventTypes;
    public int totalSubscribers;
    public Dictionary<string, int> eventDetails;
}