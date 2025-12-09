using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using GameEvents;
using System;

/// <summary>
/// 角色列表，负责加载和显示所有角色
/// </summary>
public class RoleList : MonoBehaviour
{  
    [Tooltip("角色图标预制体")]
    private GameObject roleIconPrefab;
    
    [Tooltip("角色图标的父容器")]
    [SerializeField]private Transform roleIconContainer;
    
    // 已加载的角色配置列表
    private List<CharacterConfiguration> loadedRoleConfigs = new List<CharacterConfiguration>();
    
    // 创建的角色图标列表
    private List<RoleIcon> roleIcons = new List<RoleIcon>();
        
    // 初始化状态
    private bool isInitialized = false;

    public Action<CharacterConfiguration> OnCharacterSelected;
    
    /// <summary>
    /// 初始化角色列表
    /// </summary>
    public async void Init()
    {
        if (isInitialized) return;

        roleIconPrefab = await ResourceManager.Instance.LoadResourceAsync<GameObject>("RoleIcon");
        loadedRoleConfigs = await ResourceManager.Instance.LoadResourcesByLabelAsync<CharacterConfiguration>("characterConfiguration");

        CreateRoleIcons();
        isInitialized = true;

        // 默认选中第一个角色
        if (loadedRoleConfigs.Count > 0)
        {
            SelectRole(loadedRoleConfigs[0]);
        }
    }

    /// <summary>
    /// 创建角色图标UI
    /// </summary>
    private void CreateRoleIcons()
    {
        ClearRoleIcons();

        if (roleIconPrefab == null || roleIconContainer == null)
        {
            Debug.LogError("RoleList: roleIconPrefab 或 roleIconContainer 未设置");
            return;
        }

        foreach(var config in loadedRoleConfigs)
        {
            GameObject iconObj = Instantiate(roleIconPrefab, roleIconContainer);
            RoleIcon roleIcon = iconObj.GetComponent<RoleIcon>();
            roleIcon.Init(config);

            roleIcon.OnCharacterSelected = SelectRole;

            roleIcons.Add(roleIcon);
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

    public void SelectRole(CharacterConfiguration config)
    {
        foreach (var icon in roleIcons)
        {
            icon.SetSelected(icon.roleName == config.roleName);
        }
        OnCharacterSelected?.Invoke(config);
    }
}
