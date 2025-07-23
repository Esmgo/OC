using DG.Tweening;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameApplication : MonoBehaviour
{
    //[Header("测试")]
    //public int currentRoleId = 0; // 当前角色ID

    public static GameApplication Instance { get; private set; }

    //public int killCount = 0;

    //// 事件：每10个击杀
    //public delegate void KillMilestoneHandler(int milestone);
    //public event KillMilestoneHandler OnKillMilestone;

    private bool isPaused = false;

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
        DOTween.Init().SetCapacity(200, 50);
        UIManager.Instance.Init();
        await UIManager.Instance.OpenPanelAsync<MainPanel>("MainPanel");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();
        }
    }

    //public void AddKill()
    //{
    //    killCount++;
    //    if (killCount % 10 == 0)
    //    {
    //        OnKillMilestone?.Invoke(killCount / 10);
    //    }
    //}

    public void GameStart()
    {
        // 检测是否在网络模式下
        if (NetworkClient.isConnected || NetworkServer.active)
        {
            // 网络模式下不需要在这里生成角色，由NetworkManager处理
            Debug.Log("网络游戏模式已启动");
        }
        else
        {
            // 本地模式
            LoadRole();
            EnemyManager.Instance.SetSpawnEnabled(true);
        }
    }

    public async Task OpenNetworkPanel()
    {
        // Fix for CS0311: Ensure NetworkPanel inherits from UIPanel
        await UIManager.Instance.OpenPanelAsync<UIPanel>("NetworkPanel");
    }

    public async void LoadRole()
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>("Role");
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject roleObj = Instantiate(handle.Result, Vector3.zero, Quaternion.identity);
            roleObj.name = "PlayerRole"; // 设置角色名称
            await roleObj.GetComponentInChildren<Attack>().InitAsync("Bullet-Circle", 10f, 0.3f, null); // 异步加载子弹预制体
            Camera.main.GetComponent<CameraFollow>().target = roleObj.transform; // 设置摄像机跟随角色
            EnemyManager.Instance.role = roleObj.transform; // 设置敌人管理器的玩家角色
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
        EnemyManager.Instance.SetSpawnEnabled(false);
        await UIManager.Instance.OpenPanelAsync<PausePanel>("PausePanel");
    }

    // 继续游戏
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        UIManager.Instance.ClosePanel("PausePanel");
        EnemyManager.Instance.SetSpawnEnabled(true);
    }

    // 退出游戏（重置场景/回主界面）
    public async void ExitGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        UIManager.Instance.ClosePanel("PausePanel");
        
        // 如果在网络模式，停止网络
        if (NetworkClient.isConnected || NetworkServer.active)
        {
            var networkManager = FindObjectOfType<GameNetworkManager>();
            if (networkManager != null)
            {
                networkManager.StopNetwork();
            }
        }
        else
        {
            // 清除所有敌人
            EnemyManager.Instance.SetSpawnEnabled(false);
            EnemyManager.Instance.ClearAllEnemies();
            
            // 清除玩家
            var player = GameObject.Find("PlayerRole");
            if (player != null)
                Destroy(player);
        }
        
        // 返回主界面
        await UIManager.Instance.OpenPanelAsync<MainPanel>("MainPanel");
    }
}
