using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageText : MonoBehaviour, IPoolable
{
    [Header("UI���")]
    public TextMeshProUGUI damageText;
    public RectTransform rectTransform;

    [Header("��������")]
    public float moveSpeed = 100f;
    public float fadeSpeed = 2f;
    public float lifetime = 1.5f;
    public Vector2 randomOffset = new Vector2(20f, 20f);

    [Header("��ɫ����")]
    public Color normalDamageColor = Color.red;
    public Color criticalDamageColor = Color.yellow;

    private CanvasGroup canvasGroup;
    private Camera mainCamera;
    private Vector3 worldPosition;
    private Vector2 targetPosition;
    private float timer;
    private bool isActive;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        mainCamera = Camera.main;
        if (damageText == null)
        {
            damageText = GetComponent<TextMeshProUGUI>();
        }
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;

        // �ƶ��͵���
        rectTransform.anchoredPosition = Vector2.MoveTowards(
            rectTransform.anchoredPosition, 
            targetPosition, 
            moveSpeed * Time.deltaTime
        );

        canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / lifetime);

        // �����������
        if (timer >= lifetime)
        {
            ReturnToPool();
        }
    }

    public void Show(int damage, Vector3 worldPos, bool isCritical = false)
    {
        worldPosition = worldPos;
        
        // �����˺��ı�
        damageText.text = damage.ToString();
        damageText.color = isCritical ? criticalDamageColor : normalDamageColor;
        
        // ������Ļλ��
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPosition);
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform, 
            screenPoint, 
            null, 
            out localPoint
        );

        rectTransform.anchoredPosition = localPoint;
        
        // ����Ŀ��λ�ã������ƶ���������ƫ�ƣ�
        targetPosition = localPoint + Vector2.up * 50f + new Vector2(
            Random.Range(-randomOffset.x, randomOffset.x),
            Random.Range(-randomOffset.y, randomOffset.y)
        );

        // ����״̬
        timer = 0f;
        isActive = true;
        canvasGroup.alpha = 1f;
        gameObject.SetActive(true);
    }

    public void Show(int damage, Vector3 worldPos)
    {
        Show(damage, worldPos, false);
    }

    private void ReturnToPool()
    {
        isActive = false;
        
        // ����ʹ��ͨ�ö����
        if (ObjectPoolManager.Instance != null && ObjectPoolManager.Instance.HasPool("DamageText"))
        {
            ObjectPoolManager.Instance.ReturnObject("DamageText", gameObject);
        }
        // ���˵�ר�ó�
        else if (DamageTextPool.Instance != null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region IPoolable Implementation
    
    public void OnGetFromPool()
    {
        timer = 0f;
        isActive = true;
        canvasGroup.alpha = 1f;
    }

    public void OnReturnToPool()
    {
        isActive = false;
        timer = 0f;
        canvasGroup.alpha = 0f;
    }
    
    #endregion
}
