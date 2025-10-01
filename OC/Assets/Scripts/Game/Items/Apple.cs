using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneTemplate;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Configuration", menuName = "Game/Items/0_Apple")]
public class Apple : ItemConfiguration
{
    [Header("属性")]
    [Tooltip("增加的最大生命值")]
    public int value = 5;
    public override void OnGet()
    {
        Tools.GetGlobalAttributeModifier().AddModifier(AttributeType.maxHealthModifier, ModificationType.Add, value);
    }
}

