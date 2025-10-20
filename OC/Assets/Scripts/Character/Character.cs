using BuffSystem;
using GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TextCore.Text;


public class Character : MonoBehaviour
{
    public Canvas canvas;

    private Weapon weapon;
    private SpriteRenderer[] spriteRenderers;
    private MoveBase moveComponent;
    [Tooltip("全局角色属性修饰器")]
    private PlayerAttributeModifier gam;
    [Tooltip("角色的buff管理器")]
    private BuffManager buffManager;

    private RoleConfiguration roleConfig;
    private WeaponConfiguration weaponConfig;
    private Weapon currentWeapon;

    [Tooltip("角色自身属性修饰器")]
    public PlayerAttributeModifier modifier;
    

    public int maxHealth; // 角色最大生命值
    public int currentHealth;   // 角色当前生命值
    public float healthRegenRate;   // 角色生命值每秒恢复量
    public int maxEnergy;    // 角色能量值
    public int currentEnergy; // 角色当前能量值
    public float energyRegenRate; // 角色能量值每秒恢复量
    public float moveSpeed;    // 角色移动速度
    public float dashSpeed;     // 角色冲刺速度
    public float dashCooldown;     // 角色冲刺冷却时间
    public float sanity;        // 角色理智值
    public int shield;      // 角色护盾值
    public string roleName;     // 角色名称
    public string description;    // 角色描述

    public float physicalDamage; // 物理伤害
    public float energyDamage; // 异能伤害
    public float atttackInterval; // 角色攻击间隔


    private float invincibleTime = 0.5f; // 无敌时间
    private float invincibleTimer = 0f; // 无敌计时器
    private bool isInvincible = false; // 是否处于无敌状态
    private float energyRegenValue = 0f; // 能量恢复值累积

    private float damagePercent = 1f; // 伤害加成百分比

    private void Awake()
    {
        // 在Awake中获取已存在的Weapon组件
        weapon = GetComponentInChildren<Weapon>();
        canvas = GetComponentInChildren<Canvas>(true);
    }

    private void OnDestroy()
    {
        EventCenter.Unsubscribe<EnergyChangeEvent, int>(Energy);
        EventCenter.Unsubscribe<WaveStartEvent>(OnWaveStart);
        EventCenter.Unsubscribe<SetFlashEvent, float, Color>(SetFlash);
        EventCenter.Unsubscribe<PlayerAttributeModifierChangedEvent>(UpdateData);
        EventCenter.Unsubscribe<HPChangeEvent, int>(Health);
    }

    private void Update()
    {
        if (!buffManager.HasBuff(BuffType.EnergyWeak))
        {
            energyRegenValue += energyRegenRate * Time.deltaTime;
            if (energyRegenValue >= 1f)
            {
                int energyToAdd = Mathf.FloorToInt(energyRegenValue);
                Energy(energyToAdd);
                energyRegenValue -= energyToAdd;
            }
        }

        if (isInvincible)
        {
            invincibleTimer += Time.deltaTime;
            if (invincibleTimer >= invincibleTime)
            {
                isInvincible = false;
                invincibleTimer = 0f;
            }
        }
    }

    /// <summary>
    /// 角色初始化
    /// </summary>
    /// <param name="config"></param>
    /// <param name="weaponConfig"></param>
    public void Init(RoleConfiguration config, WeaponConfiguration weaponConfig)
    {
        
        this.roleConfig = config;
        this.weaponConfig = weaponConfig;

        maxHealth = config.maxHealth;
        currentHealth = config.maxHealth;
        healthRegenRate = config.healthRegenRate;
        maxEnergy = config.maxEnergy;
        currentEnergy = config.maxEnergy;
        energyRegenRate = config.energyRegenRate;
        moveSpeed = config.moveSpeed;
        dashSpeed = config.dashSpeed;
        dashCooldown = config.dashCooldown;
        sanity = config.sanity;
        shield = config.shield; 
        roleName = config.roleName;
        description = config.description;

        // 初始化移动组件
        moveComponent = GetComponent<MoveBase>();
        if (moveComponent != null)
        {
            moveComponent.Init(config);
        }

        // 初始化武器
        weapon.Init(this, weaponConfig);
        currentWeapon = weapon;

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        gam = GlobalModificationManager.Instance.globalPlayerAttributeModifier;
        buffManager = GetComponent<BuffManager>();

        EventCenter.Subscribe<EnergyChangeEvent, int>(Energy);
        EventCenter.Subscribe<WaveStartEvent>(OnWaveStart);
        EventCenter.Subscribe<SetFlashEvent, float, Color>(SetFlash);
        EventCenter.Subscribe<PlayerAttributeModifierChangedEvent>(UpdateData);
        EventCenter.Subscribe<HPChangeEvent, int>(Health);

    }

    private void UpdateData() 
    {
        maxHealth = (int)((roleConfig.maxHealth + modifier.maxHealthModifier + gam.maxHealthModifier)
                    * (1 + modifier.maxHealthModifierPercent + gam.maxHealthModifierPercent));
        //currentHealth = maxHealth;
        //maxEnergy = (int)((roleConfig.maxEnergy + modifier.maxEnergyModifier + gam.maxEnergyModifier)
        //            * (1 + modifier.maxEnergyModifierPercent + gam.maxEnergyModifierPercent));
        //currentEnergy = Math.Clamp(currentEnergy, 0, maxEnergy);
        
        moveSpeed = (roleConfig.moveSpeed + modifier.moveSpeedModifier + gam.moveSpeedModifier)
                    * (1 + modifier.moveSpeedModifierPercent + gam.moveSpeedModifierPercent);

        atttackInterval = (weaponConfig.attackInterval + modifier.attackIntervalModifier + gam.attackIntervalModifier)
                    * (1 + modifier.attackIntervalModifierPercent + gam.attackIntervalModifierPercent);

        damagePercent = 1 + modifier.damagePercentModifier + gam.damagePercentModifier;

        physicalDamage = (weaponConfig.physicalDamage + modifier.physicalDamageModifier + gam.physicalDamageModifier)
                    * (1 + modifier.physicalDamageModifierPercent + gam.physicalDamageModifierPercent);

        energyDamage = (weaponConfig.energyDamage + modifier.energyDamageModifier + gam.energyDamageModifier)
                    * (1 + modifier.energyDamageModifierPercent + gam.energyDamageModifierPercent);

        if (moveComponent != null)
        {
            moveComponent.UpdateData(moveSpeed, dashSpeed, dashCooldown);
        }
        if (currentWeapon != null)
        {
            currentWeapon.UpdateData(atttackInterval, damagePercent, physicalDamage, energyDamage);
        }

        EventCenter.Publish<UpdateInfoDisplayEvent, Character>(this);
    }

    private void OnWaveStart()
    {
        UpdateData();
    }

    private void Energy(int value)
    {
        int _e = currentEnergy + value;
        if (_e < 0)
        {
            currentEnergy = 0;
            if (!buffManager.HasBuff(BuffType.EnergyWeak))
            {
                buffManager.AddBuff<EnergyWeak>();
                var t = ObjectPoolManager.Instance.GetObject<TextPopUp>("TextPopUp", transform.position, Quaternion.identity);
                t.transform.SetParent(canvas.transform);
                t.Show("能量耗尽！", 2, Color.white, 0.5f, true);
            }
            else
            {
                buffManager.AddBuff<EnergyWeak>();
            }
        }
        else
        {
            currentEnergy = Mathf.Min(_e, maxEnergy);
        }

        EventCenter.Publish<UpdateInfoDisplayEvent, Character>(this);
    }

    public void Health(int value)
    {
        if (value < 0 && isInvincible)
        {
            return;
        }else if (value < 0)
        {
            isInvincible = true;
        }

        int _h = currentHealth + value;

        if (_h <= 0)
        {            
            currentHealth = 0;
            // 角色死亡
            Debug.Log($"{roleName} 死亡！");
            //EventCenter.Publish<PlayerDeadEvent, Character>(this);
        }
        else
        {
            currentHealth = Mathf.Min(_h, maxHealth);
        }
        EventCenter.Publish<UpdateInfoDisplayEvent, Character>(this);
    }

    private void SetFlash(float speed, Color color)
    {
        foreach (var sr in spriteRenderers)
        {
            Material mat = sr.material;
            mat.SetFloat("_Speed", speed);
            mat.SetColor("_Color", color);
        }
    }
}
