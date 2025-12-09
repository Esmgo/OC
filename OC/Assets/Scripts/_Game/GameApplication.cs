using DG.Tweening;
using GameEvents;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static Mirror.BouncyCastle.Math.EC.ECCurve;

/// <summary>
/// 管理整个游戏进程
/// </summary>
public class GameApplication : MonoBehaviour
{
    public static GameApplication Instance { get; private set; }

    [Header("测试用")]
    [Tooltip("测试地图配置")]
    public MapConfiguration testMapConfig;

    [Tooltip("背景那个大图片")]
    private GameObject background;

    private void Awake()
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

    private async void Start()
    {
        Init();                                     //初始化自身
        DOTween.Init().SetCapacity(200, 50);        // DOTween初始化，设置初始容量
        Tools.Init();                               // 工具类初始化
        ResourceManager.Instance.Init();            // 资源管理器初始化加载资源
        CameraManager.Instance.Init();              // 相机管理器初始化
        UIManager.Instance.Init();                  // UI管理器初始化
        ObjectPoolManager.Instance.Init();         // 对象池管理器初始化
        ItemManager.Instance.Init();                // 物品管理器初始化
        await UIManager.Instance.OpenPanelAsync<MainPanel>("MainPanel");    // 打开主界面
        //await UIManager.Instance.OpenPanelAsync<StartPanel>("StartPanel");  // 打开开始界面
    }

    /// <summary>
    /// 自身初始化
    /// </summary>
    private void Init()
    {
        background = GameObject.Find("Background");
    }

    private void Update()
    {
        // 按下Esc键暂停或继续游戏(退出有bug,先别用)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameStateManager.Instance.IsFighting)
            {
                if (!GameStateManager.Instance.IsPaused)
                    GameStateManager.Instance.PauseGame();
                else
                    GameStateManager.Instance.ResumeGame();
            }
        }
    }
    /// <summary>
    /// 开始游戏，打开游戏界面，加载角色和武器配置，
    /// </summary>
    /// <param name="roleConfig">角色配置</param>
    public async void GameStart()
    {
        //Time.timeScale = 0.1f;
        await CharacterManager.Instance.StartGame();
        await UIManager.Instance.OpenPanelAsync<FightUI>("FightUI");
        background.SetActive(false);
        EnemyManager.Instance.Init(testMapConfig);
        EnemyManager.Instance.StartNextWave();
    }
}
