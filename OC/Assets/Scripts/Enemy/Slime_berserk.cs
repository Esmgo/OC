using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime_berserk : EnemyBase
{
    override protected void Die()
    {
        //var bc = ObjectPoolManager.Instance.GetObject<BuffCircle>("BuffCircle", transform.position);
        base.Die();
    }
}
