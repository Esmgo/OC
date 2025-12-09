using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterStatComponent))]
[RequireComponent(typeof(CharacterMoveComponent))]
public class CharacterComponent : EntityComponent
{
    public int maxHealth => statComponent?.maxHealth ?? -1;
    public int currentHealth => statComponent?.currentHealth ?? -1;
    public int maxEnergy => statComponent?.maxEnergy ?? -1;
    public int currentEnergy => statComponent?.currentEnergy ?? -1;

    private CharacterConfiguration config;
    private CharacterStatComponent statComponent;
    private CharacterMoveComponent moveComponent;
    private CharacterWeaponComponent weaponComponent;
    private WeaponRotateComponent weaponRotateComponent;
    public void Init(CharacterConfiguration config)
    {
        this.config = config;
        statComponent = GetComponent<CharacterStatComponent>();
        moveComponent = GetComponent<CharacterMoveComponent>();
        weaponComponent = GetComponentInChildren<CharacterWeaponComponent>();
        weaponRotateComponent = GetComponentInChildren<WeaponRotateComponent>();
        statComponent?.Init(config);
        moveComponent?.Init(config);
        weaponComponent?.Init(config);

        if (weaponComponent != null)
        {
            weaponComponent.OnStartAttack += OnStartAttack;
            weaponComponent.OnStopAttack += OnStopAttack;
        }
    }

    private void OnDestroy()
    {
        if (weaponComponent != null)
        {
            weaponComponent.OnStartAttack -= OnStartAttack;
            weaponComponent.OnStopAttack -= OnStopAttack;
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            HandleAttackInput();
        }
    }

    private void HandleAttackInput()
    {
        if (!CanAttack()) return;
        weaponComponent.Attack();
    }

    private bool CanAttack()
    {
        if (weaponComponent == null || statComponent == null) return false;
        if (statComponent.currentEnergy < weaponComponent.attackCost) return false;
        if (Time.time < weaponComponent.attackInterval + weaponComponent.lastAttackTime) return false;
        return true;
    }

    private void OnStartAttack()
    {
        moveComponent.PauseFlip(true);
        weaponRotateComponent?.PauseRotate(true);
    }

    private void OnStopAttack()
    {
        moveComponent.PauseFlip(false);
        weaponRotateComponent?.PauseRotate(false);
    }
}
