using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanel : UIPanel
{
    public override void OnOpen()
    {
        var btn = transform.Find("Start").GetComponent<UIButton>();
        btn.onClick.AddListener(async () =>
        {
            await UIManager.Instance.OpenPanelAsync<SelectRolePanel>("SelectRolePanel");
            UIManager.Instance.ClosePanel("MainPanel");
        });
    }
}
