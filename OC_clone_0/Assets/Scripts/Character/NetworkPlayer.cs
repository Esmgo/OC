using Mirror;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    // ���ñ���������
    private Attack attackComponent;
    private Move moveComponent;

    // ͬ������
    //[SyncVar(hook = nameof(OnKillCountChanged))]
    //private int networkKillCount = 0;

    private void Awake()
    {
        attackComponent = GetComponentInChildren<Attack>();
        moveComponent = GetComponent<Move>();
    }

    private void Start()
    {
        _ = StartAsync();
    }

    private async Task StartAsync()
    {
        // ������Ǳ�����ң����ñ��ؿ������
        if (!isLocalPlayer)
        {
            if (moveComponent) moveComponent.enabled = false;
            if (attackComponent) attackComponent.enabled = false;
            return;
        }

        // �Ǳ�����ң�����������͵��˹�����
        Camera.main.GetComponent<CameraFollow>().target = transform;

        if (EnemyManager.Instance)
        {
            EnemyManager.Instance.role = transform;

            // ����Ƿ���ˣ����õ�������
            if (isServer)
            {
                EnemyManager.Instance.SetSpawnEnabled(true);
            }
        }

        // �첽��ʼ������
        if (attackComponent)
        {
            await attackComponent.InitAsync("Bullet-Circle", 10f, 0.3f, null);
        }
    }

    // ��������һ�ɱʱ����
    [Command]
    public void CmdAddKill()
    {
        // �ڷ����������ӻ�ɱ��
        //networkKillCount++;

        // ֪ͨ���пͻ��˸��»�ɱ��
        //RpcUpdateKillCount(networkKillCount);
    }

    //// �����пͻ��˸��»�ɱ��
    //[ClientRpc]
    //void RpcUpdateKillCount(int newKillCount)
    //{
    //    if (isLocalPlayer && GameApplication.Instance != null)
    //    {
    //        GameApplication.Instance.killCount = newKillCount;

    //        if (newKillCount % 10 == 0)
    //        {
    //            //GameApplication.Instance.OnKillMilestone?.Invoke(newKillCount / 10);
    //        }
    //    }
    //}

    // SyncVar���ӷ���
    void OnKillCountChanged(int oldValue, int newValue)
    {
        Debug.Log($"��ɱ������: {oldValue} -> {newValue}");
    }
}