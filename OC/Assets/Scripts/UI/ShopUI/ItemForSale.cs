using GameEvents;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemForSale : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI itemPrice;

    private ItemConfiguration itemConfig;
    public void Init(ItemConfiguration itemConfig) 
    { 
        itemName.text = itemConfig.itemName;
        itemDescription.text = itemConfig.description;
        itemPrice.text = itemConfig.price.ToString();

        this.itemConfig = itemConfig;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"购买物品: {itemConfig.itemName}，价格: {itemConfig.price}");
        //ItemManager.Instance.AddItem(itemConfig, CharacterManager.Instance.currentCharacter);
        //EventCenter.Publish<PlayerPropertyChangedEvent>();
        gameObject.SetActive(false); // 购买后隐藏物品
    }
}
