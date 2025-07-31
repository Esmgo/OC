using UnityEngine;

/// <summary>
/// ��Ϸ�¼����弯��
/// </summary>
namespace GameEvents
{
    #region ��Ϸ״̬�¼�

    /// <summary>
    /// ��Ϸ��ʼ�¼�
    /// </summary>
    public struct GameStartedEvent : IGameEvent { }

    /// <summary>
    /// ��Ϸ��ͣ�¼�
    /// </summary>
    public struct GamePausedEvent : IGameEvent { }

    /// <summary>
    /// ��Ϸ�ָ��¼�
    /// </summary>
    public struct GameResumedEvent : IGameEvent { }

    /// <summary>
    /// ��Ϸ�����¼�
    /// </summary>
    public struct GameOverEvent : IGameEvent { }

    /// <summary>
    /// �������˵��¼�
    /// </summary>
    public struct ReturnToMainMenuEvent : IGameEvent { }

    #endregion

    #region ��ɫ�¼�

    /// <summary>
    /// ��ɫ�����¼�
    /// </summary>
    public struct PlayerSpawnedEvent : IGameEvent { }

    /// <summary>
    /// ��ɫ�����¼�
    /// </summary>
    public struct PlayerDiedEvent : IGameEvent { }

    /// <summary>
    /// ��ɫ�����¼�
    /// ����: �˺�ֵ, ��ǰ����ֵ, �������ֵ
    /// </summary>
    public struct PlayerTakeDamageEvent : IGameEvent { }

    /// <summary>
    /// ��ɫ�����¼�
    /// ����: ������, ��ǰ����ֵ, �������ֵ
    /// </summary>
    public struct PlayerHealedEvent : IGameEvent { }

    /// <summary>
    /// ��ɫ�����¼�
    /// ����: �µȼ�, ��þ���ֵ
    /// </summary>
    public struct PlayerLevelUpEvent : IGameEvent { }

    /// <summary>
    /// ��ɫ���øı��¼�
    /// ����: �µĽ�ɫ����
    /// </summary>
    public struct PlayerConfigChangedEvent : IGameEvent { }

    #endregion

    #region ս���¼�

    /// <summary>
    /// �����¼�
    /// ����: ������Transform, Ŀ��Transform, �˺�ֵ
    /// </summary>
    public struct AttackEvent : IGameEvent { }

    /// <summary>
    /// ��ɱ�¼�
    /// ����: ��ɱ��Transform, ����ɱ��Transform
    /// </summary>
    public struct KillEvent : IGameEvent { }

    /// <summary>
    /// �����¼�
    /// ����: ������Transform, Ŀ��Transform, �����˺�
    /// </summary>
    public struct CriticalHitEvent : IGameEvent { }

    /// <summary>
    /// �ӵ������¼�
    /// ����: ����λ��, ����, �ӵ�����
    /// </summary>
    public struct BulletFiredEvent : IGameEvent { }

    /// <summary>
    /// �ӵ������¼�
    /// ����: ����λ��, Ŀ��Transform, �˺�ֵ
    /// </summary>
    public struct BulletHitEvent : IGameEvent { }

    #endregion

    #region �����¼�

    /// <summary>
    /// ���������¼�
    /// ����: ����Transform, ��������
    /// </summary>
    public struct EnemySpawnedEvent : IGameEvent { }

    /// <summary>
    /// ���������¼�
    /// ����: ����Transform, ��ɱ��Transform
    /// </summary>
    public struct EnemyDiedEvent : IGameEvent { }

    /// <summary>
    /// ���˹����¼�
    /// ����: ����Transform, Ŀ��Transform
    /// </summary>
    public struct EnemyAttackedEvent : IGameEvent { }

    #endregion

    #region UI�¼�

    /// <summary>
    /// UI�����¼�
    /// ����: �������
    /// </summary>
    public struct UIPanelOpenedEvent : IGameEvent { }

    /// <summary>
    /// UI���ر��¼�
    /// ����: �������
    /// </summary>
    public struct UIPanelClosedEvent : IGameEvent { }

    /// <summary>
    /// ��ť����¼�
    /// ����: ��ť����, �������
    /// </summary>
    public struct ButtonClickedEvent : IGameEvent { }

    #endregion

    #region �����;����¼�

    /// <summary>
    /// �����ı��¼�
    /// ����: �·���, ���ӵķ���
    /// </summary>
    public struct ScoreChangedEvent : IGameEvent { }

    /// <summary>
    /// ����ֵ�ı��¼�
    /// ����: �¾���ֵ, ���ӵľ���ֵ, ��ǰ�ȼ�
    /// </summary>
    public struct ExperienceChangedEvent : IGameEvent { }

    /// <summary>
    /// ��ɱ���ı��¼�
    /// ����: �»�ɱ��, ���ӵĻ�ɱ��
    /// </summary>
    public struct KillCountChangedEvent : IGameEvent { }

    #endregion

    #region ��Ʒ��װ���¼�

    /// <summary>
    /// ��Ʒ����¼�
    /// ����: ��ƷID, ����
    /// </summary>
    public struct ItemAcquiredEvent : IGameEvent { }

    /// <summary>
    /// ��Ʒʹ���¼�
    /// ����: ��ƷID, ʹ������
    /// </summary>
    public struct ItemUsedEvent : IGameEvent { }

    /// <summary>
    /// װ�������¼�
    /// ����: װ��λ��, ��װ��ID, ��װ��ID
    /// </summary>
    public struct EquipmentChangedEvent : IGameEvent { }

    #endregion

    #region ϵͳ�¼�


    public struct RoleSelectedEvent : IGameEvent { }


    public struct WeaponSelectedEvent : IGameEvent { }


    /// <summary>
    /// ����ش����¼�
    /// ����: ������, �ش�С
    /// </summary>
    public struct ObjectPoolCreatedEvent : IGameEvent { }

    /// <summary>
    /// ����ض����ȡ�¼�
    /// ����: ������, ��ȡ�Ķ���
    /// </summary>
    public struct ObjectPoolObjectRetrievedEvent : IGameEvent { }

    /// <summary>
    /// ����ض��󷵻��¼�
    /// ����: ������, ���صĶ���
    /// </summary>
    public struct ObjectPoolObjectReturnedEvent : IGameEvent { }

    /// <summary>
    /// ��Դ��������¼�
    /// ����: ��Դ��ַ, ��Դ����
    /// </summary>
    public struct ResourceLoadedEvent : IGameEvent { }

    /// <summary>
    /// ���ü�������¼�
    /// ����: ������������
    /// </summary>
    public struct ConfigurationLoadedEvent : IGameEvent { }

    #endregion

    #region ��Ƶ�¼�

    /// <summary>
    /// ������Ч�¼�
    /// ����: ��Ч����, ����
    /// </summary>
    public struct PlaySoundEffectEvent : IGameEvent { }

    /// <summary>
    /// ���ű��������¼�
    /// ����: ��������, �Ƿ�ѭ��
    /// </summary>
    public struct PlayBackgroundMusicEvent : IGameEvent { }

    /// <summary>
    /// ֹͣ��Ч�¼�
    /// ����: ��Ч����
    /// </summary>
    public struct StopSoundEffectEvent : IGameEvent { }

    #endregion
}