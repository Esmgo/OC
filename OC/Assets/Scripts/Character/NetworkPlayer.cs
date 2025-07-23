using Mirror;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    // 引用本地玩家组件
    private Attack attackComponent;
    private Move moveComponent;

    // 同步变量
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
        // 如果不是本地玩家，禁用本地控制组件
        if (!isLocalPlayer)
        {
            if (moveComponent) moveComponent.enabled = false;
            if (attackComponent) attackComponent.enabled = false;
            return;
        }

        // 是本地玩家，设置摄像机和敌人管理器
        Camera.main.GetComponent<CameraFollow>().target = transform;

        if (EnemyManager.Instance)
        {
            EnemyManager.Instance.role = transform;

            // 如果是服务端，启用敌人生成
            if (isServer)
            {
                EnemyManager.Instance.SetSpawnEnabled(true);
            }
        }

        // 异步初始化武器
        if (attackComponent)
        {
            await attackComponent.InitAsync("Bullet-Circle", 10f, 0.3f, null);
        }
    }

    // 当本地玩家击杀时调用
    [Command]
    public void CmdAddKill()
    {
        // 在服务器上增加击杀数
        //networkKillCount++;

        // 通知所有客户端更新击杀数
        //RpcUpdateKillCount(networkKillCount);
    }

    //// 在所有客户端更新击杀数
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

    // SyncVar钩子方法
    void OnKillCountChanged(int oldValue, int newValue)
    {
        Debug.Log($"击杀数更新: {oldValue} -> {newValue}");
    }
}