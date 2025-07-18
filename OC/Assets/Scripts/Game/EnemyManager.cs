using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("���ɲ���")]
    public GameObject[] enemyPrefabs; // ����Ԥ�������飬֧�ֶ�������
    public int maxEnemyCount = 50; // �����������
    public float spawnInterval = 1.5f; // ���ɼ�����룩
    public float spawnRadius = 8f; // ���ɰ뾶�������Ϊ���ģ�
    public Transform player; // ��Ҷ���

    private List<GameObject> enemyPool = new List<GameObject>();
    private int activeEnemyCount = 0;
    private float timer = 0f;

    void Awake()
    {
        // ��ѡ���Զ��������
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
