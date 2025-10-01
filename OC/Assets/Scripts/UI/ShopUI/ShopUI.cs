using GameEvents;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEditor.Progress;
using static UnityEngine.Rendering.DebugUI;

public class ShopUI : UIPanel
{
    private GameObject itemForSalePrefab;
    private GameObject list;

    private List<ItemConfiguration> itemsForSaleConfigs;
    private List<GameObject> itemCreated = new();
    private bool isItemCreated = false;

    public override async void OnOpen()
    {
        if(itemForSalePrefab == null)
            itemForSalePrefab = await Tools.LoadAddressable<GameObject>("ItemForSale");

        if(list == null)
            list = transform.Find("List").gameObject;

        if(itemsForSaleConfigs == null)
            await LoadItemConfigurations();

        if (itemForSalePrefab != null && list != null)
        {
            if (!isItemCreated)
            {
                CreateShopItems(3);
                isItemCreated = true;
            }
            else
            {
                foreach(var  item in itemCreated)
                {
                    item.SetActive(true);
                    item.GetComponent<ItemForSale>().Init(itemsForSaleConfigs[Tools.RandomInt(0, itemsForSaleConfigs.Count)]);
                }
            }
        }

        Debug.Log("itemCreated.Count: " + itemCreated.Count);

        RegisterButton("Continue",async () => {
            EnemyManager.Instance.StartWave();
            await UIManager.Instance.OpenPanelAsync<FightUI>("FightUI");
            UIManager.Instance.ClosePanel("ShopUI");
        });
    }


    /// <summary>
    /// 加载所有带"Item"标签的物品配置文件
    /// </summary>
    private async Task LoadItemConfigurations()
    {
        try
        {
            itemsForSaleConfigs = await Tools.LoadAddressablesByLabel<ItemConfiguration>("Item");
            Debug.Log($"成功加载 {itemsForSaleConfigs.Count} 个物品预制体");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"加载Item标签资源失败: {ex.Message}");
            itemsForSaleConfigs = new List<ItemConfiguration>();
        }
    }

    /// <summary>
    /// 根据加载的物品创建商店项目
    /// </summary>
    private void CreateShopItems(int value)
    {
        if (itemsForSaleConfigs.Count == 0)
        {
            Debug.LogWarning("没有找到带'Item'标签的物品!!!!!!");
        }
        else
        {
            for (int i = 0; i < value; i++)
            {
                var shopItem = Instantiate(itemForSalePrefab, list.transform);
                shopItem.GetComponent<ItemForSale>().Init(itemsForSaleConfigs[Tools.RandomInt(0, itemsForSaleConfigs.Count)]);
                itemCreated.Add(shopItem);
            }
        }
    }
}
