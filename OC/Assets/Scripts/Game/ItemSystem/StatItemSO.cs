using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 代表一类“数值型”道具的 ScriptableObject。
/// 这类道具的核心效果是提供永久性的全局属性加成。
/// </summary>
[CreateAssetMenu(fileName = "New Stat Item", menuName = "Game/Items/Stat Item")]
public class StatItemSO : ItemConfiguration
{
    [Header("数值加成效果")]
    [Tooltip("当获取此物品时，会应用的永久性全局属性效果列表。")]
    public List<ModifierPack> globalEffects;

    /// <summary>
    /// 重写 OnGet 方法，实现自动应用所有定义的全局效果。
    /// </summary>
    public override void OnGet(Entity entity)
    {
        if (globalEffects == null || globalEffects.Count == 0)
        {
            Debug.LogWarning($"道具 {this.name} 是一个 StatItemSO，但没有配置任何全局效果。");
            return;
        }

        // 遍历此道具携带的所有效果，并逐一应用到全局管理器
        foreach (var effect in globalEffects)
        {
            GlobalStatModifier.Instance.AddModifierForPlayer(effect);
        }
    }
}
