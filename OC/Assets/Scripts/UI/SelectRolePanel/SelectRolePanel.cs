using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectRolePanel : UIPanel
{
    private RoleList roleList;
    private InformationPanel informationField;

    private bool isInited = false;

    public override void OnOpen()
    {
        if (!isInited) Init();
    }

    private void Init()
    {
        isInited = true;
 
        roleList = transform.Find("RoleList").GetComponent<RoleList>();
        informationField = transform.Find("InformationField").GetComponent<InformationPanel>();

        roleList.OnCharacterSelected = OnCharacterSelected;
        roleList.Init();

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

    private void OnCharacterSelected(CharacterConfiguration config)
    {
        // 更新信息面板
        informationField.ShowRoleInfo(config);
        informationField.RefreshWeaponIcon(config);

        //更新选中的角色
        CharacterManager.Instance.SetSelectedCharacterConfig(config);
    }
}
