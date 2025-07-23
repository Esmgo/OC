using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BulletPool : NetworkBehaviour
{
    public static BulletPool Instance { get; private set; }

    public GameObject bulletPrefab;
    public int poolSize = 30;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (GameNetworkManager.IsLANGame)
        {
            InitializePoolOnServer();
        }
        else
        {
            InitializePoolLocally();
        }
    }

    private void InitializePoolOnServer()
    {
        if (isServer)
        {
            for (int i = 0; i < poolSize; i++)
            {
                var obj = Instantiate(bulletPrefab, transform);
                obj.SetActive(false);
                NetworkServer.Spawn(obj); // 在服务器上生成并同步到客户端
                pool.Enqueue(obj);
            }
        }
    }

    private void InitializePoolLocally()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(bulletPrefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetBullet(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = pool.Count > 0 ? pool.Dequeue() : Instantiate(bulletPrefab, transform);
        bullet.transform.position = position;
        bullet.transform.rotation = rotation;
        bullet.SetActive(true);

        var bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.ResetState();
        }

        if (GameNetworkManager.IsLANGame && isServer)
        {
            NetworkServer.Spawn(bullet); // 确保新生成的子弹在局域网模式下同步到客户端
        }

        return bullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        pool.Enqueue(bullet);
    }
}