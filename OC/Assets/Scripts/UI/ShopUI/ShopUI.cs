using GameEvents;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEditor.Progress;
using static UnityEngine.Rendering.DebugUI;

public class ShopUI : UIPanel
{
    private GameObject itemForSalePrefab;
    private GameObject list;
    private TextMeshProUGUI plyerInfo;
    private Character character;
    private List<ItemConfiguration> itemsForSaleConfigs;
    private List<GameObject> itemCreated = new();
    private bool isItemCreated = false;

    private bool isInited = false;

    public override void OnOpen()
    {
        Init();

        
        //if (itemForSalePrefab != null && list != null)
        //{
            
        //}
    }

    private void Update()
    {
        if (isInited && character != null)
        {
            // 使用 StringBuilder 来高效地构建多行文本
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            // 使用 <pos=60%> 标签将数字部分对齐到文本框宽度的 60% 位置
            // 你可以根据需要调整这个百分比
            sb.AppendLine($"最大生命值<pos=60%>{character.maxHealth}");
            sb.AppendLine($"移动速度<pos=60%>{character.moveSpeed}");
            sb.AppendLine($"物理伤害<pos=60%>{character.physicalDamage}");
            sb.AppendLine($"异能伤害<pos=60%>{character.energyDamage}");

            plyerInfo.text = sb.ToString();
        }
    }

    private async void Init()
    {
        if(isInited)
            return;
        isInited = true;
        //if (itemForSalePrefab == null)
            itemForSalePrefab = await Tools.LoadAddressable<GameObject>("ItemForSale");

        //if (list == null)
            list = transform.Find("List").gameObject;

        //if (itemsForSaleConfigs == null)
            await LoadItemConfigurations();

        plyerInfo = transform.Find("PlayerInfo/Text").GetComponent<TextMeshProUGUI>();
        character = Tools.GetCharacter();

        if (itemForSalePrefab != null && list != null)
        {
            RefreshItems();
            //if (!isItemCreated)
            //{
            //    CreateShopItems(3);
            //    isItemCreated = true;
            //}
            //else
            //{
            //    foreach (var item in itemCreated)
            //    {
            //        item.SetActive(true);
            //        item.GetComponent<ItemForSale>().Init(itemsForSaleConfigs[Tools.RandomInt(0, itemsForSaleConfigs.Count)]);
            //    }
            //}
        }

        //Debug.Log("itemCreated.Count: " + itemCreated.Count);

        RegisterButton("Continue", async () => {
            EnemyManager.Instance.StartWave();
            await UIManager.Instance.OpenPanelAsync<FightUI>("FightUI");
            UIManager.Instance.ClosePanel("ShopUI");
        });

        RegisterButton("Refresh", () => {
            RefreshItems();
        });
    }

    /// <summary>
    /// 刷新商店物品
    /// </summary>
    private void RefreshItems()
    {
        if (!isItemCreated)
        {
            CreateShopItems(3);
            isItemCreated = true;
        }
        //else
        //{
            foreach (var item in itemCreated)
            {
                item.SetActive(true);
                item.GetComponent<ItemForSale>().Init(itemsForSaleConfigs[Tools.RandomInt(0, itemsForSaleConfigs.Count)]);
            }
        //}
    }



    /// <summary>
    /// 加载所有带"Item"标签的物品配置文件
    /// </summary>
    private async Task LoadItemConfigurations()
    {
        try
        {
            itemsForSaleConfigs = await Tools.LoadAddressablesByLabel<ItemConfiguration>("Item");
            //Debug.Log($"成功加载 {itemsForSaleConfigs.Count} 个物品预制体");
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
