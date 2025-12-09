using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EntityFactory : MonoBehaviour
{
    public static EntityFactory Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async Task<GameObject> CreatEntityAsync(string address, bool usePool = false)
    {
        if (usePool && ObjectPoolManager.Instance.HasPool(address))
        {
            return ObjectPoolManager.Instance.GetObject(address);
        }

        GameObject prefab = await ResourceManager.Instance.LoadResourceAsync<GameObject>(address);

        if(prefab != null)
        {
            if (usePool)
            {
                ObjectPoolManager.Instance.CreatePool(address, prefab);
                return ObjectPoolManager.Instance.GetObject(address);
            }

            return Instantiate(prefab);
        }
        else
        {
            Debug.LogError($"Éú³ÉÊ§°Ü{address}");
            return null;
        }
    }
}
