using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
[CreateAssetMenu(fileName = "New Weapon Configuration", menuName = "Game/Weapon Configuration")]
public class WeaponConfiguration : ScriptableObject
{
    [Header("��������")]
    [Tooltip("����")]
    public string weaponName = "Default Weapon";

    [Tooltip("�������")]
    public float attackInterval = 1.0f;

    [Tooltip("����״̬����������ӣ��ٷֱȣ�")]
    public float weakenedAttackInterval = 0.2f;

    [Tooltip("����״̬�˺�˥�����ٷֱȣ�")]
    public float weakenedDamageReduction = 0.2f;

    [Tooltip("�ӵ�Ԥ����")]
    public GameObject bulletPrefab;

    [Header("�˺�ռ��")]
    [Tooltip("�����˺�ռ��")]
    public float physicalDamageRatio = 0;

    [Tooltip("Ԫ���˺�ռ��")]
    public float elementalDamageRatio = 0;

    [Tooltip("�����˺�ռ��")]
    public float energyDamageRatio = 0;
}
