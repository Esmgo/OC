using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameApplication : Singleton<GameApplication>
{
    public int killCount = 0;

    // �¼���ÿ10����ɱ
    public delegate void KillMilestoneHandler(int milestone);
    public event KillMilestoneHandler OnKillMilestone;

    async void Start() // �޸�Ϊ�첽����
    {
        DOTween.Init().SetCapacity(200, 50);
        UIManager.Instance.RegisterPanel("MainPanel", "MainPanel");
        await UIManager.Instance.OpenPanelAsync<MainPanel>("MainPanel"); 
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
