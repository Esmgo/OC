using GameEvents;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Mirror.BouncyCastle.Math.EC.ECCurve;

public class InformationPanel : MonoBehaviour
{
    public GameObject informationBar;
    public GameObject weaponIcon;
    public Transform roleInformationContent;
    public Transform weaponIconContent;
    public Transform weaponInformationContent;
    public TextMeshProUGUI roleName;


    private List<InformationBar> roleInfoBars = new();
    private List<WeaponIcon> weaponIcons = new();
    private List<InformationBar> weaponInfoBars = new();

    public void ShowRoleInfo(RoleConfiguration config)
    {
        roleName.text = config.roleName;
        GameObject.Find("Background").GetComponent<SpriteRenderer>().material.SetColor("_LineColor", config.themeColor);
        ShowInfo("����", config.maxHealth, 0, config.themeColor, roleInformationContent, roleInfoBars);
        ShowInfo("����", config.energy, 1, config.themeColor, roleInformationContent, roleInfoBars);
        ShowInfo("�����ظ�", config.energyRegenRate, 2, config.themeColor, roleInformationContent, roleInfoBars, false, 20);
        ShowInfo("�����˺�", config.physicalDamage, 3, config.themeColor, roleInformationContent, roleInfoBars, false, 30);
        ShowInfo("Ԫ���˺�", config.elementalDamage, 4, config.themeColor, roleInformationContent, roleInfoBars, false, 30);
        ShowInfo("�����˺�", config.energyDamage, 5, config.themeColor, roleInformationContent, roleInfoBars, false, 30);
        ShowInfo("�ƶ��ٶ�", config.moveSpeed, 6, config.themeColor, roleInformationContent, roleInfoBars, false, 30);
        ShowInfo("����ֵ", config.sanity, 7, config.themeColor, roleInformationContent, roleInfoBars);
        ShowInfo("����ٶ�", config.dashSpeed, 8, config.themeColor, roleInformationContent, roleInfoBars, false, 30);
        ShowInfo("�����ȴ", config.dashCooldown, 9, config.themeColor, roleInformationContent, roleInfoBars, false, 5);
        ShowInfo("�����ظ�", config.healthRegenRate, 10, config.themeColor, roleInformationContent, roleInfoBars, false, 30);
        ShowInfo("����", config.shield, 11, config.themeColor, roleInformationContent, roleInfoBars, false, 30);
    }

    public void ShowWeaponInfo(WeaponConfiguration weaponConfig)
    {
        ShowInfo("�������", weaponConfig.attackInterval, 0, Color.white, weaponInformationContent, weaponInfoBars, false, 2);
        ShowInfo("�����˺�ռ��", weaponConfig.physicalDamageRatio, 1, Color.white, weaponInformationContent, weaponInfoBars, true);
        ShowInfo("Ԫ���˺�ռ��", weaponConfig.elementalDamageRatio, 2, Color.white, weaponInformationContent, weaponInfoBars, true);
        ShowInfo("�����˺�ռ��", weaponConfig.energyDamageRatio, 3, Color.white, weaponInformationContent, weaponInfoBars, true);
        ShowInfo("����״̬�������", weaponConfig.attackInterval * (100 + weaponConfig.weakenedAttackInterval) / 100, 4, Color.white, weaponInformationContent, weaponInfoBars, false, 2);
        ShowInfo("����״̬�˺�˥��", weaponConfig.weakenedDamageReduction, 5, Color.red, weaponInformationContent, weaponInfoBars, true);
    }

    private void ShowInfo(string name, float value, int no, Color themeColor, Transform content, List<InformationBar> aimBar, bool isPercent = false, float max = 100)
    {
        if (no >= aimBar.Count)
        {
            GameObject bar = Instantiate(informationBar, content, false);
            bar.name = name;
            var b = bar.GetComponent<InformationBar>();
            b.Set(name, value, value / max, themeColor, isPercent);
            aimBar.Add(b);
        }
        else
        {
            var b = aimBar[no];
            b.Set(name, value, value / max, themeColor, isPercent);
        }
    }

    public void CreatWeaponIcon(RoleConfiguration roleConfig)
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject wpIcon = Instantiate(weaponIcon, weaponIconContent, false);
            weaponIcons.Add(wpIcon.GetComponent<WeaponIcon>());
        }
        RefreshWeaponIcon(roleConfig);
        EventCenter.Publish<WeaponSelectedEvent, WeaponConfiguration>(roleConfig.weaponConfig[0]);
    }

    public void RefreshWeaponIcon(RoleConfiguration roleConfig)
    {
        for (int i = 0; i < 5; i++)
        {
            if (i < roleConfig.weaponConfig.Count)
            {
                weaponIcons[i].gameObject.SetActive(true);
                weaponIcons[i].Init(roleConfig.weaponConfig[i]);

            }
            else
            {
                weaponIcons[i].gameObject.SetActive(false);
            }
        }
    }
}
