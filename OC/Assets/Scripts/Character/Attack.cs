using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("攻击参数")]
    public GameObject bulletPrefab; // 子弹预制体
    public float bulletSpeed = 10f;
    public float attackInterval = 0.3f; // 射击间隔（秒）

    private float lastAttackTime = -999f;
    private Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void OnEnable()
    {
        // 订阅击杀里程碑事件
        if (GameApplication.Instance != null)
            GameApplication.Instance.OnKillMilestone += OnKillMilestone;
    }

    void OnDisable()
    {
        // 取消订阅，防止内存泄漏
        if (GameApplication.Instance != null)
            GameApplication.Instance.OnKillMilestone -= OnKillMilestone;
    }

    void Update()
    {
        // 长按鼠标左键攻击
        if (Input.GetMouseButton(0) && Time.time >= lastAttackTime + attackInterval)
        {
            Shoot();
            lastAttackTime = Time.time;
        }
    }

    void Shoot()
    {
        // 获取鼠标世界坐标
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        // 计算方向
        Vector2 dir = (mouseWorldPos - transform.position).normalized;

        // 实例化子弹
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = dir * bulletSpeed;
        }
        // 设置子弹朝向
        bullet.transform.right = dir;
    }

    // 事件回调：每10个击杀减少射击间隔
    private void OnKillMilestone(int milestone) 
    {
        attackInterval *= 0.75f;
        attackInterval = Mathf.Max(attackInterval, 0.05f); // 可设置下限
    }
}
