using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoleIcon : MonoBehaviour,IPointerClickHandler
{
    public Image icon;
    public Image selectedIcon;
    public Color selectedColor;
    public string roleName;
    private RoleConfiguration roleConfig;

    public void Init(RoleConfiguration config)
    {
        icon.sprite = config.roleIcon;
        roleName = config.roleName;
        selectedIcon.color = Color.clear;
        roleConfig = config;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EventCenter.Publish<RoleSelectedEvent, RoleConfiguration>(roleConfig);
    }

    public void SetSelected(bool state)
    {
        if (state)
        {
            selectedIcon.color = selectedColor;
        }
        else
        {
            selectedIcon.color = Color.clear;
        }
    }
}
