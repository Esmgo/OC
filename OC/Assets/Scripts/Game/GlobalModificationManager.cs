using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalModificationManager : MonoBehaviour
{
    public static GlobalModificationManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public PlayerAttributeModifier globalPlayerAttributeModifier = new();

    public void Init()
    {
        globalPlayerAttributeModifier = new();
        EventCenter.Subscribe<GetItemEvent, List<AttributeEffect>>(OnGetItem);
    }


    private void OnGetItem(List<AttributeEffect> attributeEffectList)
    {
        foreach (var effect in attributeEffectList)
        {
            globalPlayerAttributeModifier.AddModifier(effect.attributeType, effect.modificationType, effect.value);
        }
    }
}
