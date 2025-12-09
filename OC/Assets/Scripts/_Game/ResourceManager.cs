using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

/// <summary>
/// 资源管理器
/// </summary>
public class ResourceManager : MonoBehaviour
{
    #region 单例
    public static ResourceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    [Tooltip("已加载资源资源分类列表")]
    private Dictionary<string, Dictionary<string, Object>> categorizedResourceCache = new Dictionary<string, Dictionary<string, Object>>();

    public async void Init()
    {
        //预加载资源
        await LoadResourcesByLabelAsync<CharacterConfiguration>("characterConfiguration");
        await LoadResourcesByLabelAsync<EnemyConfiguration>("enemyConfiguration");
    }

    /// <summary>
    /// 异步加载资源并缓存到分类字典
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="label">资源标签</param>
    /// <param name="address">资源地址</param>
    /// <returns>加载的资源</returns>
    public async Task<T> LoadResourceAsync<T>(string address, string label = "default") where T : Object
    {
        if (!categorizedResourceCache.ContainsKey(label))
        {
            categorizedResourceCache[label] = new Dictionary<string, Object>();
        }

        if (categorizedResourceCache.ContainsKey(address))
        {
            return categorizedResourceCache[label][address] as T;
        }

        var handle = Addressables.LoadAssetAsync<T>(address);
        await handle.Task;

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            categorizedResourceCache[label][address] = handle.Result;
            return handle.Result;
        }
        else
        {
            Debug.LogError($"加载资源失败:{label}/{address}");
            return null;
        }
    }

    /// <summary>
    /// 按标签异步加载资源列表并缓存到分类字典
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="label">标签</param>
    /// <returns>资源列表</returns>
    public async Task<List<T>> LoadResourcesByLabelAsync<T>(string label) where T : Object
    {
        if (!categorizedResourceCache.ContainsKey(label))
        {
            categorizedResourceCache[label] = new Dictionary<string, Object>();
        }

        IList<IResourceLocation> locations = await Addressables.LoadResourceLocationsAsync(label).Task;

        if (locations == null || locations.Count == 0)
        {
            Debug.LogError($"未找到标签对应的资源: {label}");
            return null;
        }

        List<T> loadedResources = new List<T>();

        foreach (var location in locations)
        {
            string address = location.PrimaryKey;
            if (!categorizedResourceCache[label].ContainsKey(address))
            {
                var handle = Addressables.LoadAssetAsync<T>(address);
                await handle.Task;
                if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    categorizedResourceCache[label][address] = handle.Result;
                    loadedResources.Add(handle.Result);
                }
                else
                {
                    Debug.LogError($"加载资源失败: {address}");
                }
            }
            else
            {
                loadedResources.Add(categorizedResourceCache[label][address] as T);
            }
        }
        return loadedResources;
    }

    /// <summary>
    /// 释放单个资源
    /// </summary>
    /// <param name="label"></param>
    /// <param name="address"></param>
    public void ReleaseResource(string label, string address)
    {
        if (categorizedResourceCache.TryGetValue(label, out var innerDict)
            && innerDict != null
            && innerDict.ContainsKey(address))
        {
            Addressables.Release(innerDict[address]);
            innerDict.Remove(address);
        }
        else
        {
            Debug.LogWarning($"尝试释放未缓存的资源: {label} - {address}");
        }
    }

    /// <summary>
    /// 按标签释放资源分类
    /// </summary>
    /// <param name="label"></param>
    public void ReleaseResourcesByLabel(string label)
    {
        if (categorizedResourceCache.TryGetValue(label, out var innerDict)
            && innerDict != null)
        {
            foreach (var resource in innerDict.Values)
            {
                Addressables.Release(resource);
            }
            innerDict.Clear();
        }
        else
        {
            Debug.LogWarning($"尝试释放未缓存的资源分类: {label}");
        }
    }

    /// <summary>
    /// 释放所有资源
    /// </summary>
    public void ReleaseAllResources()
    {
        foreach (var label in categorizedResourceCache.Keys)
        {
            ReleaseResourcesByLabel(label);
        }

        categorizedResourceCache.Clear();
    }

    /// <summary>
    /// 检查资源是否已加载
    /// </summary>
    /// <param name="label">资源标签</param>
    /// <param name="resourcePath">资源地址</param>
    /// <returns>是否已加载</returns>
    public bool IsResourceLoaded(string label, string resourcePath)
    {
        return categorizedResourceCache.ContainsKey(label) && categorizedResourceCache[label].ContainsKey(resourcePath);
    }
}
