/// <summary>
/// ����ؽӿڣ�ʵ�ִ˽ӿڵĶ�����Ա�����ع���
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// ������ӳ���ȡ��ʱ����
    /// </summary>
    void OnGetFromPool();

    /// <summary>
    /// �����󷵻س���ʱ����
    /// </summary>
    void OnReturnToPool();
}