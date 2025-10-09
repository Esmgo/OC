using GameEvents;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [Header("生成参数")]
    public int maxEnemyCount = 50; // 敌人最大数量
    public float minSpawnRadius = 5f; // 最小生成半径（玩家为中心）
    public float maxSpawnRadius = 12f; // 最大生成半径（玩家为中心）

    public Transform character => Tools.GetCharacter().transform; // 玩家对象

    private MapConfiguration currentMapConfig; // 当前地图配置

    private List<GameObject> enemyPrefabs = new(); // 敌人预制体数组


    [Header("对象池配置")]
    public int poolSizePerEnemyType = 20; // 每种敌人类型的池大小

    public int activeEnemyCount = 0;
    private float timer = 0f;
    private float spawnDelayTimer = 0f;
    private bool delayFinished = false;

    [Header("生成控制")]
    [SerializeField]private float spawnDelay = 2f; // 开始生成延迟（秒）
    [SerializeField]private float spawnInterval = 2f; // 生成间隔（秒）
    public float waveTimer = 0f; // 用于波次生成的计时器
    private int currentWaveIndex = 0; // 当前波次索引
    public bool spawnEnabled = true; // 敌人生成开关
    private bool waveEnded = false; // 添加波次结束标志
    [SerializeField]private int waveTime = 10;//每波持续时间，单位秒

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

        //// 可选：自动查找玩家
        //if (character == null)
        //{
        //    var moveObj = FindObjectOfType<MoveBase>();
        //    if (moveObj != null)
        //        character = moveObj.transform;
        //}
    }

    private void OnEnable()
    {
        
    }

    private void OnDestroy()
    {
        EventCenter.Unsubscribe<EnemyDiedEvent>(OnEnemyDead);
    }

    public void Init(MapConfiguration mapConfig)
    {
        currentMapConfig = mapConfig;

        EventCenter.Subscribe<EnemyDiedEvent>(OnEnemyDead);

        currentWaveIndex = 0;
        SetSpawnEnabled(false);
        ResetWaveState();
    }

    /// <summary>
    /// 重置波次状态，用于开始新的波次
    /// </summary>
    public void ResetWaveState()
    {
        waveTimer = 0f;
        waveEnded = false;
        timer = 0f;
        spawnDelayTimer = 0f;
        delayFinished = false;
    }

    /// <summary>
    /// 初始化敌人对象池
    /// </summary>
    private void InitializeEnemyPools()
    {
        if (ObjectPoolManager.Instance == null)
        {
            Debug.LogError("ObjectPoolManager实例未找到！");
            return;
        }

        // 为每种敌人类型创建对象池
        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            if (enemyPrefabs[i] != null)
            {
                string poolName = enemyPrefabs[i].name;
                ObjectPoolManager.Instance.GetOrCreatePool(poolName, enemyPrefabs[i], poolSizePerEnemyType);
                Debug.Log($"创建敌人池: {poolName}");
            }
        }
    }

    /// <summary>
    /// 获取敌人池名称
    /// </summary>
    private string GetEnemyPoolName(int enemyTypeIndex)
    {
        return enemyPrefabs[enemyTypeIndex].name;
    }

    void Update()
    {
        if (!spawnEnabled || character == null) return;

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

        if (waveTimer < waveTime)
        {
            waveTimer += Time.deltaTime;
        }
        else if (!waveEnded)
        {
            waveEnded = true;
            StartCoroutine(EndWaveAndOpenShop());
        }
    }

    /// <summary>
    /// 结束当前波次并打开商店
    /// </summary>
    private IEnumerator EndWaveAndOpenShop()
    {
        //清除所有敌人
        ClearAllEnemies();
        
        //停止敌人生成
        SetSpawnEnabled(false);

        ResetWaveState();

        EventCenter.Publish<WaveCompletedEvent>();

        yield return null;
        //UIManager.Instance.ClosePanel("FightUI");

        ////异步打开商店UI
        //var shopTask = UIManager.Instance.OpenPanelAsync<ShopUI>("ShopUI");
        //yield return new WaitUntil(() => shopTask.IsCompleted);

        //if (shopTask.Exception != null)
        //{
        //    Debug.LogError($"打开商店失败: {shopTask.Exception}");
        //}
        //else
        //{
        //    shopTask.Result?.OnOpen();
        //    Debug.Log("商店已打开");
        //}
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs.Count == 0) return;

        Vector2 spawnPosition = GetRandomPositionAroundPlayer();
        
        // 随机选择一种敌人类型
        int enemyTypeIndex = Random.Range(0, enemyPrefabs.Count);
        string poolName = GetEnemyPoolName(enemyTypeIndex);
        
        // 从对象池获取敌人
        GameObject enemy = ObjectPoolManager.Instance.GetObject(poolName, spawnPosition, Quaternion.identity);
        
        if (enemy != null)
        {
            activeEnemyCount++;
        }
        else
        {
            Debug.LogWarning($"无法从池 {poolName} 获取敌人对象");
        }
    }

    /// <summary>
    /// 获取玩家周围的随机位置
    /// </summary>
    /// <returns></returns>
    private Vector2 GetRandomPositionAroundPlayer()
    {
        if (character == null) return Vector2.zero;
        
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius = Random.Range(minSpawnRadius, maxSpawnRadius);
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        return (Vector2)character.position + offset;
    }

    /// <summary>
    /// 回收所有敌人到对象池（简化版本）
    /// </summary>
    public void ClearAllEnemies()
    {
        if (ObjectPoolManager.Instance != null)
        {
            // 使用新的回收方法，更高效
            foreach (var prefab in enemyPrefabs)
            {
                if (prefab != null)
                {
                    string poolName = prefab.name;
                    ObjectPoolManager.Instance.RecycleAllActiveObjects(poolName);
                }
            }
        }
        
        activeEnemyCount = 0;
        timer = 0f;
        spawnDelayTimer = 0f;
        delayFinished = false;
        Debug.Log("回收所有敌人到对象池");
    }

    /// <summary>
    /// 强制清除所有敌人（用于紧急情况）
    /// </summary>
    public void ForceClearAllEnemies()
    {
        if (ObjectPoolManager.Instance != null)
        {
            foreach (var prefab in enemyPrefabs)
            {
                if (prefab != null)
                {
                    string poolName = prefab.name;
                    ObjectPoolManager.Instance.ForceRecycleAllActiveObjects(poolName);
                }
            }
        }
        
        activeEnemyCount = 0;
        timer = 0f;
        spawnDelayTimer = 0f;
        delayFinished = false;
        Debug.Log("强制回收所有敌人到对象池");
    }

    /// <summary>
    /// 敌人死亡触发
    /// </summary>
    private void OnEnemyDead()
    {
        activeEnemyCount--;
    }

    /// <summary>
    /// 启用/禁用敌人生成
    /// </summary>
    public void SetSpawnEnabled(bool enabled)
    {
        spawnEnabled = enabled;
    }

    /// <summary>
    /// 开始新的一波敌人生成
    /// </summary>
    /// <param name="mapConfig"></param>
    public void StartWave()
    {
        if(currentWaveIndex < currentMapConfig.waveConfigurations.Count)
        {
            ResetWaveState();
            ClearAllEnemies();
            enemyPrefabs = currentMapConfig.waveConfigurations[currentWaveIndex].enemyPrefabs;
            InitializeEnemyPools();
            currentWaveIndex++;
        }
        else
        {
            Debug.LogWarning("没有更多波次可供生成！");
        }
        SetSpawnEnabled(true);
        EventCenter.Publish<WaveStartEvent>();
        Debug.Log($"开始波次{currentWaveIndex}");
    }

    /// <summary>
    /// 获取当前活跃敌人数量
    /// </summary>
    public int GetActiveEnemyCount()
    {
        return activeEnemyCount;
    }

    /// <summary>
    /// 获取指定类型敌人池的统计信息
    /// </summary>
    public PoolStats GetEnemyPoolStats(int enemyTypeIndex)
    {
        if (enemyTypeIndex < 0 || enemyTypeIndex >= enemyPrefabs.Count)
            return new PoolStats();

        string poolName = GetEnemyPoolName(enemyTypeIndex);
        return ObjectPoolManager.Instance.GetPoolStats(poolName);
    }

    /// <summary>
    /// 打印所有敌人池的统计信息
    /// </summary>
    [ContextMenu("Print Enemy Pool Stats")]
    public void PrintEnemyPoolStats()
    {
        Debug.Log("=== 敌人池统计信息 ===");
        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            if (enemyPrefabs[i] != null)
            {
                var stats = GetEnemyPoolStats(i);
                string poolName = GetEnemyPoolName(i);
                Debug.Log($"池: {poolName} - 总数: {stats.totalObjects}, 活跃: {stats.activeObjects}, 可用: {stats.availableObjects}");
            }
        }
        Debug.Log($"总活跃敌人数: {activeEnemyCount}");
    }
}
