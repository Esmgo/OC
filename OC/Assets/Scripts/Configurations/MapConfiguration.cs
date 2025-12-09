using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapConfiguration", menuName = "Game/MapConfiguration")]
public class MapConfiguration : ScriptableObject
{
    [Tooltip("地图名")]
    public string mapName = "DefaultMap"; // 地图名称

    [Tooltip("波次配置")]
    public List<WaveConfiguration> waveConfigurations = new List<WaveConfiguration>(); // 波次配置列表
}

[Serializable]
public class WaveConfiguration
{
    [Tooltip("敌人预制体列表")]
    public List<EnemyConfiguration> enemyConfigs = new(); // 敌人预制体列表
}
