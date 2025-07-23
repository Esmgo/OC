using UnityEngine;

public class EnemyShooter : Enemy
{
    [Header("射击参数")]
    public float shootDistance = 5f;         // 距离玩家多远开始射击
    public float shootInterval = 1.2f;       // 射击间隔
    public float bulletSpeed = 7f;           // 子弹速度
    public GameObject bulletPrefab;          // 子弹预制体

    private float lastShootTime = -999f;

    protected override void Update()
    { 
        base.Update();

        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > shootDistance)
        {
            // 继续靠近玩家
            Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        }
        else
        {
            // 停止移动并朝玩家射击
            if (Time.time >= lastShootTime + shootInterval)
            {
                ShootAtPlayer();
                lastShootTime = Time.time;
            }
        }
    }

    private void ShootAtPlayer()
    {
        if (player == null) return;

        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        GameObject bullet = BulletPool.Instance.GetBullet(transform.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = dir * bulletSpeed;
        }
        bullet.transform.right = dir;
    }
}