using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;
using Data;

[Serializable]
public class Weapon
{
    /* 
     * AMMO         0
     * ACCURANCY    1
     * RELOAD       2
     * FIRERATE     3
     * DAMAGE       4
     * RANGE        5
     */
    public string name = "";
    public int cost = 0;
    public bool available = false;

    public SerializableDictionary<WeaponDataType, WeaponData> data;

    public float GetValue(WeaponDataType type)
    {
        if (!data.ContainsKey(type)) return 1;

        return data[type].currentValue;
    }
}

[Serializable]
public class WeaponData
{
    public float defaultValue;
    public float currentValue;
    public float upgradeValue = 0;
    public float maxValue;
    public int cost;

    public void Upgrade(ref int money)
    {
        if (currentValue < maxValue && money >= cost)
        {
            money -= cost;
            currentValue += upgradeValue;
            cost += ((int)Math.Round(cost * 0.02f) * 10);
        }
    }
}