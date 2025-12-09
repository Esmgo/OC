using GameEvents;
using System;
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
    private CharacterConfiguration roleConfig;

    public Action<CharacterConfiguration> OnCharacterSelected;

    public void Init(CharacterConfiguration config)
    {
        //icon.sprite = config.roleIcon;
        roleName = config.roleName;
        selectedIcon.color = Color.clear;
        roleConfig = config;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnCharacterSelected?.Invoke(roleConfig);
    }

    public void SetSelected(bool state)
    {
        selectedIcon.color = state ? selectedColor : Color.clear;
    }
}
