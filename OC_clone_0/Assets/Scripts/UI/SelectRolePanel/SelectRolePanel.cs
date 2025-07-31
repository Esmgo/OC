using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectRolePanel : UIPanel
{
    private RoleList roleList;
    private InformationPanel InformationField;

    private RoleConfiguration selectedRoleConfig;
    private WeaponConfiguration selectedWeaponConfig;
    private bool isWeaponIconCreated = false;

    public override void OnOpen()
    {
        EventCenter.Subscribe<RoleSelectedEvent, RoleConfiguration>(OnRoleSelected);
        EventCenter.Subscribe<WeaponSelectedEvent, WeaponConfiguration>(OnWeaponSelected);

        roleList = transform.Find("RoleList").GetComponent<RoleList>();
        InformationField = transform.Find("InformationField").GetComponent<InformationPanel>();

        roleList.Init();
        
        RegisterButton("Start", () =>
        {
            GameApplication.Instance.GameStart(selectedRoleConfig, selectedWeaponConfig);
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

    private void OnRoleSelected(RoleConfiguration config)
    {
        selectedRoleConfig = config;
        roleList.SelectRole(config);
        Debug.Log($"��ѡ���ɫ: {config.roleName}");
        if(!isWeaponIconCreated)
        {
            InformationField.CreatWeaponIcon(config);
            isWeaponIconCreated = true;
        }
        InformationField.ShowRoleInfo(config);
        InformationField.RefreshWeaponIcon(config);
        if (selectedWeaponConfig != null && config.weaponConfig.Contains(selectedWeaponConfig))
        {
            InformationField.ShowWeaponInfo(selectedWeaponConfig);
        }
        else
        {
            InformationField.ShowWeaponInfo(config.weaponConfig[0]);
        }
    }

    private void OnWeaponSelected(WeaponConfiguration config)
    {
        selectedWeaponConfig = config;
        Debug.Log($"��ѡ������: {config.weaponName}");
        if(selectedRoleConfig != null && selectedRoleConfig.weaponConfig.Contains(config))
        {
            InformationField.ShowWeaponInfo(config);
        }
        else
        {
            InformationField.ShowWeaponInfo(selectedRoleConfig.weaponConfig[0]);
        }
    }
}
