using UnityEngine;

/// <summary>
/// 游戏事件定义集合
/// </summary>
namespace GameEvents
{
    #region 游戏状态事件

    /// <summary>
    /// 游戏开始事件
    /// </summary>
    public struct GameStartedEvent : IGameEvent { }

    /// <summary>
    /// 游戏暂停事件
    /// </summary>
    public struct GamePausedEvent : IGameEvent { }

    /// <summary>
    /// 游戏恢复事件
    /// </summary>
    public struct GameResumedEvent : IGameEvent { }

    /// <summary>
    /// 游戏结束事件
    /// </summary>
    public struct GameOverEvent : IGameEvent { }

    /// <summary>
    /// 返回主菜单事件
    /// </summary>
    public struct ReturnToMainMenuEvent : IGameEvent { }

    #endregion

    #region 角色事件

    /// <summary>
    /// 角色生成事件
    /// </summary>
    public struct PlayerSpawnedEvent : IGameEvent { }

    /// <summary>
    /// 角色死亡事件
    /// </summary>
    public struct PlayerDiedEvent : IGameEvent { }

    /// <summary>
    /// 角色受伤事件
    /// 参数: 伤害值, 当前生命值, 最大生命值
    /// </summary>
    public struct PlayerTakeDamageEvent : IGameEvent { }

    /// <summary>
    /// 角色治疗事件
    /// 参数: 治疗量, 当前生命值, 最大生命值
    /// </summary>
    public struct PlayerHealedEvent : IGameEvent { }

    /// <summary>
    /// 角色升级事件
    /// 参数: 新等级, 获得经验值
    /// </summary>
    public struct PlayerLevelUpEvent : IGameEvent { }

    /// <summary>
    /// 角色配置改变事件
    /// 参数: 新的角色配置
    /// </summary>
    public struct PlayerConfigChangedEvent : IGameEvent { }

    #endregion

    #region 战斗事件

    /// <summary>
    /// 攻击事件
    /// 参数: 攻击者Transform, 目标Transform, 伤害值
    /// </summary>
    public struct AttackEvent : IGameEvent { }

    /// <summary>
    /// 击杀事件
    /// 参数: 击杀者Transform, 被击杀者Transform
    /// </summary>
    public struct KillEvent : IGameEvent { }

    /// <summary>
    /// 暴击事件
    /// 参数: 攻击者Transform, 目标Transform, 暴击伤害
    /// </summary>
    public struct CriticalHitEvent : IGameEvent { }

    /// <summary>
    /// 子弹发射事件
    /// 参数: 发射位置, 方向, 子弹类型
    /// </summary>
    public struct BulletFiredEvent : IGameEvent { }

    /// <summary>
    /// 子弹击中事件
    /// 参数: 击中位置, 目标Transform, 伤害值
    /// </summary>
    public struct BulletHitEvent : IGameEvent { }

    #endregion

    #region 敌人事件

    /// <summary>
    /// 敌人生成事件
    /// 参数: 敌人Transform, 敌人类型
    /// </summary>
    public struct EnemySpawnedEvent : IGameEvent { }

    /// <summary>
    /// 敌人死亡事件
    /// 参数: 敌人Transform, 击杀者Transform
    /// </summary>
    public struct EnemyDiedEvent : IGameEvent { }

    /// <summary>
    /// 敌人攻击事件
    /// 参数: 敌人Transform, 目标Transform
    /// </summary>
    public struct EnemyAttackedEvent : IGameEvent { }

    #endregion

    #region UI事件

    /// <summary>
    /// UI面板打开事件
    /// 参数: 面板名称
    /// </summary>
    public struct UIPanelOpenedEvent : IGameEvent { }

    /// <summary>
    /// UI面板关闭事件
    /// 参数: 面板名称
    /// </summary>
    public struct UIPanelClosedEvent : IGameEvent { }

    /// <summary>
    /// 按钮点击事件
    /// 参数: 按钮名称, 面板名称
    /// </summary>
    public struct ButtonClickedEvent : IGameEvent { }

    #endregion

    #region 分数和经验事件

    /// <summary>
    /// 分数改变事件
    /// 参数: 新分数, 增加的分数
    /// </summary>
    public struct ScoreChangedEvent : IGameEvent { }

    /// <summary>
    /// 经验值改变事件
    /// 参数: 新经验值, 增加的经验值, 当前等级
    /// </summary>
    public struct ExperienceChangedEvent : IGameEvent { }

    /// <summary>
    /// 击杀数改变事件
    /// 参数: 新击杀数, 增加的击杀数
    /// </summary>
    public struct KillCountChangedEvent : IGameEvent { }

    #endregion

    #region 物品和装备事件

    /// <summary>
    /// 物品获得事件
    /// 参数: 物品ID, 数量
    /// </summary>
    public struct ItemAcquiredEvent : IGameEvent { }

    /// <summary>
    /// 物品使用事件
    /// 参数: 物品ID, 使用数量
    /// </summary>
    public struct ItemUsedEvent : IGameEvent { }

    /// <summary>
    /// 装备更换事件
    /// 参数: 装备位置, 新装备ID, 旧装备ID
    /// </summary>
    public struct EquipmentChangedEvent : IGameEvent { }

    #endregion

    #region 系统事件


    public struct RoleSelectedEvent : IGameEvent { }


    public struct WeaponSelectedEvent : IGameEvent { }


    /// <summary>
    /// 对象池创建事件
    /// 参数: 池名称, 池大小
    /// </summary>
    public struct ObjectPoolCreatedEvent : IGameEvent { }

    /// <summary>
    /// 对象池对象获取事件
    /// 参数: 池名称, 获取的对象
    /// </summary>
    public struct ObjectPoolObjectRetrievedEvent : IGameEvent { }

    /// <summary>
    /// 对象池对象返回事件
    /// 参数: 池名称, 返回的对象
    /// </summary>
    public struct ObjectPoolObjectReturnedEvent : IGameEvent { }

    /// <summary>
    /// 资源加载完成事件
    /// 参数: 资源地址, 资源对象
    /// </summary>
    public struct ResourceLoadedEvent : IGameEvent { }

    /// <summary>
    /// 配置加载完成事件
    /// 参数: 配置类型名称
    /// </summary>
    public struct ConfigurationLoadedEvent : IGameEvent { }

    #endregion

    #region 音频事件

    /// <summary>
    /// 播放音效事件
    /// 参数: 音效名称, 音量
    /// </summary>
    public struct PlaySoundEffectEvent : IGameEvent { }

    /// <summary>
    /// 播放背景音乐事件
    /// 参数: 音乐名称, 是否循环
    /// </summary>
    public struct PlayBackgroundMusicEvent : IGameEvent { }

    /// <summary>
    /// 停止音效事件
    /// 参数: 音效名称
    /// </summary>
    public struct StopSoundEffectEvent : IGameEvent { }

    #endregion
}