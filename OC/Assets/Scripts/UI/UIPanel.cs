using UnityEngine;

public class UIPanel : MonoBehaviour
{
    // ��ʱ����
    public virtual void OnOpen() { }
    // �ر�ʱ����
    public virtual void OnClose() { }

    // �����UIPanel�������ű���
    void Start()
    {
        //var btn = transform.Find("MyButton").GetComponent<UIButton>();
        //btn.AddClickListener(OnMyButtonClick);
    }

    void OnMyButtonClick()
    {
        Debug.Log("��ť�������");
    }
}