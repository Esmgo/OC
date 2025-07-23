using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// GameNetworkManager �� Mirror ����չ�࣬���ڹ�������������������߼���
/// ���������������ͻ��˵�������ֹͣ���Լ���ҵĶ�̬���غͳ����л���
/// ���༯���� Unity Addressables ϵͳ�������첽���������ɫԤ���塣
/// </summary>
[DisallowMultipleComponent]
public class GameNetworkManager : NetworkManager
{
    [Header("��Ϸ����")]
    [Tooltip("��Ϸ�������ƣ���������������ʱ������Ϸ����")]
    public string gameSceneName = "GameScene";

    [Header("Addressables ����")]
    [Tooltip("�����ɫԤ����� Addressables ���ã������첽������ҽ�ɫ")]
    public AssetReferenceGameObject networkRolePrefabReference;

    //[Header("������Ϸ�Ƿ��ѿ���")]
    //public bool isNetworkGameStarted = false;

    /// <summary>
    /// ����ģʽ��ȷ��ȫ��ֻ��һ�� GameNetworkManager ʵ����
    /// </summary>
    public static GameNetworkManager Instance { get; private set; }

    public static bool IsLANGame { get; private set; } = false; // �Ƿ�Ϊ��������Ϸ

    /// <summary>
    /// ��ʼ������ģʽ��ȷ�� GameNetworkManager �ڳ�����Ψһ��
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

        // ȷ�� Transport.active ����ȷ����
        if (Transport.active == null)
        {
            Transport.active = GetComponent<Transport>();
            if (Transport.active == null)
            {
                Debug.LogError("δ�ҵ� Transport �������ȷ���� GameNetworkManager ���ڵ� GameObject ������� Transport �����");
            }
        }
    }

    #region �������¼�

    /// <summary>
    /// ��������������ʱ���á�
    /// ʹ�� Addressables �첽������ҽ�ɫԤ���壬������ʵ������ָ��λ�á�
    /// </summary>
    /// <param name="conn">��ҵ��������Ӷ���</param>
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // ��ȡ��ҵ�����λ��
        Transform startPos = GetStartPosition();
        Vector3 position = startPos != null ? startPos.position : Vector3.zero;

        // ����Ƿ��Ѿ������������ɫԤ����
        if (networkRolePrefabReference.OperationHandle.IsValid())
        {
            //Debug.LogWarning("�����ɫԤ�����Ѿ����أ�ֱ��ʹ���Ѽ��ص�ʵ����");
            GameObject playerPrefab = (GameObject)networkRolePrefabReference.OperationHandle.Result;
            GameObject player = Instantiate(playerPrefab, position, Quaternion.identity);
            player.name = $"Player_{conn.connectionId}";

            // Ϊ��ҷ�������
            NetworkServer.AddPlayerForConnection(conn, player);
        }
        else
        {
            // ʹ�� Addressables �첽���ؽ�ɫԤ����
            networkRolePrefabReference.LoadAssetAsync().Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    //Debug.Log("���������ɫԤ����ɹ�");
                    GameObject playerPrefab = handle.Result;
                    GameObject player = Instantiate(playerPrefab, position, Quaternion.identity);
                    player.name = $"Player_{conn.connectionId}";

                    // Ϊ��ҷ�������
                    NetworkServer.AddPlayerForConnection(conn, player);
                }
                else
                {
                    Debug.LogError($"���������ɫԤ����ʧ��: {handle.OperationException}");
                }
            };
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        GameNetworkManager.IsLANGame = true; // ����Ϊ������ģʽ
        Debug.Log("��������������������ģʽ");
    }

    #endregion

    #region �ͻ����¼�

    public override void OnStartClient()
    {
        base.OnStartClient();
        GameNetworkManager.IsLANGame = true; // ����Ϊ������ģʽ
        Debug.Log("�ͻ��������ӣ�������ģʽ");
    }

    #endregion

    #region �Զ��巽��
     
    /// <summary>
    /// ����������������+�ͻ��ˣ�����������Ϸ������
    /// </summary>
    public void StartHostGame()
    {
        SetLANGame(true); // ����Ϊ��������Ϸ
        // �����������ص�ַ
        networkAddress = "127.0.0.1"; // ���� IP
        Debug.Log("�������������ȴ��ͻ�������...");
        StartHost();
        SceneManager.LoadSceneAsync(gameSceneName);
        
    }

    /// <summary>
    /// �����ͻ��ˣ������ӵ�ָ���ķ����� IP ��ַ�Ͷ˿ڡ�
    /// </summary>
    /// <param name="ipAddress">�������� IP ��ַ</param>
    /// <param name="port">�������Ķ˿�</param>
    public void StartClientGame(string ipAddress, int port)
    {
        // ����Զ�� IP �Ͷ˿�
        networkAddress = ipAddress;
        TelepathyTransport transport = Transport.active as TelepathyTransport;
        if (transport != null)
        {
            transport.port = (ushort)port; // ����Զ�̶˿�
        }
        Debug.Log($"�������ӵ���������{ipAddress}:{port}");
        StartClient();
    }

    /// <summary>
    /// ֹͣ���������������������ͻ��ˣ������������˵�������
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
        SetLANGame(false); // ���þ�������Ϸ״̬
        SceneManager.LoadScene("MainMenu"); // �������˵�����
    }

    public void SetLANGame(bool isLAN)
    {
        IsLANGame = isLAN;
    }

    public void StartOfflineGame()
    {
        GameNetworkManager.IsLANGame = false; // ����Ϊ�Ǿ�����ģʽ
        Debug.Log("����������Ϸģʽ");
    }

    #endregion
}