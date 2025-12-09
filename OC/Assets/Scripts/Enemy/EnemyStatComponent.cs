using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatComponent : StatComponent
{
    public int maxHealth { get; private set; }
    public int currentHealth { get; private set; }

    public void Init(EnemyConfiguration config)
    {

    }

    public override void Health()
    {
        
    }
}
