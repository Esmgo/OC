using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // Addressables���keyע���
    private Dictionary<string, string> panelKeys = new Dictionary<string, string>();
    // ����
    private Dictionary<string, UIPanel> activePanels = new Dictionary<string, UIPanel>();
    // UI���ڵ�
    public Transform uiRoot;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (uiRoot == null)
        {
            var canvas = FindObjectOfType<Canvas>();
            uiRoot = canvas != null ? canvas.transform : this.transform;
        }
    }

    /// <summary>
    /// ע��UI����Addressable key
    /// </summary>
    public void RegisterPanel(string panelName, string addressableKey)
    {
        if (!panelKeys.ContainsKey(panelName))
            panelKeys.Add(panelName, addressableKey);
    }

    /// <summary>
    /// �첽��UI���
    /// </summary>
    public async Task<T> OpenPanelAsync<T>(string panelName) where T : UIPanel
    {
        if (activePanels.ContainsKey(panelName))
        {
            activePanels[panelName].gameObject.SetActive(true);
            return activePanels[panelName] as T;
        }

        if (!panelKeys.ContainsKey(panelName))
        {
            Debug.LogError($"UIManager: δע����� {panelName}");
            return null;
        }
         
        string addressableKey = panelKeys[panelName];

        // Addressables�첽����
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(addressableKey);
        await handle.Task;
        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"UIManager: Addressables ����ʧ�� {addressableKey}");
            return null;
        }

        GameObject go = Instantiate(handle.Result, uiRoot);
        T panel = go.GetComponent<T>();
        if (panel == null)
            panel = go.AddComponent<T>();
        activePanels.Add(panelName, panel);
        panel.OnOpen();
        return panel;
    }

    // �ر�UI���
    public void ClosePanel(string panelName)
    {
        if (activePanels.TryGetValue(panelName, out UIPanel panel))
        {
            panel.OnClose();
            panel.gameObject.SetActive(false);
        }
    }

    // ����UI���
    public void DestroyPanel(string panelName)
    {
        if (activePanels.TryGetValue(panelName, out UIPanel panel))
        {
            Destroy(panel.gameObject);
            activePanels.Remove(panelName);
        }
    }
}