using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TextPopUp : MonoBehaviour, IPoolable
{
    [Header("UI组件")]
    public TextMeshProUGUI damageText;

    [Header("动画设置")]
    public float moveDistance = 100f;
    public float moveDuration = 1.2f;
    public float fadeDelay = 0.3f;
    public float fadeDuration = 0.9f;
    public float horizontalOffset = 5f; // 水平偏移量，用于随机化位置

    [Header("缩放效果")]
    public bool useScaleEffect = true;
    public float scaleStartSize = 1.2f;
    public float scaleEndSize = 1f;
    public float scaleDuration = 0.3f;
    public Ease scaleEase = Ease.OutBack;

    [Header("颜色设置")]
    public Color normalDamageColor = Color.red;
    public Color criticalDamageColor = Color.yellow;
    public Color healingColor = Color.green;
    
    [Header("暴击特效")]
    public bool useCriticalShake = true;
    public float shakeStrength = 0.4f;
    public float shakeDuration = 0.2f;

    private Camera mainCamera;
    private bool isActive;
    
    // DOTween相关
    private Tween moveTween;    
    private Tween fadeTween;
    private Tween scaleTween;
    private Tween shakeTween;
    private Sequence mainSequence;

    void Awake()
    {
        mainCamera = Camera.main;
        if (damageText == null)
        {
            damageText = GetComponent<TextMeshProUGUI>();
        }
    }

    /// <summary>
    /// 显示文本
    /// </summary>
    public void Show(string text, float distance, Color color, float fontSize, bool isCritical = false)
    {
        damageText.text = text;
        
        if (isCritical)
        {
            fontSize *= 1.3f; // 暴击文字更大
        }
        
        damageText.color = color;
        damageText.fontSize = fontSize;
        
        // 重置状态
        isActive = true;
        gameObject.SetActive(true);
        
        // 开始DOTween动画
        PlayDamageAnimation(distance, isCritical);
    }

    private void PlayDamageAnimation(float distance, bool isCritical)
    {        
        // 停止之前的动画
        KillAllTweens();
        
        mainSequence = DOTween.Sequence();

        // 添加随机性到移动距离和方向
        float randomDistance = distance + Random.Range(-distance * 0.2f, distance * 0.3f); // 距离随机变化 ±20%-30%
        float randomX = Random.Range(-horizontalOffset, horizontalOffset); // 随机水平偏移
        
        // 随机旋转角度（轻微倾斜）
        float randomRotation = Random.Range(-15f, 15f);
        
        // 随机动画持续时间
        float randomMoveDuration = moveDuration + Random.Range(-0.2f, 0.3f);
        float randomFadeDuration = fadeDuration + Random.Range(-0.1f, 0.2f);

        // 淡出动画
        fadeTween = damageText.DOFade(0f, randomFadeDuration)
            .SetDelay(fadeDelay)
            .SetEase(Ease.InQuart);
        mainSequence.Join(fadeTween);

        // 带随机性的移动动画
        Vector3 targetPosition = new Vector3(randomX, randomDistance, 0);
        moveTween = transform.DOLocalMove(targetPosition, randomMoveDuration)
            .SetEase(Ease.OutQuart);
        mainSequence.Join(moveTween);
        
        // 添加随机旋转动画
        Tween rotateTween = transform.DOLocalRotate(new Vector3(0, 0, randomRotation), randomMoveDuration * 0.5f)
            .SetEase(Ease.OutQuart);
        mainSequence.Join(rotateTween);

        // 如果是暴击，添加更多特效
        if (isCritical)
        {
            // 暴击时添加弹性缩放
            scaleTween = transform.DOScale(scaleStartSize, scaleDuration * 0.3f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => {
                    transform.DOScale(scaleEndSize, scaleDuration * 0.7f)
                        .SetEase(Ease.InBack);
                });
            mainSequence.Join(scaleTween);
            
            // 暴击时添加震动效果
            if (useCriticalShake)
            {
                shakeTween = transform.DOShakePosition(shakeDuration, shakeStrength, 10, 90, false, true)
                    .SetDelay(0.1f);
                mainSequence.Join(shakeTween);
            }
        }
        else
        {
            // 普通伤害的轻微缩放效果
            scaleTween = transform.DOScale(scaleStartSize * 0.8f, scaleDuration * 0.4f)
                .SetEase(Ease.OutQuart)
                .OnComplete(() => {
                    transform.DOScale(scaleEndSize, scaleDuration * 0.6f)
                        .SetEase(Ease.InQuart);
                });
            mainSequence.Join(scaleTween);
        }

        mainSequence.OnComplete(ReturnToPool);
        mainSequence.Play();
    }

    private void KillAllTweens()
    {
        // 安全地停止所有动画
        mainSequence?.Kill();
        moveTween?.Kill();
        fadeTween?.Kill();
        scaleTween?.Kill();
        shakeTween?.Kill();
    }

    private void ReturnToPool()
    {
        if (!isActive) return; // 防止重复调用
        
        isActive = false;
        
        // 停止所有动画
        KillAllTweens();
        
        // 优先使用通用对象池
        if (ObjectPoolManager.Instance != null && ObjectPoolManager.Instance.HasPool("TextPopUp"))
        {
            ObjectPoolManager.Instance.ReturnObject(gameObject);
        }
        else
        {
            Debug.LogWarning("No object pool found for DamageText, destroying object instead.");
            Destroy(gameObject);
        }
    }

    #region IPoolable Implementation
    
    public void OnGetFromPool()
    {
        isActive = true;
        KillAllTweens(); // 确保没有残留动画
    }

    public void OnReturnToPool()
    {
        isActive = false;
        KillAllTweens(); // 清理动画
        
        // 重置文本属性
        if (damageText != null)
        {
            damageText.color = normalDamageColor;
            damageText.fontSize = 24f; // 重置为默认大小
        }
        
        // 重置变换状态
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
    
    #endregion

    void OnDestroy()
    {
        // 对象销毁时确保清理所有动画
        KillAllTweens();
    }

    #region 公共方法
    
    /// <summary>
    /// 立即停止动画并返回池中
    /// </summary>
    public void ForceReturn()
    {
        KillAllTweens();
        ReturnToPool();
    }
    
    /// <summary>
    /// 暂停动画
    /// </summary>
    public void PauseAnimation()
    {
        mainSequence?.Pause();
    }
    
    /// <summary>
    /// 恢复动画
    /// </summary>
    public void ResumeAnimation()
    {
        mainSequence?.Play();
    }
    
    #endregion
}
