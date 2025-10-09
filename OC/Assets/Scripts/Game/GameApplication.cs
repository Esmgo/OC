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

    [Header("游戏状态")]
    [Tooltip("游戏是否暂停")]
    private bool isPaused = false;
    [Tooltip("游戏是否正在进行中")]
    private bool isGameActive = false;

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
        Init();     //初始化自身
        DOTween.Init().SetCapacity(200, 50);        // DOTween初始化，设置初始容量
        Tools.Init();       // 工具类初始化
        UIManager.Instance.Init();      // UI管理器初始化
        ItemsManager.Instance.Init();   // 物品管理器初始化
        GlobalModificationManager.Instance.Init();      //全局增益管理器初始化
        await UIManager.Instance.OpenPanelAsync<MainPanel>("MainPanel");    // 打开主界面
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
            if (isGameActive)
            {
                if (!isPaused)
                    PauseGame();
                else
                    ResumeGame();
            }
        }
    }
    /// <summary>
    /// 开始游戏，打开游戏界面，加载角色和武器配置，
    /// </summary>
    /// <param name="roleConfig">角色配置</param>
    /// <param name="weaponConfig">武器配置</param>
    public async void GameStart(RoleConfiguration roleConfig, WeaponConfiguration weaponConfig)
    {
        // 先加载角色，等待完成后再初始化游戏
        await LoadRoleAsync(roleConfig, weaponConfig);

        await UIManager.Instance.OpenPanelAsync<FightUI>("FightUI");
        isGameActive = true;
        background.SetActive(false);
        EnemyManager.Instance.Init(testMapConfig);
        EnemyManager.Instance.StartWave();
    }

    /// <summary>
    /// 异步加载角色预制体并初始化角色
    /// </summary>
    /// <param name="roleConfig"></param>
    /// <param name="weaponConfig"></param>
    private async Task LoadRoleAsync(RoleConfiguration roleConfig, WeaponConfiguration weaponConfig)
    {       
        var loadRolePrefabTask = Tools.LoadAddressable<GameObject>(roleConfig.rolePrefabAddress);
        GameObject _roleObj = await loadRolePrefabTask;
        if (_roleObj != null)
        {
            GameObject roleObj = Instantiate(_roleObj, Vector3.zero, Quaternion.identity);
            roleObj.name = "PlayerRole";
            roleObj.GetComponent<Character>().Init(roleConfig, weaponConfig);

            // 设置摄像机跟随角色
            var cameraFollow = Camera.main.transform.parent.GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.SetTarget(roleObj.transform);
            }

            //设置工具类中的玩家角色引用
            Tools.SetCharacter(roleObj.GetComponent<Character>());
        }
        else
        {
            Debug.LogError("加载role预制体失败");
        }
    }

    // 暂停游戏
    public async void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        await UIManager.Instance.OpenPanelAsync<PausePanel>("PausePanel");
    }

    // 继续游戏
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        UIManager.Instance.ClosePanel("PausePanel");
    }

    // 退出游戏（重置场景/回主界面）
    public async void ExitGame()
    {
        background.SetActive(true);
        isPaused = false;
        isGameActive = false; 
        Time.timeScale = 1f;
        UIManager.Instance.ClosePanel("PausePanel");

            // 清除所有敌人
            if (EnemyManager.Instance != null)
            {
                EnemyManager.Instance.ClearAllEnemies();
            }

            // 清除玩家
            var player = GameObject.Find("PlayerRole");
            if (player != null)
                Destroy(player);

        // 返回主界面
        await UIManager.Instance.OpenPanelAsync<MainPanel>("MainPanel");
    }
}
