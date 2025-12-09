using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1WeaponComponent : CharacterWeaponComponent
{
    private Animator animator;

    protected override void OnInit(CharacterConfiguration config)
    {
        animator = GetComponent<Animator>();
    }

    protected override void OnAttack()
    {
        animator.SetTrigger("Attack");
    }
}
