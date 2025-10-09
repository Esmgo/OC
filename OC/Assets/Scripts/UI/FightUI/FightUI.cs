using DG.Tweening;
using GameEvents;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FightUI : UIPanel
{
    private TextMeshProUGUI timeText;
    private TextMeshProUGUI coinsText;

    private GameObject waveCompletedGroup;
    private Image bar;
    private TextMeshProUGUI textUp;
    private TextMeshProUGUI textDown;

    private Vector3 originalTextUpPos;
    private Vector3 originalTextDownPos;

    public override void OnOpen()
    {
        EventCenter.Subscribe<WaveCompletedEvent>(WaveCompleted);

        timeText = transform.Find("TimeText").GetComponent<TextMeshProUGUI>();
        coinsText = transform.Find("Coins/Text").GetComponent<TextMeshProUGUI>();

        waveCompletedGroup = transform.Find("WaveCompleted").gameObject;
        bar = transform.Find("WaveCompleted/Bar").GetComponent<Image>();
        textUp = transform.Find("WaveCompleted/Text_up").GetComponent<TextMeshProUGUI>();
        textDown = transform.Find("WaveCompleted/Text_down").GetComponent<TextMeshProUGUI>();
        waveCompletedGroup.SetActive(false);
        originalTextUpPos = textUp.transform.localPosition;
        originalTextDownPos = textDown.transform.localPosition;

        EventCenter.Publish<UpdateInfoDisplayEvent, Character>(Tools.GetCharacter());
    }

    public override void OnClose()
    {
        EventCenter.Unsubscribe<WaveCompletedEvent>(WaveCompleted);
    }

    private void Update()
    {
        if (timeText != null) 
        {
            timeText.text = $"Time: {EnemyManager.Instance.waveTimer:F2} seconds";
        }
        if (coinsText != null) 
        {
            coinsText.text = $"{ItemsManager.Instance.GetCoins()}";
        }
    }

    public async void WaveCompleted()
    {
        waveCompletedGroup.SetActive(true);

        // 重置动画初始状态
        bar.transform.localScale = new Vector3(0, 1, 1);
        textUp.transform.localPosition = originalTextUpPos + Vector3.down * 50;
        textDown.transform.localPosition = originalTextDownPos + Vector3.up * 50;
        textUp.alpha = 0;
        textDown.alpha = 0;
        bar.color = new Color(bar.color.r, bar.color.g, bar.color.b, 1);


        // 创建入场动画序列
        Sequence sequence = DOTween.Sequence();
        sequence.Append(bar.transform.DOScaleX(5, 0.5f).SetEase(Ease.OutQuad));
        sequence.Join(textUp.transform.DOLocalMove(originalTextUpPos, 0.5f).SetEase(Ease.OutQuad));
        sequence.Join(textDown.transform.DOLocalMove(originalTextDownPos, 0.5f).SetEase(Ease.OutQuad));
        sequence.Join(textUp.DOFade(1, 0.5f));
        sequence.Join(textDown.DOFade(1, 0.5f));

        // 等待动画完成
        await sequence.Play().AsyncWaitForCompletion();

        // 延迟3秒
        await Task.Delay(2000);

        // 创建渐隐动画
        Sequence fadeOutSequence = DOTween.Sequence();
        fadeOutSequence.Append(bar.DOFade(0, 0.5f));
        fadeOutSequence.Join(textUp.DOFade(0, 0.5f));
        fadeOutSequence.Join(textDown.DOFade(0, 0.5f));

        // 等待渐隐完成
        await fadeOutSequence.Play().AsyncWaitForCompletion();

        waveCompletedGroup.SetActive(false);

        // 跳转到商店
        await UIManager.Instance.OpenPanelAsync<ShopUI>("ShopUI");

    }
}
