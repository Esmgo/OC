using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextPool : MonoBehaviour
{
    public static DamageTextPool Instance { get; private set; }

    public DamageText damageTextPrefab;
    public int poolSize = 20;

    private Queue<DamageText> pool = new Queue<DamageText>();

    void Awake()
    {
        Instance = this;
        for (int i = 0; i < poolSize; i++)
        {
            var dt = Instantiate(damageTextPrefab, transform); 
            dt.gameObject.SetActive(false);
            pool.Enqueue(dt);
        }
    }

    public void Show(int damage, Vector3 position)
    {
        DamageText dt = Get();
        dt.Show(damage, position);
    }

    private DamageText Get()
    {
        foreach (var dt in pool)
        {
            if (!dt.gameObject.activeInHierarchy)
                return dt;
        }
        // ³ØÂúÊ±¿ÉÀ©ÈÝ
        var newDt = Instantiate(damageTextPrefab, transform);
        pool.Enqueue(newDt);
        return newDt;
    }
}
