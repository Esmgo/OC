using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Character : MonoBehaviour
{
    private List<Weapon> weapons = new List<Weapon>();

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
        weapons = GetAllChildGameObjectsWithComponent<Weapon>(true);

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

        transform.Find(weaponConfig.name)?.gameObject.SetActive(true);

        // 初始化移动组件
        var moveComponent = GetComponent<MoveBase>();
        if (moveComponent != null)
        {
            moveComponent.Init(config);
        }
        foreach (var weapon in weapons)
        {
            if (weapon != null && weapon.gameObject.name == weaponConfig.weaponName)
            {
                weapon.gameObject.SetActive(true);
                weapon.Init(this, weaponConfig);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
        }
        //if (weapon != null)
        //{
        //    weapon.Init(this, weaponConfig);
        //}
        //else
        //{
        //    Debug.LogError("Weapon component is not assigned to the character. Please assign a weapon in the inspector or through code.");
        //}
    }



    /// <summary>
    /// 获取子物体中所有带指定组件的GameObject
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="includeInactive">是否包含未激活的物体</param>
    /// <returns>所有找到的GameObject</returns>
    public List<T> GetAllChildGameObjectsWithComponent<T>(bool includeInactive = false) where T : Component
    {
        T[] _components = GetComponentsInChildren<T>(includeInactive);
        List<T> components = new List<T>();

        for (int i = 0; i < _components.Length; i++)
        {
            components.Add(_components[i]);
        }

        return components;
    }

}
