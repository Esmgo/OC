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

    private Character character;

    public void Init()
    {
        character = Tools.GetCharacter();
        character.OnStatChanged += UpdateBar;
    }

    //private void Update()
    //{
    //     UpdateBar();
    //}

    private void UpdateBar()
    {
        if (character == null) 
        {
            Debug.LogError("PlayerInfoBar: ½ÇÉ«Îª¿Õ£¡");
            return; 
        }

        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)character.currentHealth / character.currentMaxHealth;
            healthText.text = $"{character.currentHealth}/{character.currentMaxHealth}";
        }
        if (healthWarnning != null)
        {
            if(character.currentHealth <= character.currentMaxHealth * 0.3f)
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
            energyBarFill.fillAmount = (character.currentEnergy / character.currentMaxEnergy);
            energyText.text = $"{character.currentEnergy} / {character.currentMaxEnergy}";
        }
        if (energyWarnning != null)
        {
            if (character.currentEnergy <= character.currentMaxEnergy * 0.3f)
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
