using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(RectTransform))]
public class UIButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("��ť�¼�")]
    public UnityEvent onClick;
    public UnityEvent onDown;
    public UnityEvent onUp;
    public UnityEvent onEnter;
    public UnityEvent onExit;

    [Header("��ѡ����ť�Ӿ�����")]
    public Image targetImage;
    public Color normalColor = Color.white;
    public Color pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    public Color hoverColor = new Color(0.9f, 0.9f, 0.9f, 1f);

    private void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
        if (targetImage != null)
            targetImage.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onDown?.Invoke();
        if (targetImage != null)
            targetImage.color = pressedColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onUp?.Invoke();
        if (targetImage != null)
            targetImage.color = hoverColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke();
        if (targetImage != null)
            targetImage.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onExit?.Invoke();
        if (targetImage != null)
            targetImage.color = normalColor;
    }

    /// <summary>
    /// ����ע�����¼�
    /// </summary>
    public void AddClickListener(Action callback)
    {
        onClick.AddListener(() => callback());
    }
    public void RemoveClickListener(Action callback)
    {
        onClick.RemoveListener(() => callback());
    }
}