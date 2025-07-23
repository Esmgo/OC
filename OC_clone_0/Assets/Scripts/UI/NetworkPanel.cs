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
        // �������������
        networkManager = FindObjectOfType<GameNetworkManager>();
        if (networkManager == null)
        {
            Debug.LogError("GameNetworkManager δ�ҵ�����ȷ����������һ�� GameNetworkManager ����");
            return;
        }
        else
        {
            Debug.Log("GameNetworkManager �ҵ���");
        }

        ipAddressInput = transform.Find("IP").GetComponent<TMP_InputField>();
        portInput = transform.Find("Port").GetComponent<TMP_InputField>();


        // ����Ĭ��IPΪ����
        if (ipAddressInput != null) 
        {
            ipAddressInput.text = "localhost";
        }
        else
        {
            Debug.LogError("ipAddressInput δ�󶨣����� Inspector ���ã�");
        }

        // �󶨰�ť�¼�
        RegisterButton("Back", OnBackButtonClick);
        RegisterButton("Host", OnHostButtonClick);
        RegisterButton("Join", OnJoinButtonClick);

        // **���Ŀͻ������ӳɹ��¼���ʹ�� Mirror �� OnConnected �¼���**
        Transport.active.OnClientConnected += OnClientConnected;
        //Debug.Log("�Ѷ��� OnConnectedEvent �¼�");
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
            Debug.LogError("UIManager.Instance δ��ʼ�������� UIManager ���ã�");
        }
    }

    // **�޸ģ�ȥ��������ֱ��ʹ���޲ΰ汾**
    private void OnClientConnected()
    {
        Debug.Log("�ͻ��˳ɹ����ӵ���������");

        // ȷ�� UIManager ʵ�����ڲ��ر� NetworkPanel
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ClosePanel("NetworkPanel");
        }
        else 
        {
            Debug.LogError("UIManager.Instance δ��ʼ�����޷��ر� NetworkPanel��");
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        // **ȡ�������¼���ʹ�� -=��**
        Transport.active.OnClientConnected -= OnClientConnected;
    }
}