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
            // 关闭当前面板
            UIManager.Instance.ClosePanel("SelectRolePanel");
        });

        RegisterButton("Back", async () =>
        {
            await UIManager.Instance.OpenPanelAsync<MainPanel>("MainPanel");
            // 返回主菜单
            UIManager.Instance.ClosePanel("SelectRolePanel");
        });
    }
}
