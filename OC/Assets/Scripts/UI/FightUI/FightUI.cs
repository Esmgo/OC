using GameEvents;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FightUI : UIPanel
{
    private TextMeshProUGUI timeText;

    public override void OnOpen()
    {
        timeText = transform.Find("TimeText").GetComponent<TextMeshProUGUI>();
        EventCenter.Publish<UpdateInfoDisplayEvent, Character>(Tools.GetCharacter());
    }

    private void Update()
    {
        if (timeText != null) 
        {
            timeText.text = $"Time: {EnemyManager.Instance.waveTimer:F2} seconds";
        }
    }
}
