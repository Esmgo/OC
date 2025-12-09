using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyStatComponent))]
[RequireComponent(typeof(EnemyBaseMoveComponent))]

public class EnemyComponent : EntityComponent
{
    private EnemyStatComponent statComponent;
    private EnemyBaseMoveComponent moveComponent;

    public void Init(EnemyConfiguration config)
    {
        statComponent = GetComponent<EnemyStatComponent>();
        moveComponent = GetComponent<EnemyBaseMoveComponent>();
        statComponent.Init(config);
        moveComponent.Init(config);
    }
}
