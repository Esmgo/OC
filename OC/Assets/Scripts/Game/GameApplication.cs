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
    //[Header("��Ϸ״̬")]
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

            // �첽�����ӵ�Ԥ����
            var attackComponent = roleObj.GetComponentInChildren<Attack>();
            if (attackComponent != null)
            {
                await attackComponent.InitAsync("Bullet-Circle", 10f, 0.3f, null);
            }

            // ��������������ɫ
            var cameraFollow = Camera.main.GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.target = roleObj.transform;
            }

            // ���õ��˹���������ҽ�ɫ
            if (EnemyManager.Instance != null)
            {
                EnemyManager.Instance.role = roleObj.transform;
            }
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
        await UIManager.Instance.OpenPanelAsync<PausePanel>("PausePanel");
    }

    // ������Ϸ
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        UIManager.Instance.ClosePanel("PausePanel");
    }

    // �˳���Ϸ�����ó���/�������棩
    public async void ExitGame()
    {
        isPaused = false;
        isGameActive = false;
        Time.timeScale = 1f;
        UIManager.Instance.ClosePanel("PausePanel");

            // ������е���
            if (EnemyManager.Instance != null)
            {
                EnemyManager.Instance.ClearAllEnemies();
            }

            // ������
            var player = GameObject.Find("PlayerRole");
            if (player != null)
                Destroy(player);

        // ����������
        await UIManager.Instance.OpenPanelAsync<MainPanel>("MainPanel");
    }
}
