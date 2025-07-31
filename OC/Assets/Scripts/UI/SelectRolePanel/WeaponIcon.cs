using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponIcon : MonoBehaviour, IPointerClickHandler
{
    private WeaponConfiguration weaponConfig;

    public void Init(WeaponConfiguration weaponConfig)
    {
        this.weaponConfig = weaponConfig;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EventCenter.Publish<WeaponSelectedEvent, WeaponConfiguration>(weaponConfig);
    }
}
