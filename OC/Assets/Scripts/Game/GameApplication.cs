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
        Init();     
        DOTween.Init().SetCapacity(200, 50);        // DOTween初始化，设置初始容量
        Tools.Init();
        UIManager.Instance.Init();      // UI管理器初始化
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
    /// 由选角色界面转到开始游戏，加载角色和武器配置，
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
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(roleConfig.rolePrefabAddress);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject roleObj = Instantiate(handle.Result, Vector3.zero, Quaternion.identity);
            roleObj.name = "PlayerRole";
            roleObj.GetComponent<Character>().Init(roleConfig, weaponConfig);

            // 设置摄像机跟随角色
            var cameraFollow = Camera.main.transform.parent.GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.target = roleObj.transform;
            }

            // 设置敌人管理器的玩家角色
            if (EnemyManager.Instance != null)
            {
                EnemyManager.Instance.role = roleObj.transform;
            }
        }
        else
        {
            Debug.LogError("加载role预制体失败");
        }
        Addressables.Release(handle);
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
