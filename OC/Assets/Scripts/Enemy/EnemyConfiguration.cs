using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Configuration", menuName = "Game/Config/Enemy Configuration")]
public class EnemyConfiguration : ScriptableObject
{
    public string prefabAddress;

    public int maxHealth = 100;

    public float moveSpeed = 10.0f;
}
