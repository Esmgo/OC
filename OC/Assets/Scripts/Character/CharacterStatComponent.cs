using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatComponent : StatComponent
{
    public int maxHealth { get; private set; }
    public int currentHealth { get; private set; }
    public float healthRegenRate { get; private set; }
    public int maxEnergy { get; private set; }
    public int currentEnergy { get; private set; }
    public float energyRegenRate { get; private set; }

    private CharacterStatController statController;

    private float deltaHealth;
    private float deltaEnergy;

    public void Init(CharacterConfiguration config)
    {
        currentHealth = maxHealth = config.maxHealth;
        healthRegenRate = config.healthRegen;
        currentEnergy = maxEnergy = config.maxEnergy;
        energyRegenRate = config.energyRegen;

        statController = new(true, true);
    }

    public void UpdateInfo(CharacterStatController statController)
    {
        this.statController = statController;
    }

    public void UpdateInfo(CharacterStats stats)
    {
        maxHealth = stats.maxHealth;
        healthRegenRate = stats.healthRegen;
        maxEnergy = stats.maxEnergy;
        energyRegenRate = stats.energyRegen;
    }

    private void Update()
    {
        if(statController.healthRegenController && healthRegenRate > 0) HealthRegen();
        if(statController.energyRegenController && energyRegenRate > 0) EnergyRegen();
    }

    public override void Health()
    {

    }

    private void HealthRegen()
    {
        deltaHealth += healthRegenRate * Time.deltaTime;
        if (deltaHealth >= 1f)
        {
            int healthToAdd = Mathf.FloorToInt(deltaHealth);
            currentHealth = Mathf.Min(currentHealth + healthToAdd, maxHealth);
            deltaHealth -= healthToAdd;
        }
    }

    private void EnergyRegen()
    {
        deltaEnergy += energyRegenRate * Time.deltaTime;
        if (deltaEnergy >= 1f)
        {
            int energyToAdd = Mathf.FloorToInt(deltaEnergy);
            currentEnergy = Mathf.Min(currentEnergy + energyToAdd, maxEnergy);
            deltaEnergy -= energyToAdd;
        }
    }
}

public struct CharacterStatController 
{
    public bool healthRegenController;
    public bool energyRegenController;

    public CharacterStatController(bool healthRegenController, bool energyRegenController) 
    {
        this.healthRegenController = healthRegenController;
        this.energyRegenController = energyRegenController;
    }
}

public struct CharacterStats
{   
    public int maxHealth;
    public float healthRegen;
    public int maxEnergy;
    public float energyRegen;

    public CharacterStats(int maxHealth, float healthRegen, int maxEnergy, float energyRegen) 
    {
        this.maxHealth = maxHealth;
        this.healthRegen = healthRegen;
        this.maxEnergy = maxEnergy;
        this.energyRegen = energyRegen;
    }
}
