using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using GameEvents;

/// <summary>
/// ��ɫ�б��������������غ���ʾ���н�ɫ
/// </summary>
public class RoleList : MonoBehaviour
{  
    [Header("��ɫ�б�����")]
    [Tooltip("��ɫ���õ�Addressables��ǩ/����")]
    public string roleGroupLabel = "RoleConfiguration";
    
    [Tooltip("��ɫͼ��Ԥ����")]
    public GameObject roleIconPrefab;
    
    [Tooltip("��ɫͼ��ĸ�����")]
    public Transform roleIconContainer;
    
    // �Ѽ��صĽ�ɫ�����б�
    private List<RoleConfiguration> loadedRoleConfigs = new List<RoleConfiguration>();
    
    // �����Ľ�ɫͼ���б�
    private List<RoleIcon> roleIcons = new List<RoleIcon>();
    
    // Addressables�������
    private AsyncOperationHandle<IList<RoleConfiguration>> roleConfigsHandle;
    
    // ��ʼ��״̬
    private bool isInitialized = false;
    
    /// <summary>
    /// ��ʼ����ɫ�б�
    /// </summary>
    public async void Init()
    {
        if (isInitialized)
        {
            Debug.LogWarning("RoleList �Ѿ���ʼ������");
            return;
        }
        Debug.Log("��ʼ��ʼ����ɫ�б�...");
        
        // �����¼�
        SubscribeToEvents();
        
        // �������н�ɫ����
        await LoadAllRoleConfigurations();
        
        // ������ɫͼ��
        CreateRoleIcons();
        
        isInitialized = true;
        
        Debug.Log($"��ɫ�б��ʼ����ɣ������� {loadedRoleConfigs.Count} ����ɫ");
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
    /// ʹ��Addressables����ͬһ�����µ����н�ɫ����
    /// </summary>
    private async Task LoadAllRoleConfigurations()
    {
        try
        {
            Debug.Log($"���ڼ��ط��� '{roleGroupLabel}' �е����н�ɫ����...");
            
            // ʹ��Addressables.LoadAssetsAsync����ָ����ǩ�µ�������Դ
            roleConfigsHandle = Addressables.LoadAssetsAsync<RoleConfiguration>(roleGroupLabel,null/*�ص���������ѡ��*/);
            await roleConfigsHandle.Task;   // �ȴ��������

            if (roleConfigsHandle.Status == AsyncOperationStatus.Succeeded)
            {
                loadedRoleConfigs.Clear();
                
                // �������ص���Դ
                foreach (var roleConfig in roleConfigsHandle.Result)
                {
                    if (roleConfig != null)
                    {
                        loadedRoleConfigs.Add(roleConfig);
                        Debug.Log($"�ɹ����ؽ�ɫ����: {roleConfig.roleName}");
                    }
                    else
                    {
                        Debug.LogWarning($"��ɫ������֤ʧ��: {roleConfig?.roleName ?? "Unknown"}");
                    }
                }
                
                Debug.Log($"���������ɣ������� {loadedRoleConfigs.Count} ����Ч��ɫ����");
            }
            else
            {
                Debug.LogError($"���ؽ�ɫ���÷���ʧ��: {roleGroupLabel}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"���ؽ�ɫ����ʱ�����쳣: {e.Message}");
        }
    }
    
    /// <summary>
    /// ������ɫͼ��UI
    /// </summary>
    private void CreateRoleIcons()
    {
        if (roleIconPrefab == null || roleIconContainer == null)
        {
            Debug.LogError("RoleList: roleIconPrefab �� roleIconContainer δ����");
            return;
        }
        
        // ��������ͼ��
        ClearRoleIcons();
        
        // Ϊÿ����ɫ���ô���ͼ��
        for (int i = 0; i < loadedRoleConfigs.Count; i++)
        {
            var roleConfig = loadedRoleConfigs[i];
            CreateRoleIcon(roleConfig, i);
        }
        
        // ѡ�е�һ����ɫ������еĻ���
        if (roleIcons.Count > 0)
        {
            EventCenter.Publish<RoleSelectedEvent, RoleConfiguration>(loadedRoleConfigs[0]);
        }
    }
    
    /// <summary>
    /// ����������ɫͼ��
    /// </summary>
    private void CreateRoleIcon(RoleConfiguration roleConfig, int index)
    {
        GameObject iconObj = Instantiate(roleIconPrefab, roleIconContainer);
        RoleIcon roleIcon = iconObj.GetComponent<RoleIcon>();
        
        if (roleIcon != null)
        {
            // ��ʼ����ɫͼ��
            roleIcon.Init(roleConfig);
            roleIcons.Add(roleIcon);            
        }
        else
        {
            Debug.LogError($"��ɫͼ��Ԥ����ȱ�� RoleIcon ���");
            Destroy(iconObj);
        }
    }

    /// <summary>
    /// �������н�ɫͼ��
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
    /// ˢ�½�ɫ�б�
    /// </summary>
    public async void RefreshRoleList()
    {
        Debug.Log("ˢ�½�ɫ�б�...");
        
        // ������������
        ClearRoleIcons();
        loadedRoleConfigs.Clear();
        
        // �ͷ�֮ǰ��Addressables���
        if (roleConfigsHandle.IsValid())
        {
            Addressables.Release(roleConfigsHandle);
        }
        
        // ���¼���
        await LoadAllRoleConfigurations();
        CreateRoleIcons();
    }
    
    /// <summary>
    /// �������ƻ�ȡ��ɫ����
    /// </summary>
    public RoleConfiguration GetRoleConfiguration(string roleName)
    {
        return loadedRoleConfigs.Find(config => config.roleName == roleName);
    }
    
    /// <summary>
    /// ��ȡ���н�ɫ����
    /// </summary>
    public List<RoleConfiguration> GetAllRoleConfigurations()
    {
        return new List<RoleConfiguration>(loadedRoleConfigs);
    }
    
    /// <summary>
    /// ��ȡ��ɫ����
    /// </summary>
    public int GetRoleCount()
    {
        return loadedRoleConfigs.Count;
    }
    
    #region �¼�����
    
    private void SubscribeToEvents()
    {
        // ���Ľ�ɫ���ù����������ü�������¼�
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
            // �����ɫ���ù��������¼��������ã�ˢ���б�
            RefreshRoleList();
        }
    }
    
    #endregion
    
    #region ��������
    
    private void OnDestroy()
    {
        // ȡ���¼�����
        UnsubscribeFromEvents();
        
        // �ͷ�Addressables��Դ
        if (roleConfigsHandle.IsValid())
        {
            Addressables.Release(roleConfigsHandle);
            Debug.Log("�ͷŽ�ɫ����Addressables��Դ");
        }
    }
    
    #endregion
    
    #region �༭�����߷���
    
#if UNITY_EDITOR
    [ContextMenu("Test Load Role Configs")]
    private async void TestLoadRoleConfigs()
    {
        await LoadAllRoleConfigurations();
    }
    
    [ContextMenu("Print Loaded Roles")]
    private void PrintLoadedRoles()
    {
        Debug.Log("=== �Ѽ��صĽ�ɫ���� ===");
        foreach (var config in loadedRoleConfigs)
        {
            Debug.Log($"��ɫ: {config.roleName}, ����ֵ: {config.maxHealth}, �ƶ��ٶ�: {config.moveSpeed}");
        }
    }
#endif
    
    #endregion
}
