using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanel : UIPanel
{
    public override void OnOpen()
    {
        RegisterButton("Start", async () =>
        {
            await UIManager.Instance.OpenPanelAsync<SelectRolePanel>("SelectRolePanel");
            UIManager.Instance.ClosePanel("MainPanel");
        });

        RegisterButton("Test", async () =>
        {
            await UIManager.Instance.OpenPanelAsync<NetworkPanel>("NetworkPanel");
            UIManager.Instance.ClosePanel("MainPanel");
        });
    }
}
