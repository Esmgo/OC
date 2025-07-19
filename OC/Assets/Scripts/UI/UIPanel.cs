using UnityEngine;
using System;
using System.Threading.Tasks;

public class UIPanel : MonoBehaviour
{
    // ��ʱ����
    public virtual void OnOpen() { }
    // �ر�ʱ����
    public virtual void OnClose() { }

    /// <summary>
    /// ͨ��ע�ᰴť�¼���֧��ͬ�����첽Action��
    /// </summary>
    /// <param name="buttonName">��ť������</param>
    /// <param name="onClick">����ص���֧��async��</param>
    public void RegisterButton(string buttonName, Func<Task> onClick)
    {
        var btnTrans = transform.Find(buttonName);
        if (btnTrans == null)
        {
            Debug.LogError($"δ�ҵ���ť: {buttonName}");
            return;
        }
        var btn = btnTrans.GetComponent<UIButton>();
        if (btn == null)
        {
            Debug.LogError($"���� {buttonName} δ����UIButton���");
            return;
        }
        btn.onClick.AddListener(async () => await onClick());
    }

    /// <summary>
    /// ͨ��ע�ᰴť�¼���ͬ��Action��
    /// </summary>
    /// <param name="buttonName">��ť������</param>
    /// <param name="onClick">����ص�</param>
    public void RegisterButton(string buttonName, Action onClick)
    {
        var btnTrans = transform.Find(buttonName);
        if (btnTrans == null)
        {
            Debug.LogError($"δ�ҵ���ť: {buttonName}");
            return;
        }
        var btn = btnTrans.GetComponent<UIButton>();
        if (btn == null)
        {
            Debug.LogError($"���� {buttonName} δ����UIButton���");
            return;
        }
        btn.onClick.AddListener(() => onClick());
    }
}