using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using GameEvents;

/// <summary>
/// 角色列表管理器，负责加载和显示所有角色
/// </summary>
public class RoleList : MonoBehaviour
{  
    [Header("角色列表配置")]
    [Tooltip("角色配置的Addressables标签/分组")]
    public string roleGroupLabel = "RoleConfiguration";
    
    [Tooltip("角色图标预制体")]
    public GameObject roleIconPrefab;
    
    [Tooltip("角色图标的父容器")]
    public Transform roleIconContainer;
    
    // 已加载的角色配置列表
    private List<RoleConfiguration> loadedRoleConfigs = new List<RoleConfiguration>();
    
    // 创建的角色图标列表
    private List<RoleIcon> roleIcons = new List<RoleIcon>();
    
    // Addressables操作句柄
    private AsyncOperationHandle<IList<RoleConfiguration>> roleConfigsHandle;
    
    // 初始化状态
    private bool isInitialized = false;
    
    /// <summary>
    /// 初始化角色列表
    /// </summary>
    public async void Init()
    {
        if (isInitialized)
        {
            Debug.LogWarning("RoleList 已经初始化过了");
            return;
        }
        Debug.Log("开始初始化角色列表...");
        
        // 订阅事件
        SubscribeToEvents();
        
        // 加载所有角色配置
        await LoadAllRoleConfigurations();
        
        // 创建角色图标
        CreateRoleIcons();
        
        isInitialized = true;
        
        Debug.Log($"角色列表初始化完成，共加载 {loadedRoleConfigs.Count} 个角色");
    }

    public void SelectRole(RoleConfiguration config)
    {
        foreach (var icon in roleIcons)
        {
            if (icon != null && icon.roleName != config.roleName)
            {
                icon.SetSelected(false);
            }
            else if (icon != null && icon.roleName == config.roleName)
            {
                icon.SetSelected(true);
            }
        }

    }

    /// <summary>
    /// 使用Addressables加载同一分组下的所有角色配置
    /// </summary>
    private async Task LoadAllRoleConfigurations()
    {
        try
        {
            Debug.Log($"正在加载分组 '{roleGroupLabel}' 中的所有角色配置...");
            
            // 使用Addressables.LoadAssetsAsync加载指定标签下的所有资源
            roleConfigsHandle = Addressables.LoadAssetsAsync<RoleConfiguration>(roleGroupLabel,null/*回调函数（可选）*/);
            await roleConfigsHandle.Task;   // 等待加载完成

            if (roleConfigsHandle.Status == AsyncOperationStatus.Succeeded)
            {
                loadedRoleConfigs.Clear();
                
                // 遍历加载的资源
                foreach (var roleConfig in roleConfigsHandle.Result)
                {
                    if (roleConfig != null)
                    {
                        loadedRoleConfigs.Add(roleConfig);
                        Debug.Log($"成功加载角色配置: {roleConfig.roleName}");
                    }
                    else
                    {
                        Debug.LogWarning($"角色配置验证失败: {roleConfig?.roleName ?? "Unknown"}");
                    }
                }
                
                Debug.Log($"分组加载完成，共加载 {loadedRoleConfigs.Count} 个有效角色配置");
            }
            else
            {
                Debug.LogError($"加载角色配置分组失败: {roleGroupLabel}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载角色配置时发生异常: {e.Message}");
        }
    }
    
    /// <summary>
    /// 创建角色图标UI
    /// </summary>
    private void CreateRoleIcons()
    {
        if (roleIconPrefab == null || roleIconContainer == null)
        {
            Debug.LogError("RoleList: roleIconPrefab 或 roleIconContainer 未设置");
            return;
        }
        
        // 清理现有图标
        ClearRoleIcons();
        
        // 为每个角色配置创建图标
        for (int i = 0; i < loadedRoleConfigs.Count; i++)
        {
            var roleConfig = loadedRoleConfigs[i];
            CreateRoleIcon(roleConfig, i);
        }
        
        // 选中第一个角色（如果有的话）
        if (roleIcons.Count > 0)
        {
            EventCenter.Publish<RoleSelectedEvent, RoleConfiguration>(loadedRoleConfigs[0]);
        }
    }
    
    /// <summary>
    /// 创建单个角色图标
    /// </summary>
    private void CreateRoleIcon(RoleConfiguration roleConfig, int index)
    {
        GameObject iconObj = Instantiate(roleIconPrefab, roleIconContainer);
        RoleIcon roleIcon = iconObj.GetComponent<RoleIcon>();
        
        if (roleIcon != null)
        {
            // 初始化角色图标
            roleIcon.Init(roleConfig);
            roleIcons.Add(roleIcon);            
        }
        else
        {
            Debug.LogError($"角色图标预制体缺少 RoleIcon 组件");
            Destroy(iconObj);
        }
    }

    /// <summary>
    /// 清理所有角色图标
    /// </summary>
    private void ClearRoleIcons()
    {
        foreach (var icon in roleIcons)
        {
            if (icon != null)
                Destroy(icon.gameObject);
        }
        roleIcons.Clear();
    }
    
    /// <summary>
    /// 刷新角色列表
    /// </summary>
    public async void RefreshRoleList()
    {
        Debug.Log("刷新角色列表...");
        
        // 清理现有数据
        ClearRoleIcons();
        loadedRoleConfigs.Clear();
        
        // 释放之前的Addressables句柄
        if (roleConfigsHandle.IsValid())
        {
            Addressables.Release(roleConfigsHandle);
        }
        
        // 重新加载
        await LoadAllRoleConfigurations();
        CreateRoleIcons();
    }
    
    /// <summary>
    /// 根据名称获取角色配置
    /// </summary>
    public RoleConfiguration GetRoleConfiguration(string roleName)
    {
        return loadedRoleConfigs.Find(config => config.roleName == roleName);
    }
    
    /// <summary>
    /// 获取所有角色配置
    /// </summary>
    public List<RoleConfiguration> GetAllRoleConfigurations()
    {
        return new List<RoleConfiguration>(loadedRoleConfigs);
    }
    
    /// <summary>
    /// 获取角色数量
    /// </summary>
    public int GetRoleCount()
    {
        return loadedRoleConfigs.Count;
    }
    
    #region 事件处理
    
    private void SubscribeToEvents()
    {
        // 订阅角色配置管理器的配置加载完成事件
        EventCenter.Subscribe<ConfigurationLoadedEvent, string>(OnConfigurationLoaded);
    }
    
    private void UnsubscribeFromEvents()
    {
        EventCenter.Unsubscribe<ConfigurationLoadedEvent, string>(OnConfigurationLoaded);
    }
    
    private void OnConfigurationLoaded(string configurationType)
    {
        if (configurationType == "RoleConfigurationManager")
        {
            // 如果角色配置管理器重新加载了配置，刷新列表
            RefreshRoleList();
        }
    }
    
    #endregion
    
    #region 生命周期
    
    private void OnDestroy()
    {
        // 取消事件订阅
        UnsubscribeFromEvents();
        
        // 释放Addressables资源
        if (roleConfigsHandle.IsValid())
        {
            Addressables.Release(roleConfigsHandle);
            Debug.Log("释放角色配置Addressables资源");
        }
    }
    
    #endregion
    
    #region 编辑器工具方法
    
#if UNITY_EDITOR
    [ContextMenu("Test Load Role Configs")]
    private async void TestLoadRoleConfigs()
    {
        await LoadAllRoleConfigurations();
    }
    
    [ContextMenu("Print Loaded Roles")]
    private void PrintLoadedRoles()
    {
        Debug.Log("=== 已加载的角色配置 ===");
        foreach (var config in loadedRoleConfigs)
        {
            Debug.Log($"角色: {config.roleName}, 生命值: {config.maxHealth}, 移动速度: {config.moveSpeed}");
        }
    }
#endif
    
    #endregion
}
