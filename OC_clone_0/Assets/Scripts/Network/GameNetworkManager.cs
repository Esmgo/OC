using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// GameNetworkManager 是 Mirror 的扩展类，用于管理局域网联机的网络逻辑。
/// 它负责处理主机、客户端的启动与停止，以及玩家的动态加载和场景切换。
/// 此类集成了 Unity Addressables 系统，用于异步加载网络角色预制体。
/// </summary>
[DisallowMultipleComponent]
public class GameNetworkManager : NetworkManager
{
    [Header("游戏设置")]
    [Tooltip("游戏场景名称，用于在主机启动时加载游戏场景")]
    public string gameSceneName = "GameScene";

    [Header("Addressables 设置")]
    [Tooltip("网络角色预制体的 Addressables 引用，用于异步加载玩家角色")]
    public AssetReferenceGameObject networkRolePrefabReference;

    //[Header("网络游戏是否已开启")]
    //public bool isNetworkGameStarted = false;

    /// <summary>
    /// 单例模式，确保全局只有一个 GameNetworkManager 实例。
    /// </summary>
    public static GameNetworkManager Instance { get; private set; }

    public static bool IsLANGame { get; private set; } = false; // 是否为局域网游戏

    /// <summary>
    /// 初始化单例模式，确保 GameNetworkManager 在场景中唯一。
    /// </summary>
    public override void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        // 确保 Transport.active 被正确设置
        if (Transport.active == null)
        {
            Transport.active = GetComponent<Transport>();
            if (Transport.active == null)
            {
                Debug.LogError("未找到 Transport 组件，请确保在 GameNetworkManager 所在的 GameObject 上添加了 Transport 组件。");
            }
        }
    }

    #region 服务器事件

    /// <summary>
    /// 当服务器添加玩家时调用。
    /// 使用 Addressables 异步加载玩家角色预制体，并将其实例化到指定位置。
    /// </summary>
    /// <param name="conn">玩家的网络连接对象</param>
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // 获取玩家的生成位置
        Transform startPos = GetStartPosition();
        Vector3 position = startPos != null ? startPos.position : Vector3.zero;

        // 检查是否已经加载了网络角色预制体
        if (networkRolePrefabReference.OperationHandle.IsValid())
        {
            //Debug.LogWarning("网络角色预制体已经加载，直接使用已加载的实例。");
            GameObject playerPrefab = (GameObject)networkRolePrefabReference.OperationHandle.Result;
            GameObject player = Instantiate(playerPrefab, position, Quaternion.identity);
            player.name = $"Player_{conn.connectionId}";

            // 为玩家分配连接
            NetworkServer.AddPlayerForConnection(conn, player);
        }
        else
        {
            // 使用 Addressables 异步加载角色预制体
            networkRolePrefabReference.LoadAssetAsync().Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    //Debug.Log("加载网络角色预制体成功");
                    GameObject playerPrefab = handle.Result;
                    GameObject player = Instantiate(playerPrefab, position, Quaternion.identity);
                    player.name = $"Player_{conn.connectionId}";

                    // 为玩家分配连接
                    NetworkServer.AddPlayerForConnection(conn, player);
                }
                else
                {
                    Debug.LogError($"加载网络角色预制体失败: {handle.OperationException}");
                }
            };
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        GameNetworkManager.IsLANGame = true; // 设置为局域网模式
        Debug.Log("服务器已启动，局域网模式");
    }

    #endregion

    #region 客户端事件

    public override void OnStartClient()
    {
        base.OnStartClient();
        GameNetworkManager.IsLANGame = true; // 设置为局域网模式
        Debug.Log("客户端已连接，局域网模式");
    }

    #endregion

    #region 自定义方法
     
    /// <summary>
    /// 启动主机（服务器+客户端），并加载游戏场景。
    /// </summary>
    public void StartHostGame()
    {
        SetLANGame(true); // 设置为局域网游戏
        // 主机监听本地地址
        networkAddress = "127.0.0.1"; // 本地 IP
        Debug.Log("主机已启动，等待客户端连接...");
        StartHost();
        SceneManager.LoadSceneAsync(gameSceneName);
        
    }

    /// <summary>
    /// 启动客户端，并连接到指定的服务器 IP 地址和端口。
    /// </summary>
    /// <param name="ipAddress">服务器的 IP 地址</param>
    /// <param name="port">服务器的端口</param>
    public void StartClientGame(string ipAddress, int port)
    {
        // 设置远程 IP 和端口
        networkAddress = ipAddress;
        TelepathyTransport transport = Transport.active as TelepathyTransport;
        if (transport != null)
        {
            transport.port = (ushort)port; // 设置远程端口
        }
        Debug.Log($"尝试连接到服务器：{ipAddress}:{port}");
        StartClient();
    }

    /// <summary>
    /// 停止网络服务（主机、服务器或客户端），并返回主菜单场景。
    /// </summary>
    public void StopNetwork()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            StopHost();
        }
        else if (NetworkServer.active)
        {
            StopServer();
        }
        else if (NetworkClient.isConnected)
        {
            StopClient();
        }
        SetLANGame(false); // 重置局域网游戏状态
        SceneManager.LoadScene("MainMenu"); // 加载主菜单场景
    }

    public void SetLANGame(bool isLAN)
    {
        IsLANGame = isLAN;
    }

    public void StartOfflineGame()
    {
        GameNetworkManager.IsLANGame = false; // 设置为非局域网模式
        Debug.Log("启动离线游戏模式");
    }

    #endregion
}