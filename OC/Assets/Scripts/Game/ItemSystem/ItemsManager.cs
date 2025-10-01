using GameEvents;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色物品管理器
/// </summary>
public class ItemsManager : MonoBehaviour
{
    public static ItemsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    [Tooltip("道具列表")]
    private List<ItemConfiguration> items = new();

    public void Init()
    {
        items.Clear();
    }


    public void AddItem(ItemConfiguration item)
    {
        if(items.Contains(item))
        {
            foreach(var it in items)
            {
                if(it == item)
                {
                    if(it.count < it.maxStackCount)
                    {
                        it.count++;
                        it.OnGet();
                    }
                    break;
                }
            }
        }
        else
        {
            items.Add(item);
            item.count = 1;
            item.OnGet();
        }
    }

    //[SerializeField] private int coins = 0; // 金币数量

    //// 道具管理
    //private Dictionary<ItemConfiguration, int> items = new(); // 道具及其数量
    //private Dictionary<TriggerType, List<(ItemConfiguration item, MechanicEffect effect)>> effectsByTrigger = new();

    //// 组件引用
    //private Character character;
    //private BuffManager buffManager;

    //private void Awake()
    //{
    //    if(Instance != null && Instance != this)
    //    {
    //        Destroy(gameObject);
    //    }
    //    else
    //    {
    //        Instance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }

    //    // 获取必要组件
    //    character = GetComponent<Character>();
    //    buffManager = GetComponent<BuffManager>();

    //    // 初始化触发类型字典
    //    foreach (TriggerType trigger in Enum.GetValues(typeof(TriggerType)))
    //    {
    //        effectsByTrigger[trigger] = new List<(ItemConfiguration, MechanicEffect)>();
    //    }
    //}

    //private void Start()
    //{
    //    // 订阅事件
    //    EventCenter.Subscribe<GetCoinEvent, int>(Coin);
    //    //EventCenter.Subscribe<DamageTakenEvent>(OnDamageTaken);
    //    //EventCenter.Subscribe<KillEnemyEvent>(OnKill);
    //    //EventCenter.Subscribe<CooldownEndEvent>(OnCooldownEnd);
    //}

    //private void OnDestroy()
    //{
    //    EventCenter.Unsubscribe<GetCoinEvent, int>(Coin);
    //    //EventCenter.Unsubscribe<DamageTakenEvent>(OnDamageTaken);
    //    //EventCenter.Unsubscribe<KillEnemyEvent>(OnKill);
    //    //EventCenter.Unsubscribe<CooldownEndEvent>(OnCooldownEnd);
    //}

    ///// <summary>
    ///// 初始化
    ///// </summary>
    //public void Init()
    //{
    //    items.Clear();
    //    effectsByTrigger.Clear();
    //    coins = 0;
    //}

    ///// <summary>
    ///// 重置状态
    ///// </summary>
    //public void Reset()
    //{
    //    Init();
    //}

    //#region 道具管理
    ///// <summary>
    ///// 添加道具
    ///// </summary>
    //public void AddItem(ItemConfiguration item, int count = 1)
    //{
    //    if (item == null) return;

    //    // 更新道具数量
    //    if (items.ContainsKey(item))
    //    {
    //        if (item.stackable)
    //        {
    //            items[item] = Mathf.Min(items[item] + count, item.maxStackCount);
    //        }
    //    }
    //    else
    //    {
    //        items[item] = count;
    //        // 应用效果
    //        ApplyItemEffects(item);
    //    }

    //    // 注册道具效果
    //    RegisterItemEffects(item);

    //    // 如果是获得时触发的效果，立即处理
    //    //if (item.triggerType == TriggerType.OnGet)
    //    //{
    //    //    HandleEffects(TriggerType.OnGet);
    //    //}
    //}

    ///// <summary>
    ///// 移除道具
    ///// </summary>
    //public void RemoveItem(ItemConfiguration item, int count = 1)
    //{
    //    if (item == null || !items.ContainsKey(item)) return;

    //    items[item] -= count;
    //    if (items[item] <= 0)
    //    {
    //        items.Remove(item);
    //        UnregisterItemEffects(item);
    //        // 移除效果
    //        RemoveItemEffects(item);
    //    }
    //}

    ///// <summary>
    ///// 获取道具数量
    ///// </summary>
    //public int GetItemCount(ItemConfiguration item)
    //{
    //    return items.ContainsKey(item) ? items[item] : 0;
    //}

    ///// <summary>
    ///// 获取所有道具及其数量
    ///// </summary>
    //public Dictionary<ItemConfiguration, int> GetAllItems()
    //{
    //    return new Dictionary<ItemConfiguration, int>(items);
    //}
    //#endregion

    //#region 效果管理
    ///// <summary>
    ///// 注册道具效果
    ///// </summary>
    //private void RegisterItemEffects(ItemConfiguration item)
    //{
    //    //foreach (var effect in item.mechanicEffects)
    //    //{
    //    //    effectsByTrigger[effect.triggerType].Add((item, effect));
    //    //}
    //}

    ///// <summary>
    ///// 移除道具效果
    ///// </summary>
    //private void UnregisterItemEffects(ItemConfiguration item)
    //{
    //    foreach (var effectList in effectsByTrigger.Values)
    //    {
    //        effectList.RemoveAll(x => x.item == item);
    //    }
    //}

    ///// <summary>
    ///// 处理指定触发类型的效果
    ///// </summary>
    //private void HandleEffects(TriggerType triggerType)
    //{
    //    if (!effectsByTrigger.TryGetValue(triggerType, out var effects))
    //        return;

    //    foreach (var (item, effect) in effects)
    //    {
    //        ExecuteEffect(effect);
    //    }
    //}

    ///// <summary>
    ///// 执行具体效果
    ///// </summary>
    //private void ExecuteEffect(MechanicEffect effect)
    //{
    //    switch (effect.mechanicType)
    //    {
    //        case MechanicType.LifeSteal:
    //            ApplyLifeSteal(effect.parameters);
    //            break;
    //        case MechanicType.Shield:
    //            ApplyShield(effect.parameters);
    //            break;
    //        case MechanicType.CriticalHit:
    //            ApplyCriticalHit(effect.parameters);
    //            break;
    //        // ... 其他效果处理
    //    }
    //}
    //#endregion

    //#region 效果处理方法
    //private void ApplyLifeSteal(MechanicParameters parameters)
    //{
    //    if (character != null)
    //    {
    //        float healAmount = parameters.floatParam1;
    //        character.Health((int)healAmount);
    //    }
    //}

    //private void ApplyShield(MechanicParameters parameters)
    //{
    //    if (character != null)
    //    {
    //        int shieldAmount = parameters.intParam1;
    //        character.shield += shieldAmount;
    //    }
    //}

    //private void ApplyCriticalHit(MechanicParameters parameters)
    //{
    //    // 实现暴击效果
    //    float critChance = parameters.floatParam1;
    //    float critDamage = parameters.floatParam2;
    //    // ... 处理暴击逻辑
    //}
    //#endregion

    //#region 事件处理
    //private void OnDamageTaken()
    //{
    //    HandleEffects(TriggerType.OnDamageTaken);
    //}

    //private void OnKill()
    //{
    //    HandleEffects(TriggerType.OnKill);
    //}

    //private void OnCooldownEnd()
    //{
    //    HandleEffects(TriggerType.OnCooldownEnd);
    //}
    //#endregion

    //#region 金币管理
    //public void Coin(int value)
    //{
    //    int _coins = coins + value;
    //    if(_coins < 0)
    //    {
    //        return;
    //    }
    //    else
    //    {
    //        coins = _coins;
    //    }
    //}

    ///// <summary>
    ///// 直接设置金币数量
    ///// </summary>
    ///// <param name="value"></param>
    //public void SetCoins(int value)
    //{
    //    if(value < 0)
    //    {
    //        return;
    //    }
    //    coins = value;
    //}

    ///// <summary>
    ///// 金币是否足够
    ///// </summary>
    ///// <param name="value"></param>
    ///// <returns></returns>
    //public bool IsCoinsEnough(int value)
    //{
    //    return coins >= value;
    //}
    //#endregion

    //private void ApplyItemEffects(ItemConfiguration item)
    //{
    //    var character = GetComponent<Character>();
    //    if (character == null) return;



    //    //foreach (var effect in item.effects)
    //    //{
    //    //    effect.Apply(character);
    //    //    activeEffects[item].Add(effect);
    //    //}
    //}

    //private void RemoveItemEffects(ItemConfiguration item)
    //{
    //    var character = GetComponent<Character>();

    //}
}
