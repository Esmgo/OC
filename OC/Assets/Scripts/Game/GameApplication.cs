using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameApplication : MonoBehaviour
{
    [Title("����")]
    public int currentRoleId = 0; // ��ǰ��ɫID

    public static GameApplication Instance { get; private set; }

    public int killCount = 0;

    // �¼���ÿ10����ɱ
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
            roleObj.name = "PlayerRole"; // ���ý�ɫ����
            await roleObj.GetComponentInChildren<Attack>().InitAsync("Bullet-Circle", 10f, 0.3f, null); // �첽�����ӵ�Ԥ����
            Camera.main.GetComponent<CameraFollow>().target = roleObj.transform; // ��������������ɫ
            EnemyManager.Instance.role = roleObj.transform; // ���õ��˹���������ҽ�ɫ
        }
        else
        {
            Debug.LogError("����roleԤ����ʧ��");
        }
        // �粻����Ҫ���ͷ���Դ
        Addressables.Release(handle);
    }
}
