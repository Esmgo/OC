using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new enemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName = "newEnemy";
    public float MaxHealth = 10f;
    public float MoveSpeed = 5f;
    public int physicalDamage = 0;
    public int energyDamage = 0;
    public int elementalDamage = 0;
}
