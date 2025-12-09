/// <summary>
/// 对象池接口，实现此接口的对象可以被对象池管理
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// 当对象从池中取出时调用
    /// </summary>
    void OnGetFromPool();

    /// <summary>
    /// 当对象返回池中时调用
    /// </summary>
    void OnReturnToPool();
}