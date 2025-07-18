using UnityEngine;

public class UIPanel : MonoBehaviour
{
    // 打开时调用
    public virtual void OnOpen() { }
    // 关闭时调用
    public virtual void OnClose() { }

    // 在你的UIPanel或其他脚本中
    void Start()
    {
        //var btn = transform.Find("MyButton").GetComponent<UIButton>();
        //btn.AddClickListener(OnMyButtonClick);
    }

    void OnMyButtonClick()
    {
        Debug.Log("按钮被点击！");
    }
}