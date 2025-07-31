using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
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
    public bool spawnEnabled = true; // �������ɿ���
    //private float lastSpawnTime = 0f;

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
            var moveObj = FindObjectOfType<MoveBase>();
            if (moveObj != null)
                role = moveObj.transform;
        }
    }

    void Start()
    {
        spawnDelayTimer = 0f;
        delayFinished = false;
        timer = 0f;
    }

    void Update()
    {
        if (!spawnEnabled || role == null) return;

        // �ӳٴ���
        if (!delayFinished)
        {
            spawnDelayTimer += Time.deltaTime;
            if (spawnDelayTimer >= spawnDelay)
            {
                delayFinished = true;
            }
            return;
        }

        // ��ʱ���ɵ���
        timer += Time.deltaTime;
        if (timer >= spawnInterval && activeEnemyCount < maxEnemyCount)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0) return;

        Vector2 spawnPosition = GetRandomPositionAroundPlayer();
        GameObject enemy = GetEnemyFromPool(spawnPosition);
        
        if (enemy != null)
        {
            enemy.transform.position = spawnPosition;
            enemy.SetActive(true);
            activeEnemyCount++;
        }
    }

    private void SpawnEnemyLocally()
    {
        if (enemyPrefabs.Length == 0) return;
        
        Vector3 spawnPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f);
        GameObject newEnemy = Instantiate(enemyPrefabs[0], spawnPosition, Quaternion.identity, transform);
        
        // ��ӳ��������
        if (newEnemy.GetComponent<EnemyPoolHelper>() == null)
        {
            newEnemy.AddComponent<EnemyPoolHelper>().manager = this;
        }
        
        activeEnemyCount++;
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
        if (enemyPrefabs.Length == 0) return null;
        
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
        
        // ��ӳ��������
        EnemyPoolHelper helper = newEnemy.AddComponent<EnemyPoolHelper>();
        helper.manager = this;
        
        enemyPool.Add(newEnemy);
        return newEnemy;
    }

    // ���˻���ʱ����
    public void RecycleEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        activeEnemyCount = Mathf.Max(0, activeEnemyCount - 1);
    }

    // ������е���
    public void ClearAllEnemies()
    {
        // ��������еĵ���
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        // ������еĵ���
        foreach (var enemy in enemyPool)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        
        enemyPool.Clear();
        activeEnemyCount = 0;
        timer = 0f;
        spawnDelayTimer = 0f;
        delayFinished = false;
    }

    // ����/���õ�������
    public void SetSpawnEnabled(bool enabled)
    {
        spawnEnabled = enabled;
    }

    // ��ȡ��ǰ��Ծ��������
    public int GetActiveEnemyCount()
    {
        return activeEnemyCount;
    }

    // �ֶ���ӵ��˵�����
    public void AddEnemyToPool(GameObject enemy)
    {
        if (!enemyPool.Contains(enemy))
        {
            enemyPool.Add(enemy);
            
            // ȷ���г��������
            EnemyPoolHelper helper = enemy.GetComponent<EnemyPoolHelper>();
            if (helper == null)
            {
                helper = enemy.AddComponent<EnemyPoolHelper>();
            }
            helper.manager = this;
        }
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
