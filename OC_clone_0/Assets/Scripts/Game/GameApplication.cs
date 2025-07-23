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
    //[Header("����")]
    //public int currentRoleId = 0; // ��ǰ��ɫID

    public static GameApplication Instance { get; private set; }

    //public int killCount = 0;

    //// �¼���ÿ10����ɱ
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
        // ����Ƿ�������ģʽ��
        if (NetworkClient.isConnected || NetworkServer.active)
        {
            // ����ģʽ�²���Ҫ���������ɽ�ɫ����NetworkManager����
            Debug.Log("������Ϸģʽ������");
        }
        else
        {
            // ����ģʽ
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
            roleObj.name = "PlayerRole"; // ���ý�ɫ����
            await roleObj.GetComponentInChildren<Attack>().InitAsync("Bullet-Circle", 10f, 0.3f, null); // �첽�����ӵ�Ԥ����
            Camera.main.GetComponent<CameraFollow>().target = roleObj.transform; // ��������������ɫ
            EnemyManager.Instance.role = roleObj.transform; // ���õ��˹���������ҽ�ɫ
        }
        else
        {
            Debug.LogError("����roleԤ����ʧ��");
        }
        Addressables.Release(handle);
    }

    // ��ͣ��Ϸ
    public async void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        EnemyManager.Instance.SetSpawnEnabled(false);
        await UIManager.Instance.OpenPanelAsync<PausePanel>("PausePanel");
    }

    // ������Ϸ
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        UIManager.Instance.ClosePanel("PausePanel");
        EnemyManager.Instance.SetSpawnEnabled(true);
    }

    // �˳���Ϸ�����ó���/�������棩
    public async void ExitGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        UIManager.Instance.ClosePanel("PausePanel");
        
        // ���������ģʽ��ֹͣ����
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
            // ������е���
            EnemyManager.Instance.SetSpawnEnabled(false);
            EnemyManager.Instance.ClearAllEnemies();
            
            // ������
            var player = GameObject.Find("PlayerRole");
            if (player != null)
                Destroy(player);
        }
        
        // ����������
        await UIManager.Instance.OpenPanelAsync<MainPanel>("MainPanel");
    }
}
