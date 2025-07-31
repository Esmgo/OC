using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InformationBar : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI valueText;
    public Image bar;

    public void Set(string name, float value, float fillAmount, Color themeColor, bool isPercent)
    {
        nameText.text = name;
        if(isPercent)
        {
            valueText.text = $"{value}%";
        }
        else
        {
            valueText.text = value.ToString();
        }
        bar.DOFillAmount(fillAmount, 0.2f);
        bar.color = themeColor;
    }
}
