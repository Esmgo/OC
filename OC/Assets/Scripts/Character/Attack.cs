using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("��������")]
    public GameObject bulletPrefab; // �ӵ�Ԥ����
    public float bulletSpeed = 10f;
    public float attackInterval = 0.3f; // ���������룩

    private float lastAttackTime = -999f;
    private Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void OnEnable()
    {
        // ���Ļ�ɱ��̱��¼�
        if (GameApplication.Instance != null)
            GameApplication.Instance.OnKillMilestone += OnKillMilestone;
    }

    void OnDisable()
    {
        // ȡ�����ģ���ֹ�ڴ�й©
        if (GameApplication.Instance != null)
            GameApplication.Instance.OnKillMilestone -= OnKillMilestone;
    }

    void Update()
    {
        // ��������������
        if (Input.GetMouseButton(0) && Time.time >= lastAttackTime + attackInterval)
        {
            Shoot();
            lastAttackTime = Time.time;
        }
    }

    void Shoot()
    {
        // ��ȡ�����������
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        // ���㷽��
        Vector2 dir = (mouseWorldPos - transform.position).normalized;

        // ʵ�����ӵ�
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = dir * bulletSpeed;
        }
        // �����ӵ�����
        bullet.transform.right = dir;
    }

    // �¼��ص���ÿ10����ɱ����������
    private void OnKillMilestone(int milestone) 
    {
        attackInterval *= 0.75f;
        attackInterval = Mathf.Max(attackInterval, 0.05f); // ����������
    }
}
