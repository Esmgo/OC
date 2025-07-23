using System.Collections.Generic;
using UnityEngine;

public class HitEffectPool : MonoBehaviour
{
    public static HitEffectPool Instance { get; private set; }

    public GameObject hitEffectPrefab;
    public int poolSize = 20;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(hitEffectPrefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public void PlayEffect(Vector3 position)
    {
        GameObject effect = pool.Count > 0 ? pool.Dequeue() : Instantiate(hitEffectPrefab, transform);
        effect.transform.position = position;
        effect.SetActive(true);
        var particle = effect.GetComponent<ParticleSystem>();
        if (particle != null)
        {
            particle.Play();
            StartCoroutine(ReleaseAfter(particle.main.duration, effect));
        }
        else
        {
            // 如果不是ParticleSystem，1秒后回收
            StartCoroutine(ReleaseAfter(1f, effect));
        }
    }

    private System.Collections.IEnumerator ReleaseAfter(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}