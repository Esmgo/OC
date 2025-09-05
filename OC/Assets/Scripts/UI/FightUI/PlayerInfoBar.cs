using GameEvents;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoBar : MonoBehaviour
{
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI healthWarnning;
    [SerializeField] private Image energyBarFill;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI energyWarnning;

    private void Awake()
    {
        EventCenter.Subscribe<UpdateInfoDisplayEvent, Character>(UpdateBar);
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Unsubscribe()
    {

        EventCenter.Unsubscribe<UpdateInfoDisplayEvent, Character>(UpdateBar);
    }

    private void UpdateBar(Character c)
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)c.currentHealth/c.maxHealth;
            healthText.text = $"{c.currentHealth}/{c.maxHealth}";
        }
        if (healthWarnning != null)
        {
            if(c.currentHealth <= c.maxHealth * 0.3f)
            {
                healthWarnning.gameObject.SetActive(true);
            }
            else
            {
                healthWarnning.gameObject.SetActive(false);
            }
        }
        if (energyBarFill != null)
        {
            energyBarFill.fillAmount = (float)c.currentEnergy / c.maxEnergy;
            energyText.text = $"{c.currentEnergy} / {c.maxEnergy}";
        }
        if (energyWarnning != null)
        {
            if (c.currentEnergy <= c.maxEnergy * 0.3f)
            {
                energyWarnning.gameObject.SetActive(true);
            }
            else
            {
                energyWarnning.gameObject.SetActive(false);
            }
        }
    }
}
