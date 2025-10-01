using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Configuration", menuName = "Game/Items/1_Banana")]
public class Banana : ItemConfiguration
{
    [Header("属性")]
    [Tooltip("增加的速度（百分比）")]
    public float value = 0.5f;
    public override void OnGet()
    {
        GlobalModificationManager.Instance.globalPlayerAttributeModifier.AddModifier(AttributeType.moveSpeedModifierPercent, ModificationType.Add, value);
    } 
}
