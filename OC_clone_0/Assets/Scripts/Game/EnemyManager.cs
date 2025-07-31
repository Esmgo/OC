using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [Header("生成参数")]
    public GameObject[] enemyPrefabs; // 敌人预制体数组，支持多种类型
    public int maxEnemyCount = 50; // 敌人最大数量
    public float spawnInterval = 1.5f; // 生成间隔（秒）
    public float minSpawnRadius = 5f; // 最小生成半径（玩家为中心）
    public float maxSpawnRadius = 12f; // 最大生成半径（玩家为中心）
    public float spawnDelay = 2f; // 开始生成延迟（秒）
    public Transform role; // 玩家对象

    private List<GameObject> enemyPool = new List<GameObject>();
    private int activeEnemyCount = 0;
    private float timer = 0f;
    private float spawnDelayTimer = 0f;
    private bool delayFinished = false;

    [Header("生成控制")]
    public bool spawnEnabled = true; // 敌人生成开关
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

        // 可选：自动查找玩家
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

        // 延迟处理
        if (!delayFinished)
        {
            spawnDelayTimer += Time.deltaTime;
            if (spawnDelayTimer >= spawnDelay)
            {
                delayFinished = true;
            }
            return;
        }

        // 定时生成敌人
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
        
        // 添加池助手组件
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
        
        // 随机选择一种敌人类型
        int prefabIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject selectedPrefab = enemyPrefabs[prefabIndex];

        // 查找未激活且类型匹配的敌人
        foreach (var e in enemyPool)
        {
            if (!e.activeInHierarchy && e.name.StartsWith(selectedPrefab.name))
                return e;
        }
        
        // 池中无可用敌人，创建新对象
        GameObject newEnemy = Instantiate(selectedPrefab);
        newEnemy.name = selectedPrefab.name + "_Pooled"; // 保证名称匹配
        newEnemy.SetActive(false);
        
        // 添加池助手组件
        EnemyPoolHelper helper = newEnemy.AddComponent<EnemyPoolHelper>();
        helper.manager = this;
        
        enemyPool.Add(newEnemy);
        return newEnemy;
    }

    // 敌人回收时调用
    public void RecycleEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        activeEnemyCount = Mathf.Max(0, activeEnemyCount - 1);
    }

    // 清除所有敌人
    public void ClearAllEnemies()
    {
        // 清除场景中的敌人
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        // 清除池中的敌人
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

    // 启用/禁用敌人生成
    public void SetSpawnEnabled(bool enabled)
    {
        spawnEnabled = enabled;
    }

    // 获取当前活跃敌人数量
    public int GetActiveEnemyCount()
    {
        return activeEnemyCount;
    }

    // 手动添加敌人到池中
    public void AddEnemyToPool(GameObject enemy)
    {
        if (!enemyPool.Contains(enemy))
        {
            enemyPool.Add(enemy);
            
            // 确保有池助手组件
            EnemyPoolHelper helper = enemy.GetComponent<EnemyPoolHelper>();
            if (helper == null)
            {
                helper = enemy.AddComponent<EnemyPoolHelper>();
            }
            helper.manager = this;
        }
    }
}

// 辅助组件：在敌人被禁用时通知管理器回收
public class EnemyPoolHelper : MonoBehaviour
{
    [HideInInspector] public EnemyManager manager;
    
    void OnDisable()
    {
        if (manager != null)
            manager.RecycleEnemy(gameObject);
    }
}
