using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("生成参数")]
    public GameObject[] enemyPrefabs; // 敌人预制体数组，支持多种类型
    public int maxEnemyCount = 50; // 敌人最大数量
    public float spawnInterval = 1.5f; // 生成间隔（秒）
    public float spawnRadius = 8f; // 生成半径（以玩家为中心）
    public Transform player; // 玩家对象

    private List<GameObject> enemyPool = new List<GameObject>();
    private int activeEnemyCount = 0;
    private float timer = 0f;

    void Awake()
    {
        // 可选：自动查找玩家
        if (player == null)
        {
            var moveObj = FindObjectOfType<Move>();
            if (moveObj != null)
                player = moveObj.transform;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval && activeEnemyCount < maxEnemyCount)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        Vector2 spawnPos = GetRandomPositionAroundPlayer();
        GameObject enemy = GetEnemyFromPool(spawnPos);
        enemy.transform.position = spawnPos;
        enemy.SetActive(true);
        activeEnemyCount++;
    }

    Vector2 GetRandomPositionAroundPlayer()
    {
        if (player == null) return Vector2.zero;
        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
        return (Vector2)player.position + offset;
    }

    GameObject GetEnemyFromPool(Vector2 spawnPos)
    {
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
        newEnemy.AddComponent<EnemyPoolHelper>().manager = this;
        enemyPool.Add(newEnemy);
        return newEnemy;
    }

    // 敌人回收时调用
    public void RecycleEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        activeEnemyCount = Mathf.Max(0, activeEnemyCount - 1);
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
