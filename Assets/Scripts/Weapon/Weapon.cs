using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Weapon
{
    /* AMMO: 탄창 수
     * FIRE RATE: 발사 속도
     * ACCURANCY: 탄퍼짐 정도
     * RELOAD: 장전 속도
     * RANGE: 공격 범위
     */
    private int _ammo;
    private float _accuracy;
    private float _reload;
    private float _fireRate;
    private float _range;

    /*
    private int _amUpgrade;
    private float _frUpgrade;
    private float _acUpgrade;
    private float _rlUpgrade;
    
    private int _amCost;
    private float _frCost;
    private float _acCost;
    private float _rlCost;
    */
    
    public int Ammo
    {
        get { return _ammo; }
    }

    public float ReloadTime
    {
        get { return 3.0f / _reload; }
    }

    public float FireTime
    {
        get { return 1.0f / _fireRate; }
    }

    public Weapon(int am, float ac, float rl, float fr, float rg)
    {
        _ammo = am;
        _accuracy = ac;
        _reload = rl;
        _fireRate = fr;
        _range = rg;
    }
}
