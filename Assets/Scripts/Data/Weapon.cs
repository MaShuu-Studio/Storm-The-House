using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Weapon
{
    /* AMMO: źâ ��
     * FIRE RATE: �߻� �ӵ�
     * ACCURANCY: ź���� ����
     * RELOAD: ���� �ӵ�
     * RANGE: ���� ����
     */
    public string name;
    public int ammo;
    public int damage;
    public float accuracy;
    public float reload;
    public float firerate;
    public float range;

    /*
    private int amUpgrade;
    private float frUpgrade;
    private float acUpgrade;
    private float rlUpgrade;
    
    private int amCost;
    private float frCost;
    private float acCost;
    private float rlCost;
    */
}
