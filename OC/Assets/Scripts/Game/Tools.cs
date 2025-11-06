using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class Tools 
{
    private static Character character;
    /// <summary>
    /// 工具类初始化
    /// </summary>
    public static void Init()
    {
        Random.InitState(GenerateRandomSeed());
        character = null;

        //增加统一的资源加载
    }

    /// <summary>
    /// 获得[min, max)范围内的随机整数
    /// </summary>
    /// <param name="min">最小</param>
    /// <param name="max">最大</param>
    /// <returns></returns>
    public static int RandomInt(int min, int max)
    {
        return Random.Range(min, max);
    }

    /// <summary>
    /// 获得[min, max)范围内的随机浮点数
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float RandomFloat(float min, float max)
    {
        return Random.Range(min, max);
    }

    // 生成随机数种子（时间戳 + 哈希增强随机性）
    private static int GenerateRandomSeed()
    {
        int seed = System.DateTime.Now.Millisecond;
        seed ^= System.Guid.NewGuid().GetHashCode(); // 增加熵
        return seed;
    }

    /// <summary>
    /// 获取鼠标相对于某个中心点的角度（0-360度，右侧为0度，逆时针增加）
    /// </summary>
    /// <param name="center"></param>
    /// <returns></returns>
    public static float GetMouseAngle(Transform center)
    {
        return (Vector2.SignedAngle(Vector2.right, Camera.main.ScreenToWorldPoint(Input.mousePosition) - center.position) + 360) % 360;
    }

    /// <summary>
    /// 获得鼠标相对于某个中心点的方向（单位向量）
    /// </summary>
    /// <param name="center"></param>
    /// <returns></returns>
    public static Vector2 GetMouseDir(Transform center)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(mousePos.x - center.position.x, mousePos.y - center.position.y).normalized;
    }

    /// <summary>
    /// 获得两个Transform之间的角度（0-360度，右侧为0度，逆时针增加）
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static float GetAngle(Transform from, Transform to)
    {
        return (Vector2.SignedAngle(Vector2.right, to.position - from.position) + 360) % 360;
    }

    /// <summary>
    /// 获得两个Transform之间的方向（单位向量）
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static Vector2 GetDir(Transform from, Transform to)
    {
        return (to.position - from.position).normalized;
    }

    /// <summary>
    /// 获得玩家角色
    /// </summary>
    /// <returns></returns>
    public static Character GetCharacter()
    {
        if (character != null)
        {
            return character;
        }
        else
        {
            Debug.LogWarning("未找到玩家角色！！");
            return null;
        }
    }

    public static void SetCharacter(Character c)
    {
        character = c;
    }

    public static PropertyModifier GetGlobalPlayerPropertyModifier()
    {
        if (GlobalStatModifier.Instance != null)
        {
            return GlobalStatModifier.Instance.GlobalModifierForPlayer;
        }
        else
        {
            Debug.LogWarning("未找到角色全局属性管理器！！");
            return null;
        }
    }


    #region AA加载
    // 全局句柄缓存字典（防止重复加载/泄漏）
    private static readonly Dictionary<string, AsyncOperationHandle> _handleCache =
        new Dictionary<string, AsyncOperationHandle>();

    /// <summary>
    /// 异步加载Addressable资源（自动缓存/释放管理）
    /// </summary>
    /// <typeparam name="T">资源类型（GameObject/Sprite等）</typeparam>
    /// <param name="address">资源地址或label</param>
    /// <param name="autoRelease">是否在场景切换时自动释放</param>
    public static async Task<T> LoadAddressable<T>(string address, bool autoRelease = true)
        where T : class
    {
        // 如果已有缓存且未失效，直接返回
        if (_handleCache.TryGetValue(address, out var existingHandle) &&
            existingHandle.IsDone &&
            existingHandle.Status == AsyncOperationStatus.Succeeded)
        {
            return existingHandle.Result as T;
        }

        // 加载资源
        var handle = Addressables.LoadAssetAsync<T>(address);
        _handleCache[address] = handle; // 记录句柄

        // 设置场景切换自动释放（可选）
        if (autoRelease)
        {
            handle.Completed += (h) =>
                UnityEngine.SceneManagement.SceneManager.sceneUnloaded += (_) => Release(address);
        }

        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            return handle.Result;
        }
        else
        {
            Debug.LogError($"加载失败: {address} - {handle.OperationException}");
            Release(address);
            return null;
        }
    }

    /// <summary>
    /// 根据标签异步加载多个Addressable资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="label">标签名称</param>
    /// <param name="autoRelease">是否在场景切换时自动释放</param>
    /// <returns>加载的资源列表</returns>
    public static async Task<List<T>> LoadAddressablesByLabel<T>(string label, bool autoRelease = true)
        where T : class
    {
        string cacheKey = $"label_{label}";
        
        // 如果已有缓存且未失效，直接返回
        if (_handleCache.TryGetValue(cacheKey, out var existingHandle) &&
            existingHandle.IsDone &&
            existingHandle.Status == AsyncOperationStatus.Succeeded)
        {
            var existingResult = existingHandle.Result as IList<T>;
            return existingResult != null ? new List<T>(existingResult) : new List<T>();
        }

        // 根据标签加载资源列表
        var handle = Addressables.LoadAssetsAsync<T>(label, null);
        _handleCache[cacheKey] = handle; // 记录句柄

        // 设置场景切换自动释放（可选）
        if (autoRelease)
        {
            handle.Completed += (h) =>
                UnityEngine.SceneManagement.SceneManager.sceneUnloaded += (_) => Release(cacheKey);
        }

        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            return new List<T>(handle.Result);
        }
        else
        {
            Debug.LogError($"根据标签加载失败: {label} - {handle.OperationException}");
            Release(cacheKey);
            return new List<T>();
        }
    }

    /// <summary>
    /// 释放指定资源
    /// </summary>
    public static void Release(string address)
    {
        if (_handleCache.TryGetValue(address, out var handle))
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
            _handleCache.Remove(address);
        }
    }

    /// <summary>
    /// 清空所有缓存的Addressable资源
    /// </summary>
    public static void ClearAddressableCache()
    {
        foreach (var kvp in _handleCache)
        {
            if (kvp.Value.IsValid())
            {
                Addressables.Release(kvp.Value);
            }
        }
        _handleCache.Clear();
    }
    #endregion
}
