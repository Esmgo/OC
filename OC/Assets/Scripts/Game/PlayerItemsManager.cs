using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色物品管理器
/// </summary>
public class PlayerItemsManager : MonoBehaviour
{
    public static PlayerItemsManager Instance { get; private set; }

    [SerializeField] private int coins = 0; // 金币数量


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        EventCenter.Subscribe<GetCoinEvent, int>(Coin);
    }

    private void OnDestroy()
    {
        EventCenter.Unsubscribe<GetCoinEvent, int>(Coin);
    }

    public void Init()
    {

    }

    public void Reset()
    {
        coins = 0;
    }

    /// <summary>
    /// 硬币数量操作
    /// </summary>
    /// <param name="value"></param>
    public void Coin(int value)
    {
        int _coins = coins + value;
        if(_coins < 0)
        {
            return;
        }
        else
        {
            coins = _coins;
        }
    }

    /// <summary>
    /// 直接设置硬币数量（正数）
    /// </summary>
    /// <param name="value"></param>
    public void SetCoins(int value)
    {
        if(value < 0)
        {
            return;
        }
        coins = value;
    }

    /// <summary>
    /// 硬币是否足够
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool IsCoinsEnough(int value)
    {
        return coins >= value;
    }
}
