using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通用属性及属性计算组件
/// </summary>
public class Property : MonoBehaviour
{
    public PropertyModifier selfModifier; // 自身属性修饰器
    public PropertyModifier globalModifier; // 全局属性修饰器

    [Header("面板属性")]
    [Tooltip("最大生命值")]  
    public int maxHealth;
    [Tooltip("生命回复")]
    public int healthRegenRate; 
    [Tooltip("最大能量")]
    public int maxEnergy;   
    [Tooltip("能量回复")]
    public int energyRegenRate; 
    [Tooltip("移动速度")]
    public int moveSpeed;
    [Tooltip("攻速")]
    public int attackSpeed;
    [Tooltip("伤害倍率")]
    public int damage_p;      
    [Tooltip("物理伤害")]
    public int physicalDamage; 
    [Tooltip("物理伤害倍率")]
    public int physicalDamagePercent; 
    [Tooltip("异能伤害")]
    public int manaDamage;    
    [Tooltip("异能伤害倍率")]
    public int manaDamagePercent;  
    [Tooltip("元素伤害")]
    public int elementalDamage; 
    [Tooltip("元素伤害倍率")]
    public int elementalDamagePercent; 
    [Tooltip("击退")]
    public float knockBack;   
    [Tooltip("精神值")]
    public int sanity;  
    [Tooltip("物理抗性")]
    public int physicalDefense;
    [Tooltip("异能抗性")]
    public int manaDefense;
    [Tooltip("元素抗性")]
    public int elementalDefense;   
    [Tooltip("精准度")]
    public int accuracy;
    [Tooltip("冷却")]
    public float coolDwon;
    [Tooltip("异常抗性")]
    public float ailmentResistance;
    [Tooltip("范围")]
    public int range;

    [Header("实际属性")]
    [Tooltip("最大生命值")]
    public int maxHealth_value;
    [Tooltip("当前生命值")]
    public int currentHealth_value;
    [Tooltip("生命回复")]
    public float healthRegenRate_value;
    [Tooltip("最大能量")]
    public int maxEnergy_value;
    [Tooltip("当前能量")]
    public int currentEnergy_value;
    [Tooltip("能量回复")]
    public float energyRegenRate_value;
    [Tooltip("移动速度")]
    public int moveSpeed_value;
    [Tooltip("攻速")]
    public int attackSpeed_value;
    [Tooltip("伤害倍率")]
    public int damagePercent_value;
    [Tooltip("物理伤害")]
    public int physicalDamage_value;
    [Tooltip("物理伤害倍率")]
    public int physicalDamagePercent_value;
    [Tooltip("异能伤害")]
    public int manaDamage_value;
    [Tooltip("异能伤害倍率")]
    public int manaDamagePercent_value;
    [Tooltip("元素伤害")]
    public int elementalDamage_value;
    [Tooltip("元素伤害倍率")]
    public int elementalDamagePercent_value;
    [Tooltip("击退")]
    public float knockBack_value;
    [Tooltip("精神值")]
    public int sanity_value;
    [Tooltip("物理抗性")]
    public int physicalDefense_value;
    [Tooltip("异能抗性")]
    public int manaDefense_value;
    [Tooltip("元素抗性")]
    public int elementalDefense_value;
    [Tooltip("散射角度")]
    private float spreadAngle_value;
    [Tooltip("冷却")]
    public float coolDwon_value;
    [Tooltip("异常抗性")]
    public float ailmentResistance_value;
    [Tooltip("范围")]
    public int range_value;
    [Tooltip("冲刺冷却")]
    public float dashCooldown_value;
    [Tooltip("冲刺时间")]
    public float dashDuration_value;
    [Tooltip("冲刺速度")]
    public float dashSpeed_value;

    #region 中间运算用的变量
    private float totalhealthRegenRate;
    private float totalenergyRegenRate;
    #endregion

    public void Init(CharacterConfiguration roleConfig, PropertyModifier globalModifier)
    {
        selfModifier = new();

        this.globalModifier = globalModifier;

        maxHealth = roleConfig.maxHealth;
        maxEnergy = roleConfig.maxEnergy;
    }

    //public void UpdateValue()
    //{
    //    maxHealth_value = (int)((maxHealth + selfModifier.maxHealthModifier + globalModifier.maxHealthModifier) * (1 + selfModifier.maxHealthModifierPercent + globalModifier.maxHealthModifierPercent));

    //    totalhealthRegenRate = healthRegenRate + selfModifier.healthRegenRateModifier + globalModifier.healthRegenRateModifier;
    //    healthRegenRate_value = totalhealthRegenRate > 0 ? 2.0f * Mathf.Pow(totalhealthRegenRate, 0.5f) + 0.8f * Mathf.Pow(totalhealthRegenRate, 0.33f) : 0;

    //    maxEnergy_value = (int)((maxEnergy + selfModifier.maxEnergyModifier + globalModifier.maxEnergyModifier) * (1 + selfModifier.maxEnergyModifierPercent + globalModifier.maxEnergyModifierPercent));
        
    //    totalenergyRegenRate = energyRegenRate + selfModifier.energyRegenRateModifier + globalModifier.energyRegenRateModifier;

    //}

    public void Reset()
    {
        //UpdateValue();
        currentHealth_value = maxHealth_value;
        currentEnergy_value = maxEnergy_value;
    }
}
