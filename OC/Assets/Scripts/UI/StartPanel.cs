using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : UIPanel
{
    private Image textMask;
    private CanvasGroup panelCanvasGroup;
    private TextMeshProUGUI text;
    private TextMeshProUGUI click;
    private Image black;

    private bool isClicked = false;
    private bool isInited = false;

    public override void OnOpen()
    {
        if(!isInited) Init();
    }

    private void Init()
    {
        isInited = true;

        textMask = transform.Find("TextMask").GetComponent<Image>();
        panelCanvasGroup = GetComponent<CanvasGroup>();
        text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        click = transform.Find("ClickToStart").GetComponent<TextMeshProUGUI>();
        black = transform.Find("Black").GetComponent<Image>();

        textMask.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(Input.anyKeyDown && !isClicked)
        {
            isClicked = true;
            StartCoroutine(_ClickToStart());
        }
    }

    private IEnumerator _ClickToStart()
    {
        click.text = "©–";
        textMask.gameObject.SetActive(true);
        text.text = "AUTHENTICATING";
        yield return new WaitForSeconds(0.05f);
        textMask.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.8f);
        text.text = "AUTHENTICATING.";
        yield return new WaitForSeconds(0.8f);
        text.text = "AUTHENTICATING..";
        yield return new WaitForSeconds(0.8f);
        text.text = "AUTHENTICATING...";
        yield return new WaitForSeconds(0.2f);

        text.text = "AUTHENTICATING..";
        yield return new WaitForSeconds(0.2f);
        text.text = "AUTHENTICATING...";
        yield return new WaitForSeconds(0.2f);
        text.text = "AUTHENTICATING..";
        yield return new WaitForSeconds(0.2f);
        text.text = "AUTHENTICATING...";
        yield return new WaitForSeconds(0.2f);
        text.text = "AUTHENTICATING..";
        yield return new WaitForSeconds(0.2f);
        text.text = "AUTHENTICATING...";
        yield return new WaitForSeconds(0.2f);

        textMask.gameObject.SetActive(true);
        text.text = "WELCOME!";
        yield return new WaitForSeconds(0.05f);
        textMask.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        //yield return panelCanvasGroup.DOFade(0,0.3f).WaitForCompletion();
        
        yield return black.DOFade(1, 1f).WaitForCompletion();
        yield return UIManager.Instance.OpenPanelAsync<MainPanel>("MainPanel");
        yield return new WaitForSeconds(0.1f);
        UIManager.Instance.ClosePanel("StartPanel");
    }
}
