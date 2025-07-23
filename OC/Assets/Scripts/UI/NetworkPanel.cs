using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using Mirror;

public class NetworkPanel : UIPanel
{
    [SerializeField] private TMP_InputField ipAddressInput;
    private TMP_InputField portInput;
    

    private GameNetworkManager networkManager;

    public override void OnOpen()
    {
        // 查找网络管理器
        networkManager = FindObjectOfType<GameNetworkManager>();
        if (networkManager == null)
        {
            Debug.LogError("GameNetworkManager 未找到，请确保场景中有一个 GameNetworkManager 对象！");
            return;
        }
        else
        {
            Debug.Log("GameNetworkManager 找到！");
        }

        ipAddressInput = transform.Find("IP").GetComponent<TMP_InputField>();
        portInput = transform.Find("Port").GetComponent<TMP_InputField>();


        // 设置默认IP为本地
        if (ipAddressInput != null) 
        {
            ipAddressInput.text = "localhost";
        }
        else
        {
            Debug.LogError("ipAddressInput 未绑定，请检查 Inspector 设置！");
        }

        // 绑定按钮事件
        RegisterButton("Back", OnBackButtonClick);
        RegisterButton("Host", OnHostButtonClick);
        RegisterButton("Join", OnJoinButtonClick);

        // **订阅客户端连接成功事件（使用 Mirror 的 OnConnected 事件）**
        Transport.active.OnClientConnected += OnClientConnected;
        //Debug.Log("已订阅 OnConnectedEvent 事件");
    }
     
    private void OnHostButtonClick()
    {
        if (networkManager != null)
        {
            networkManager.StartHostGame();
        }
    }

    private void OnJoinButtonClick()
    {
        if (networkManager != null && ipAddressInput != null && !string.IsNullOrEmpty(ipAddressInput.text))
        {
            networkManager.StartClientGame(ipAddressInput.text,int.Parse(portInput.text));
        }
    }

    private async Task OnBackButtonClick()
    {
        if (UIManager.Instance != null)
        {
            await UIManager.Instance.OpenPanelAsync<MainPanel>("MainPanel");
            UIManager.Instance.ClosePanel("NetworkPanel");
        }
        else
        {
            Debug.LogError("UIManager.Instance 未初始化，请检查 UIManager 设置！");
        }
    }

    // **修改：去掉参数，直接使用无参版本**
    private void OnClientConnected()
    {
        Debug.Log("客户端成功连接到服务器！");

        // 确保 UIManager 实例存在并关闭 NetworkPanel
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ClosePanel("NetworkPanel");
        }
        else 
        {
            Debug.LogError("UIManager.Instance 未初始化，无法关闭 NetworkPanel！");
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        // **取消订阅事件（使用 -=）**
        Transport.active.OnClientConnected -= OnClientConnected;
    }
}