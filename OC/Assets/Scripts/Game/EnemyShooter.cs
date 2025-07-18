using UnityEngine;

public class EnemyShooter : Enemy
{
    [Header("�������")]
    public float shootDistance = 5f;         // ������Ҷ�Զ��ʼ���
    public float shootInterval = 1.2f;       // ������
    public float bulletSpeed = 7f;           // �ӵ��ٶ�
    public GameObject bulletPrefab;          // �ӵ�Ԥ����

    private float lastShootTime = -999f;

    protected override void Update()
    { 
        base.Update();

        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > shootDistance)
        {
            // �����������
            Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        }
        else
        {
            // ֹͣ�ƶ�����������
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