using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : UIPanel
{
    private Image standImage;
    private float standImageOffset = 400f;

    private GameObject startButton;
    private GameObject rolesButton;
    private GameObject collecttionsButton;
    private float buttonOffset = 400f;

    private bool isInited = false;

    public override void OnOpen()
    {
        if(!isInited)   Init();

        StartCoroutine(_EnterAnimation());
    }

    private void Init()
    {
        isInited = true;
        standImage = transform.Find("StandImage").GetComponent<Image>();
        startButton = transform.Find("Start").gameObject;
        rolesButton = transform.Find("Roles").gameObject;
        collecttionsButton = transform.Find("Collections").gameObject;

        RegisterButton("Start", async () =>
        {
            await UIManager.Instance.OpenPanelAsync<SelectRolePanel>("SelectRolePanel");
            UIManager.Instance.ClosePanel("MainPanel");
        });
    }

    private IEnumerator _EnterAnimation()
    {
        standImage.color = new Color(standImage.color.r, standImage.color.g, standImage.color.b, 0);
        standImage.rectTransform.anchoredPosition -= new Vector2(standImageOffset, 0);

        startButton.GetComponent<RectTransform>().anchoredPosition += new Vector2(buttonOffset, 0);
        rolesButton.GetComponent<RectTransform>().anchoredPosition += new Vector2(buttonOffset, 0);
        collecttionsButton.GetComponent<RectTransform>().anchoredPosition += new Vector2(buttonOffset, 0);

        yield return null;

        standImage.DOFade(1, 0.3f).SetEase(Ease.OutQuad);
        standImage.rectTransform.DOAnchorPosX(standImage.rectTransform.anchoredPosition.x + standImageOffset, 0.3f).SetEase(Ease.OutQuad);

        yield return new WaitForSeconds(0.3f);

        startButton.GetComponent<RectTransform>().DOAnchorPosX(startButton.GetComponent<RectTransform>().anchoredPosition.x - buttonOffset, 0.3f).SetEase(Ease.OutQuad);
        rolesButton.GetComponent<RectTransform>().DOAnchorPosX(rolesButton.GetComponent<RectTransform>().anchoredPosition.x - buttonOffset, 0.5f).SetEase(Ease.OutQuad);
        collecttionsButton.GetComponent<RectTransform>().DOAnchorPosX(collecttionsButton.GetComponent<RectTransform>().anchoredPosition.x - buttonOffset, 0.8f).SetEase(Ease.OutQuad);
    }
}
