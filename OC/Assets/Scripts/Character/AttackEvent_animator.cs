using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEvent_animator : MonoBehaviour
{
    public void AttackStart()
    {
        EventCenter.Publish <AttackStartEvent>();
    }

    public void AttackCheck()
    {
        EventCenter.Publish<AttackEvent>();
    }

    public void AttackEnd()
    {
        EventCenter.Publish<AttackEndEvent>();
    }
}
