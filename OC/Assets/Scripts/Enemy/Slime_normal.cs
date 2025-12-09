using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime_normal : EnemyBase
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Character c = other.GetComponent<Character>();
        //if (c != null && !isDead)
        //{
        //    EventCenter.Publish<HPChangeEvent, int>(-10);
        //}
    }
}
