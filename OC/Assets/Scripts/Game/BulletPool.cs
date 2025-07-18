using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
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

        // ÖØÖÃ×Óµ¯×´Ì¬
        var bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.ResetState();
        }

        return bullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        pool.Enqueue(bullet);
    }
}