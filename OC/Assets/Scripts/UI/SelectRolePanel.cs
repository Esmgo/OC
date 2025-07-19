using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectRolePanel : UIPanel
{
    public override void OnOpen()
    {
        RegisterButton("Start", () =>
        {
            GameApplication.Instance.GameStart();
            // �رյ�ǰ���
            UIManager.Instance.ClosePanel("SelectRolePanel");
        });

        RegisterButton("Back", async () =>
        {
            await UIManager.Instance.OpenPanelAsync<MainPanel>("MainPanel");
            // �������˵�
            UIManager.Instance.ClosePanel("SelectRolePanel");
        });
    }
}
