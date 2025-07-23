using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemyManager : NetworkBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [Header("���ɲ���")]
    public GameObject[] enemyPrefabs; // ����Ԥ�������飬֧�ֶ�������
    public int maxEnemyCount = 50; // �����������
    public float spawnInterval = 1.5f; // ���ɼ�����룩
    public float minSpawnRadius = 5f; // ��С���ɰ뾶�����Ϊ���ģ�
    public float maxSpawnRadius = 12f; // ������ɰ뾶�����Ϊ���ģ�
    public float spawnDelay = 2f; // ��ʼ�����ӳ٣��룩
    public Transform role; // ��Ҷ���

    private List<GameObject> enemyPool = new List<GameObject>();
    private int activeEnemyCount = 0;
    private float timer = 0f;
    private float spawnDelayTimer = 0f;
    private bool delayFinished = false;

    [Header("���ɿ���")]
    public bool spawnEnabled = false; // �������ɿ���

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // ��ѡ���Զ��������
        if (role == null)
        {
            var moveObj = FindObjectOfType<Move>();
            if (moveObj != null)
                role = moveObj.transform;
        }
    }

    void Update()
    {
        if (GameNetworkManager.IsLANGame && isServer && spawnEnabled)
        {
            if (transform.childCount < maxEnemyCount)
            {
                SpawnEnemy();
            }
        }
        else if (!GameNetworkManager.IsLANGame && spawnEnabled)
        {
            if (transform.childCount < maxEnemyCount)
            {
                SpawnEnemyLocally();
            }
        }
    }

    /// <summary>
    /// ���õ�������״̬
    /// </summary>
    /// <param name="enabled"></param>
    [Server]
    public void SetSpawnEnabled(bool enabled)
    {
        spawnEnabled = enabled;
        if (enabled)
        {
            delayFinished = false;
            spawnDelayTimer = 0f;
            timer = 0f;
        }
    }

    /// <summary>
    /// �л�����״̬
    /// </summary>
    public void ToggleSpawn()
    {
        SetSpawnEnabled(!spawnEnabled);
    }

    [Server]
    private void SpawnEnemy()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f);
        GameObject enemy = Instantiate(enemyPrefabs[0], spawnPosition, Quaternion.identity, transform);
        NetworkServer.Spawn(enemy); // ͬ�����˵����пͻ���
    }

    private void SpawnEnemyLocally()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f);
        Instantiate(enemyPrefabs[0], spawnPosition, Quaternion.identity, transform);
    }

    Vector2 GetRandomPositionAroundPlayer()
    {
        if (role == null) return Vector2.zero;
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius = Random.Range(minSpawnRadius, maxSpawnRadius);
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        return (Vector2)role.position + offset;
    }

    GameObject GetEnemyFromPool(Vector2 spawnPos)
    {
        // ���ѡ��һ�ֵ�������
        int prefabIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject selectedPrefab = enemyPrefabs[prefabIndex];

        // ����δ����������ƥ��ĵ���
        foreach (var e in enemyPool)
        {
            if (!e.activeInHierarchy && e.name.StartsWith(selectedPrefab.name))
                return e;
        }
        // �����޿��õ��ˣ������¶���
        GameObject newEnemy = Instantiate(selectedPrefab);
        newEnemy.name = selectedPrefab.name + "_Pooled"; // ��֤����ƥ��
        newEnemy.SetActive(false);
        newEnemy.AddComponent<EnemyPoolHelper>().manager = this;
        enemyPool.Add(newEnemy);
        return newEnemy;
    }

    // ���˻���ʱ����
    public void RecycleEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        activeEnemyCount = Mathf.Max(0, activeEnemyCount - 1);
    }

    // ������������е���
    public void ClearAllEnemies()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        activeEnemyCount = 0;
    }
}

// ����������ڵ��˱�����ʱ֪ͨ����������
public class EnemyPoolHelper : MonoBehaviour
{
    [HideInInspector] public EnemyManager manager;
    void OnDisable()
    {
        if (manager != null)
            manager.RecycleEnemy(gameObject);
    }
}
