using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// 一些小工具
/// </summary>
public static class Tools 
{
    public static int seed { private set; get; }
    /// <summary>
    /// 工具类初始化
    /// </summary>
    public static void Init()
    {
        seed = GenerateRandomSeed();
        Random.InitState(seed);
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

    public static float GetAngle(Vector2 from, Vector2 to)
    {
        return (Vector2.SignedAngle(Vector2.right, to - from) + 360) % 360;
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
}
