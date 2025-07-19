using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameApplication : MonoBehaviour
{
    [Title("测试")]
    public int currentRoleId = 0; // 当前角色ID

    public static GameApplication Instance { get; private set; }

    public int killCount = 0;

    // 事件：每10个击杀
    public delegate void KillMilestoneHandler(int milestone);
    public event KillMilestoneHandler OnKillMilestone;

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

    public void AddKill()
    {
        killCount++;
        if (killCount % 10 == 0)
        {
            OnKillMilestone?.Invoke(killCount / 10);
        }
    }

    public void GameStart()
    {
        LoadRole();
        EnemyManager.Instance.SetSpawnEnabled(true);
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
        // 如不再需要可释放资源
        Addressables.Release(handle);
    }
}
