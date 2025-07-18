using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 2f;
    private bool hasHit = false;
    private float timer = 0f;

    void OnEnable()
    {
        hasHit = false;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            BulletPool.Instance.ReturnBullet(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            hasHit = true;
            enemy.TakeDamage(damage);
            HitEffectPool.Instance.PlayEffect(transform.position);
            BulletPool.Instance.ReturnBullet(gameObject);
        }
    }

    public void ResetState()
    {
        hasHit = false;
        timer = 0f;
    }
}