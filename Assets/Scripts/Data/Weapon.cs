using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Weapon
{
    /* AMMO: 탄창 수
     * FIRE RATE: 발사 속도
     * ACCURANCY: 탄퍼짐 정도
     * RELOAD: 장전 속도
     * RANGE: 공격 범위
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
