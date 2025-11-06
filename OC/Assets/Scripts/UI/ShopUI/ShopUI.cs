using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : UIPanel
{
    #region 字段

    [Header("UI 引用")]
    private TextMeshProUGUI plyerInfoText;
    private Image playerInfoBackground;
    private Image background;
    private GameObject list;

    [Header("预制件与配置")]
    private GameObject itemForSalePrefab;
    private List<ItemConfiguration> itemsForSaleConfigs;

    [Header("运行时数据")]
    private Character character;
    private List<GameObject> itemCreated = new();
    private bool isInitialized = false;
    private bool isCreatingItems = false;

    private Vector3 playerInfoOnScreenPos;
    private Vector3 playerInfoOffScreenPos;
    private bool arePositionsInitialized = false;

    #endregion

    #region UIPanel 生命周期

    public override void OnOpen()
    {
        character = Tools.GetCharacter();
        if (character == null)
        {
            Debug.LogError("ShopUI: 角色为空，无法打开商店！");
            return;
        }

        // 订阅事件总是在最前面，确保不会错过任何变化
        character.OnStatChanged += UpdateInfoDisplay;

        if (!isInitialized)
        {
            // 首次打开，执行异步初始化
            InitializeAsync();
        }
        else
        {
            // 非首次打开，直接刷新和播放动画
            RefreshItems();
            UpdateInfoDisplay();
            StartCoroutine(OpenAnimation());
        }
    }

    public override void OnClose()
    {
        if (character != null)
        {
            character.OnStatChanged -= UpdateInfoDisplay;
        }
    }

    #endregion

    #region 初始化

    /// <summary>
    /// 异步初始化UI，仅在首次打开时调用
    /// </summary>
    private async void InitializeAsync()
    {
        // --- 1. 加载必要的UI组件和资源 ---
        plyerInfoText = transform.Find("PlayerInfoBackground/Text").GetComponent<TextMeshProUGUI>();
        background = transform.Find("Background").GetComponent<Image>();
        playerInfoBackground = transform.Find("PlayerInfoBackground").GetComponent<Image>();
        list = transform.Find("List").gameObject;

        // 异步加载资源
        itemForSalePrefab = await Tools.LoadAddressable<GameObject>("ItemForSale");
        await LoadItemConfigurations();

        // --- 2. 注册按钮事件 ---
        RegisterButton("Continue", OnContinueClicked);
        RegisterButton("Refresh", RefreshItems);

        if (!arePositionsInitialized)
        {
            playerInfoOnScreenPos = playerInfoBackground.rectTransform.position;
            playerInfoOffScreenPos = new Vector3(playerInfoOnScreenPos.x - 500, playerInfoOnScreenPos.y, playerInfoOnScreenPos.z);
            arePositionsInitialized = true;
        }

        // --- 3. 初始化完成，更新状态并执行首次打开逻辑 ---
        isInitialized = true;
        RefreshItems();
        UpdateInfoDisplay();
        StartCoroutine(OpenAnimation());
    }

    /// <summary>
    /// 加载所有带"Item"标签的物品配置文件
    /// </summary>
    private async Task LoadItemConfigurations()
    {
        try
        {
            itemsForSaleConfigs = await Tools.LoadAddressablesByLabel<ItemConfiguration>("Item");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"加载Item标签资源失败: {ex.Message}");
            itemsForSaleConfigs = new List<ItemConfiguration>();
        }
    }

    #endregion

    #region UI 逻辑

    /// <summary>
    /// 刷新商店展示的物品
    /// </summary>
    private void RefreshItems()
    {
        if (itemCreated.Count == 0 && !isCreatingItems)
        {
            // 如果从未创建过物品，则创建它们
            CreateShopItems(3);
        }
        else
        {
            // 如果已经创建过，则刷新它们的内容
            foreach (var item in itemCreated)
            {
                item.SetActive(true);
                item.GetComponent<ItemForSale>().Init(itemsForSaleConfigs[Tools.RandomInt(0, itemsForSaleConfigs.Count)]);
            }
        }
    }

    /// <summary>
    /// 根据加载的物品创建商店项目
    /// </summary>
    private void CreateShopItems(int count)
    {
        if (itemsForSaleConfigs == null || itemsForSaleConfigs.Count == 0)
        {
            Debug.LogWarning("物品配置尚未加载或为空，无法创建商店物品。");
            return;
        }
        if (itemForSalePrefab == null)
        {
            Debug.LogError("商店物品预制件尚未加载，无法创建。");
            return;
        }

        isCreatingItems = true;
        for (int i = 0; i < count; i++)
        {
            var shopItem = Instantiate(itemForSalePrefab, list.transform);
            shopItem.GetComponent<ItemForSale>().Init(itemsForSaleConfigs[Tools.RandomInt(0, itemsForSaleConfigs.Count)]);
            itemCreated.Add(shopItem);
        }
        isCreatingItems = false;
    }

    /// <summary>
    /// 更新右侧的角色信息面板
    /// </summary>
    private void UpdateInfoDisplay()
    {
        if (!isInitialized || character == null) return;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"最大生命值<pos=60%>{character.currentMaxHealth}");
        sb.AppendLine($"生命回复<pos=60%>{character.currentHealthRegenRate:F1}/秒");
        sb.AppendLine($"最大能量值<pos=60%>{character.currentMaxEnergy}");
        sb.AppendLine($"能量回复<pos=60%>{character.currentEnergyRegenRate:F1}/秒");
        sb.AppendLine($"冷却缩减<pos=60%>{character.currentCooldownReductionPercent * 100:F0}%");
        sb.AppendLine($"冲刺冷却<pos=60%>{character.currentDashCoolDown:F1}秒");
        sb.AppendLine($"移动速度<pos=60%>{character.currentMoveSpeed:F1}");
        sb.AppendLine($"攻击间隔<pos=60%>{character.currentAttackInterval:F2}秒");
        sb.AppendLine($"物理伤害<pos=60%>{character.currentPhysicalDamage:F0}");
        sb.AppendLine($"异能伤害<pos=60%>{character.currentManaDamage:F0}");
        sb.AppendLine($"元素伤害<pos=60%>{character.currentElementalDamage:F0}");

        plyerInfoText.text = sb.ToString();
    }

    /// <summary>
    /// UI打开时的动画效果
    /// </summary>
    private IEnumerator OpenAnimation()
    {
        // 确保动画开始前，面板在屏幕外的位置
        playerInfoBackground.rectTransform.position = playerInfoOffScreenPos;

        background.DOFade(1f, 0.5f);

        // 从屏幕外的位置移动到屏幕内的位置
        playerInfoBackground.rectTransform.DOMoveX(playerInfoOnScreenPos.x, 1f).SetEase(Ease.OutQuad);

        yield return null;
    }

    #endregion

    #region 按钮事件处理

    private async void OnContinueClicked()
    {
        EnemyManager.Instance.StartWave();
        await UIManager.Instance.OpenPanelAsync<FightUI>("FightUI");
        UIManager.Instance.ClosePanel("ShopUI");
    }

    #endregion
}
