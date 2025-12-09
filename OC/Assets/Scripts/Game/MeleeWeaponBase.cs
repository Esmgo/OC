using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 近战武器的基类。
/// 提供了扇形攻击范围检测、动画触发和调试绘制等通用功能。
/// </summary>
public abstract class MeleeWeaponBase : Weapon
{
    protected WeaponConfiguration config;
    protected Animator animator;

    /// <summary>
    /// 初始化武器，获取配置和动画控制器
    /// </summary>
    public override void Init(Character owner, WeaponConfiguration weaponConfig)
    {
        base.Init(owner, config); // 调用基类 Init
        config = weaponConfig;


        // 获取动画控制器
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("武器上未找到 Animator 组件。", this);
        }
    }

    /// <summary>
    /// 基类的攻击方法，触发动画并执行索敌。
    /// </summary>
    protected override void Attack()
    {
        if (config == null) return;

        // 1. 触发攻击动画
        animator?.SetTrigger("Attack");

        // 2. 索敌并处理命中
        DetectAndProcessHits();
    }

    /// <summary>
    /// 检测并处理命中逻辑。
    /// </summary>
    protected virtual void DetectAndProcessHits()
    {
        // 在攻击范围内检测所有碰撞体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, config.attackRange, config.targetLayer);

        var targets = new List<Entity>();

        // 筛选出在攻击角度内的目标
        foreach (var collider in colliders)
        {
            Vector2 directionToTarget = Tools.GetDir(transform, collider.transform);

            float angleToTarget = Tools.GetAngle(transform.right, directionToTarget);
            //float angleToTarget = Vector2.Angle(weaponForward, directionToTarget);

            if (angleToTarget <= config.attackAngle / 2)
            {
                if (collider.TryGetComponent<Entity>(out var entity))
                {
                    targets.Add(entity);
                }
            }
        }
        Debug.Log($"检测到 {targets.Count} 个命中目标。");
        // 对所有有效目标执行命中效果
        foreach (var target in targets)
        {
            ProcessHit(target);
        }
    }

    /// <summary>
    /// 处理对单个目标的命中效果。
    /// 子类可以重写此方法以实现不同的伤害逻辑或特效。
    /// </summary>
    /// <param name="target">被击中的目标实体</param>
    protected virtual void ProcessHit(Entity target)
    {
        // 示例：应用基础物理伤害
        //target.TakeDamage(owner.currentPhysicalDamage);

        // 示例：应用击退
        //Vector2 knockbackDirection = (target.transform.position - owner.transform.position).normalized;
        // target.ApplyKnockback(knockbackDirection, meleeConfig.knockBackForce);
    }

    /// <summary>
    /// 在编辑器中绘制辅助线，方便调试攻击范围和角度。
    /// </summary>
    protected virtual void OnDrawGizmosSelected()
    {
        if (config == null) return;

        Gizmos.color = Color.yellow;
        // 绘制攻击范围圆
        Gizmos.DrawWireSphere(transform.position, config.attackRange);

        // 绘制攻击角度扇形
        Vector3 forward = transform.right; // 武器的“前方”
        Quaternion leftRayRotation = Quaternion.AngleAxis(-config.attackAngle / 2, Vector3.forward);
        Quaternion rightRayRotation = Quaternion.AngleAxis(config.attackAngle / 2, Vector3.forward);

        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, leftRayDirection * config.attackRange);
        Gizmos.DrawRay(transform.position, rightRayDirection * config.attackRange);
    }
}