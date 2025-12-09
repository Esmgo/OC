using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways] // 确保在编辑器模式下也能运行 OnDrawGizmos
public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [Header("全局生成参数")]
    [Tooltip("场景中允许存在的最大敌人数量")]
    public int maxEnemyCount = 50;
    [Tooltip("每次集中生成的敌人数量")]
    public int enemiesPerCluster = 3;
    [Tooltip("集中生成点周围的半径")]
    public float clusterRadius = 3f;

    [Header("全局生成范围")]
    [Tooltip("敌人生成的中心区域")]
    public Vector2 globalSpawnCenter = Vector2.zero;
    [Tooltip("敌人生成区域的最大半径")]
    public float globalSpawnRadius = 20f;

    [Header("波次控制")]
    [Tooltip("每波开始时的生成延迟")]
    [SerializeField] private float initialSpawnDelay = 2f;
    [Tooltip("波次中每次生成的间隔")]
    [SerializeField] private float spawnInterval = 2f;
    [Tooltip("每波的持续时间（秒）")]
    [SerializeField] private int waveTime = 60;

    // 内部状态
    public float waveTimer { get; private set; }

    private MapConfiguration currentMapConfig;
    private List<EnemyConfiguration> currentEnemyConfigs = new();
    private int activeEnemyCount = 0;
    private int currentWaveIndex = 0;
    
    private float spawnTimer = 0f;
    private bool isWaveActive = false;

    #region Unity生命周期
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
        }
    }

    void OnEnable()
    {
        EventCenter.Subscribe<EnemyDiedEvent>(OnEnemyDied);
    }

    void OnDisable()
    {
        EventCenter.Unsubscribe<EnemyDiedEvent>(OnEnemyDied);
    }

    void Update()
    {
        if (!isWaveActive) return;

        HandleWaveTimer();
        HandleEnemySpawning();
    }
    #endregion

    #region 公共方法
    /// <summary>
    /// 初始化敌人管理器并准备第一波
    /// </summary>
    public void Init(MapConfiguration mapConfig)
    {
        currentMapConfig = mapConfig;
        currentWaveIndex = 0;
        activeEnemyCount = 0;
        isWaveActive = false;
    }

    /// <summary>
    /// 开始新的一波
    /// </summary>
    public void StartNextWave()
    {
        if (currentMapConfig == null || currentWaveIndex >= currentMapConfig.waveConfigurations.Count)
        {
            Debug.LogWarning("所有波次已完成或地图配置无效！");
            // 在此可以触发游戏胜利等逻辑
            return;
        }

        // 清理上一波的敌人
        ClearAllEnemies();

        // 设置当前波的敌人类型
        currentEnemyConfigs = currentMapConfig.waveConfigurations[currentWaveIndex].enemyConfigs;

        // 重置计时器和状态
        waveTimer = waveTime;
        spawnTimer = initialSpawnDelay; // 应用初始延迟
        isWaveActive = true;

        Debug.Log($"第 {currentWaveIndex + 1} 波开始！");

        currentWaveIndex++;
    }

    /// <summary>
    /// 回收所有当前活跃的敌人
    /// </summary>
    public void ClearAllEnemies()
    {
        if (ObjectPoolManager.Instance != null)
        {
            foreach (var config in currentEnemyConfigs)
            {
                ObjectPoolManager.Instance.RecycleAllActiveObjects(config.prefabAddress);
            }
        }
        activeEnemyCount = 0;
        Debug.Log("已回收所有活跃敌人。");
    }
    #endregion

    #region 内部逻辑
    /// <summary>
    /// 处理波次计时
    /// </summary>
    private void HandleWaveTimer()
    {
        waveTimer -= Time.deltaTime;
        if (waveTimer <= 0)
        {
            isWaveActive = false;
            StartCoroutine(EndWaveSequence());
        }
    }

    /// <summary>
    /// 处理敌人生成计时和逻辑
    /// </summary>
    private void HandleEnemySpawning()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0 && activeEnemyCount < maxEnemyCount)
        {
            SpawnEnemyCluster();
            spawnTimer = spawnInterval; // 重置为常规生成间隔
        }
    }

    /// <summary>
    /// 波次结束后的处理序列
    /// </summary>
    private IEnumerator EndWaveSequence()
    {
        Debug.Log($"第 {currentWaveIndex} 波结束。");
        ClearAllEnemies();

        // 此处可以添加打开商店等逻辑
         yield return UIManager.Instance.OpenPanelAsync<ShopUI>("ShopUI");
        //yield return new WaitForSeconds(1f); // 等待1秒，准备下一波
        
        // 示例：自动开始下一波
        // StartNextWave(); 
    }

    /// <summary>
    /// 在指定区域内生成一簇敌人
    /// </summary>
    private void SpawnEnemyCluster()
    {
        if (currentEnemyConfigs.Count == 0) return;

        Vector2 clusterCenter = GetRandomPositionInGlobalRange();

        for (int i = 0; i < enemiesPerCluster; i++)
        {
            if (activeEnemyCount >= maxEnemyCount) break;

            Vector2 spawnPosition = clusterCenter + Random.insideUnitCircle * clusterRadius;

            SpawnEnemy(currentEnemyConfigs[Random.Range(0, currentEnemyConfigs.Count)], spawnPosition);
        }
    }

    /// <summary>
    /// 使用EntityFactory异步生成单个敌人
    /// </summary>
    private async void SpawnEnemy(EnemyConfiguration config, Vector2 spawnPosition)
    {
        GameObject enemy = await EntityFactory.Instance.CreatEntityAsync(config.prefabAddress, true);
        enemy.transform.position = spawnPosition;
        enemy.GetComponent<EnemyComponent>().Init(config);
        if (enemy != null)
        {
            activeEnemyCount++;
        }
        else
        {
            Debug.LogWarning($"无法从工厂创建敌人: {config.prefabAddress}");
        }
    }

    /// <summary>
    /// 在全局生成范围内获取一个随机位置
    /// </summary>
    private Vector2 GetRandomPositionInGlobalRange()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius = Random.Range(0f, globalSpawnRadius);
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        return globalSpawnCenter + offset;
    }

    /// <summary>
    /// 敌人死亡事件的回调
    /// </summary>
    private void OnEnemyDied()
    {
        if (activeEnemyCount > 0)
        {
            activeEnemyCount--;
        }
    }
    #endregion

    #region 编辑器可视化
    /// <summary>
    /// 在编辑器中绘制生成范围的辅助线
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(globalSpawnCenter, globalSpawnRadius);
    }
    #endregion
}
