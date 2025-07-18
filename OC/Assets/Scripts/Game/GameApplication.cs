using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameApplication : Singleton<GameApplication>
{
    public int killCount = 0;

    // 事件：每10个击杀
    public delegate void KillMilestoneHandler(int milestone);
    public event KillMilestoneHandler OnKillMilestone;

    // Start is called before the first frame update
    void Start()
    {
        DOTween.Init().SetCapacity(200, 50);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddKill()
    {
        killCount++;
        if (killCount % 10 == 0)
        {
            OnKillMilestone?.Invoke(killCount / 10);
        }
    }
}
