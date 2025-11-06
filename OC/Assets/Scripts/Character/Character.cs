using BuffSystem;
using GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Character : Entity 
{
    [Tooltip("角色的UI画布")]
    public Canvas canvas;
    [Tooltip("角色的武器")]
    private Weapon weaponComponent;
    [Tooltip("角色的精灵图")]
    private SpriteRenderer[] spriteRenderers;
    [Tooltip("角色的移动组件")]
    private MoveBase moveComponent;
    [Tooltip("角色的buff管理器")]
    private BuffManager buffManager;
    [Tooltip("角色的配置文件")]
    private RoleConfiguration roleConfig;
    //[Tooltip("角色的武器配置文件")]
    //private WeaponConfiguration weaponConfig;

    //private float damagePercent = 1f; // 伤害加成百分比
    //private void OnEnable()
    //{
    //    EventCenter.Subscribe<SetFlashEvent, float, Color>(SetFlash);
    //}

    //private void OnDestroy()
    //{
    //    EventCenter.Unsubscribe<SetFlashEvent, float, Color>(SetFlash);
    //}

    private void Update()
    {
        //if (!buffManager.HasBuff(BuffType.EnergyWeak))
        //{
        //    energyRegenValue += energyRegenRate * Time.deltaTime;
        //    if (energyRegenValue >= 1f)
        //    {
        //        int energyToAdd = Mathf.FloorToInt(energyRegenValue);
        //        Energy(energyToAdd);
        //        energyRegenValue -= energyToAdd;
        //    }
        //}

    }

    /// <summary>
    /// 角色初始化
    /// </summary>
    /// <param name="config"></param>
    /// <param name="weaponConfig"></param>
    public override void Init(RoleConfiguration config)
    {
        base.Init(config);
        this.roleConfig = config;

        // 初始化移动组件
        moveComponent = GetComponent<MoveBase>();
        moveComponent.Init(config);

        // 初始化武器组件
        weaponComponent = GetComponentInChildren<Weapon>();
        weaponComponent.Init(this, config);

        //获取画布
        canvas = GetComponentInChildren<Canvas>();
        
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        //初始化Buff管理器
        buffManager = GetComponent<BuffManager>();
        //buffManager.Init();

        base.Init(config);
        // 初始数据更新
        //UpdateData();
    }

    public override void TakeDamage(float amount)
    {
        
    }

    //

    private void SetFlash(float speed, Color color)
    {
        foreach (var sr in spriteRenderers)
        {
            Material mat = sr.material;
            mat.SetFloat("_Speed", speed);
            mat.SetColor("_Color", color);
        }
    }

    ///// <summary>
    ///// 当能量耗尽时调用的方法。
    ///// </summary>
    //private void OnEnergyDepleted()
    //{
    //    if (buffManager != null && !buffManager.HasBuff(BuffType.EnergyWeak))
    //    {
    //        buffManager.AddBuff<EnergyWeak>();
    //        var t = ObjectPoolManager.Instance.GetObject<TextPopUp>("TextPopUp", transform.position, Quaternion.identity);
    //        if (t != null && canvas != null)
    //        {
    //            t.transform.SetParent(canvas.transform);
    //            t.Show("能量耗尽！", 2, Color.white, 0.5f, true);
    //        }
    //    }
    //}
}
