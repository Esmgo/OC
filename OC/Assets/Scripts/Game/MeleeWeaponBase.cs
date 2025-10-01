using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 近战武器基类
/// </summary>
public class MeleeWeaponBase : Weapon
{
    [Header("MeleeWeaponBase设置")]
    [SerializeField] protected Animator animator; // 动画控制器

    protected float weakTimer = 0f; // 虚弱状态计时器

    protected virtual void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Weapon_0_Dao: Animator component not found in children!");
        }
    }

    private void OnEnable()
    {
        // 订阅攻击事件
        EventCenter.Subscribe<AttackEvent>(BaseAttack);
    }

    private void OnDestroy()
    {
        // 取消订阅攻击事件
        EventCenter.Unsubscribe<AttackEvent>(BaseAttack);
    }

    protected virtual void Update()
    {
        if(Input.GetMouseButton(0) && (Time.time >= lastAttackTime + attackInterval))
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    /// <summary>
    /// 基础近战攻击逻辑
    /// </summary>
    protected virtual void BaseAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange, targetLayer);
        List<Collider2D> targets = new();

        foreach (var collider in colliders)
        {
            float angle = Tools.GetAngle(transform, collider.transform);
            float mouseAngle = Tools.GetMouseAngle(transform) - meleeFixAngle;
            float targetAngle = angle;
            // 计算角度差，处理360度边界问题
            float angleDiff = Mathf.DeltaAngle(mouseAngle, targetAngle);
            if (Mathf.Abs(angleDiff) <= meleeAttackAngle / 2)
            {
                targets.Add(collider);
            }
        }

        foreach (var target in targets)
        {
            // 对每个目标应用伤害
            var enemy = target.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(GetPhysicalDamage());

                // 计算击退方向并应用击退
                Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                enemy.ApplyKnockback(knockbackDirection, knockBackForce);
            }
        }

        EventCenter.Publish<EnergyChangeEvent, int>(-energyCostPerAttack);
    }

    private void OnDrawGizmosSelected()
    {
        // 绘制攻击范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 计算鼠标角度并应用修正
        float mouseAngle = Tools.GetMouseAngle(transform) - meleeFixAngle;

        // 绘制攻击角度范围 - 与攻击条件保持一致
        float leftAngleValue = mouseAngle - meleeAttackAngle / 2;
        float rightAngleValue = mouseAngle + meleeAttackAngle / 2;

        // 确保角度在0-360范围内
        leftAngleValue = (leftAngleValue + 360) % 360;
        rightAngleValue = (rightAngleValue + 360) % 360;

        // 将角度转换为方向向量
        Vector3 leftAngle = new Vector3(Mathf.Cos(leftAngleValue * Mathf.Deg2Rad), Mathf.Sin(leftAngleValue * Mathf.Deg2Rad), 0) * attackRange;
        Vector3 rightAngle = new Vector3(Mathf.Cos(rightAngleValue * Mathf.Deg2Rad), Mathf.Sin(rightAngleValue * Mathf.Deg2Rad), 0) * attackRange;

        Gizmos.DrawLine(transform.position, transform.position + leftAngle);
        Gizmos.DrawLine(transform.position, transform.position + rightAngle);
    }
}
