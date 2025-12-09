using DG.Tweening;
using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime_boom : EnemyBase
{
    //[Header("爆炸设置")]
    //[Tooltip("爆炸伤害")]
    //[SerializeField] private int explosionDamage = 10;
    //[Tooltip("爆炸范围")]
    //[SerializeField] private float explosionRadius = 4f;
    //[Tooltip("击退力度")]
    //[SerializeField] private float knockbackForce = 12f;
    
    //// 缓存碰撞结果数组，避免重复分配内存
    //private Collider2D[] targetsCache;
    //[SerializeField] private int maxTargets = 50;
    
    //// 防止多次触发爆炸
    //private bool hasExploded = false;
    
    //protected override void Awake()
    //{
    //    base.Awake();
    //    // 初始化碰撞器缓存数组
    //    targetsCache = new Collider2D[maxTargets];
    //}
    
    //protected override void OnEnable()
    //{
    //    base.OnEnable();
    //    hasExploded = false;
    //}
    
    //protected override void Die()
    //{
    //    // 避免重复调用爆炸效果
    //    if (!hasExploded)
    //    {
    //        hasExploded = true;
            
    //        // 在死亡位置生成爆炸效果
    //        ObjectPoolManager.Instance.GetObject<ParticleRecycler>("Effect_Boom_1", transform.position);
    //        TriggerExplosion();
    //    }
        
    //    base.Die();
    //}

    ///// <summary>
    ///// 触发爆炸效果
    ///// </summary>
    //private void TriggerExplosion()
    //{
    //    // 清空缓存数组，防止之前的引用残留
    //    System.Array.Clear(targetsCache, 0, targetsCache.Length);
        
    //    // 使用NonAlloc版本避免GC分配
    //    int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, explosionRadius, targetsCache);

    //    for (int i = 0; i < hitCount; i++)
    //    {
    //        var target = targetsCache[i];
    //        if (target == null) continue;
            
    //        // 检查是否是玩家
    //        var player = target.GetComponent<Character>();
    //        if (player != null)
    //        {
    //            // 对玩家造成伤害
    //            EventCenter.Publish<HPChangeEvent, int>(-explosionDamage);

    //            // 显示伤害文本
    //            var damageText = ObjectPoolManager.Instance.GetObject<TextPopUp>("TextPopUp", target.transform.position);
    //            if (damageText != null)
    //            {
    //                damageText.transform.SetParent(player.canvas.transform);
    //                damageText.Show($"-{explosionDamage}", 2f, Color.red, 0.6f, true);
    //            }
    //        }

    //        // 检查是否是其他敌人
    //        var enemy = target.GetComponent<EnemyBase>();
    //        if (enemy != null && enemy != this && !enemy.IsDead())
    //        {
    //            // 对其他敌人造成伤害
    //            enemy.TakeDamage(explosionDamage / 2, 0); // 对敌人造成一半伤害

    //            // 计算击退方向并应用击退
    //            Vector2 knockbackDirection = Tools.GetDir(transform, target.transform);
    //            enemy.ApplyKnockback(knockbackDirection, knockbackForce);
    //        }
    //    }

    //    Camera.main.transform.DOShakePosition(0.1f, 0.5f);
    //}

    ///// <summary>
    ///// 对玩家应用击退效果
    ///// </summary>
    //private void ApplyKnockbackToPlayer(MoveBase playerMovement, Vector2 knockbackDirection)
    //{
    //    // 这里需要根据MoveBase的实现来添加击退逻辑
    //    // 例如：playerMovement.ApplyKnockback(knockbackDirection * knockbackForce);
    //}
    
    //protected override void OnDisable()
    //{
    //    // 重置爆炸状态
    //    hasExploded = false;
    //    base.OnDisable();
    //}
    
    //protected void OnDestroy()
    //{
    //    // 清理数组引用
    //    if (targetsCache != null)
    //    {
    //        System.Array.Clear(targetsCache, 0, targetsCache.Length);
    //        targetsCache = null;
    //    }
        
    //    // 调用基类的OnDestroy方法
    //    //base.OnDestroy();
    //}

    //// 在编辑器中显示爆炸范围
    //void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, explosionRadius);

    //    // 绘制半透明填充
    //    Color fillColor = Color.red;
    //    fillColor.a = 0.2f;
    //    Gizmos.color = fillColor;
    //    Gizmos.DrawSphere(transform.position, explosionRadius);
    //}
}
