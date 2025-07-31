using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ɫ�������ݣ�ʹ��ScriptableObject�洢��ɫ����
/// </summary>
[CreateAssetMenu(fileName = "New Role Configuration", menuName = "Game/Role Configuration")]
public class RoleConfiguration : ScriptableObject
{
    [Header("������Ϣ")]
    [Tooltip("��ɫ����")]
    public string roleName = "Default Role";

    [Tooltip("��ɫ����")]
    [TextArea(3, 5)]
    public string description = "��ɫ����...";

    [Tooltip("����ɫ")]
    public Color themeColor = Color.white;

    [Tooltip("��ɫԤ�����ַ��Addressables��")]
    public string rolePrefabAddress = "Role";

    [Header("��������")]
    [Tooltip("�������ֵ")]
    public int maxHealth = 100;
   
    [Tooltip("�����ظ�(ÿ��)")]
    public float healthRegenRate = 0f;

    [Tooltip("����")]
    public float energy = 100f;

    [Tooltip("�����ظ�(ÿ��)")]
    public float energyRegenRate = 0f;

    [Tooltip("��ɫͼ��")]
    public Sprite roleIcon;

    [Tooltip("��ɫ����")]
    public Sprite standImage;

    [Header("�ƶ�����")]
    [Tooltip("�ƶ��ٶ�")]
    public float moveSpeed = 5f;

    [Tooltip("����ٶ�")]
    public float dashSpeed = 12f;

    [Tooltip("�����ȴʱ��")]
    public float dashCooldown = 0.5f;

    [Tooltip("�������")]
    public float attackInterval = 0.3f;

    [Tooltip("�����˺�")]
    public float physicalDamage = 0f;

    [Tooltip("Ԫ���˺�")]
    public float elementalDamage = 0f;

    [Tooltip("�����˺�")]
    public float energyDamage = 0f;

    [Header("����")]
    [Tooltip("����ֵ")]
    public int shield = 0;

    [Tooltip("����ֵ")]
    public float sanity = 100f;

    [Header("��������")]
    [Tooltip("���������ļ�")]
    public List<WeaponConfiguration> weaponConfig = new();
}