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
    //[Header("游戏状态")]
    //public int currentLevel = 1;
    //public int playerScore = 0;
    //public int killCount = 0;

    public static GameApplication Instance { get; private set; }

    private bool isPaused = false;
    private bool isGameActive = false;

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
            if (isGameActive)
            {
                if (!isPaused)
                    PauseGame();
                else
                    ResumeGame();
            }
        }
    }

    public void GameStart(RoleConfiguration roleConfig, WeaponConfiguration weaponConfig)
    {
        LoadRole(roleConfig, weaponConfig);
        isGameActive = true;
    }

    public async void LoadRole(RoleConfiguration roleConfig, WeaponConfiguration weaponConfig)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>("Role");
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject roleObj = Instantiate(handle.Result, Vector3.zero, Quaternion.identity);
            roleObj.name = "PlayerRole";
            roleObj.GetComponent<Character>().Init(roleConfig, weaponConfig);

            // 异步加载子弹预制体
            var attackComponent = roleObj.GetComponentInChildren<Attack>();
            if (attackComponent != null)
            {
                await attackComponent.InitAsync("Bullet-Circle", 10f, 0.3f, null);
            }

            // 设置摄像机跟随角色
            var cameraFollow = Camera.main.GetComponent<CameraFollow>();
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
