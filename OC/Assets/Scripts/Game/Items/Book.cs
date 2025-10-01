using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 书 - 增加异能伤害
/// </summary>
[CreateAssetMenu(fileName = "New Item Configuration", menuName = "Game/Items/2_Book")]
public class Book : ItemConfiguration
{
    [Header("属性")]
    [Tooltip("增加的异能伤害")]
    public int value = 3;
    public override void OnGet()
    {
        Tools.GetGlobalAttributeModifier().AddModifier(AttributeType.energyDamageModifier, ModificationType.Add, value);
    }
}
