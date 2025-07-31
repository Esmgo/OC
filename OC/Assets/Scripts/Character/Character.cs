using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Character : MonoBehaviour
{
    public Weapon weapon;

    public float maxHealth;
    public float currentHealth;
    public float healthRegenRate;
    public float moveSpeed;
    public float dashSpeed;
    public float dashCooldown;
    public float damagePercent;
    public float physicalDamage;
    public float elementalDamage;
    public float energyDamage;
    public float sanity;
    public int shield;
    public string roleName;
    public string description;  

    public void Init(RoleConfiguration config, WeaponConfiguration weaponConfig)
    {
        maxHealth = currentHealth = config.maxHealth;
        healthRegenRate = config.healthRegenRate;
        moveSpeed = config.moveSpeed;
        dashSpeed = config.dashSpeed;
        dashCooldown = config.dashCooldown;
        physicalDamage = config.physicalDamage;
        elementalDamage = config.elementalDamage;
        energyDamage = config.energyDamage;
        sanity = config.sanity;
        shield = config.shield;
        roleName = config.roleName;
        description = config.description;

        // 初始化移动组件
        var moveComponent = GetComponent<MoveBase>();
        if (moveComponent != null)
        {
            moveComponent.Init(config);
        }

        if (weapon != null)
        {
            weapon.Init(this, weaponConfig);
        }
        else
        {
            Debug.LogError("Weapon component is not assigned to the character. Please assign a weapon in the inspector or through code.");
        }
    }
}
