using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // 活动面板
    private Dictionary<string, UIPanel> activePanels = new Dictionary<string, UIPanel>();
    // UI父节点
    public Transform uiRoot;

    public async void Init()
    {
        await ResourceManager.Instance.LoadResourcesByLabelAsync<GameObject>("UI");
    }

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
    /// 打开面板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="panelName">面板名，与AA地址相同</param>
    /// <returns></returns>
    public async Task<T> OpenPanelAsync<T>(string panelName) where T : UIPanel
    {
        if (activePanels.ContainsKey(panelName))
        {
            activePanels[panelName].gameObject.SetActive(true);
            activePanels[panelName].OnOpen();
            return activePanels[panelName] as T;
        }

        GameObject go = Instantiate(await ResourceManager.Instance.LoadResourceAsync<GameObject>(panelName, "UI"), uiRoot);

        T panel = go.GetComponent<T>();
        if (panel == null)
            panel = go.AddComponent<T>();
        activePanels.Add(panelName, panel);
        panel.OnOpen();
        return panel;
    }

    // 关闭UI面板
    public void ClosePanel(string panelName)
    {
        if (activePanels.TryGetValue(panelName, out UIPanel panel))
        {
            panel.OnClose();
            panel.gameObject.SetActive(false);
        }
    }

    // 销毁UI面板
    public void DestroyPanel(string panelName)
    {
        if (activePanels.TryGetValue(panelName, out UIPanel panel))
        {
            Destroy(panel.gameObject);
            activePanels.Remove(panelName);
        }
    }
}