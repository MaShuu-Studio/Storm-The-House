using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;
using Data;

[Serializable]
public class Item
{
    /* 
     * AMMO         0
     * ACCURANCY    1
     * RELOAD       2
     * FIRERATE     3
     * DAMAGE       4
     * RANGE        5
     */
    public ItemType type;
    public string name = "";
    public int cost = 0;
    public string description = "";
    public bool isRemain = false;
    public bool available = false;

    public SerializableDictionary<UpgradeDataType, UpgradeData> data;

    public float GetValue(UpgradeDataType type)
    {
        if (!data.ContainsKey(type))
        {
            float defaultValue = 1;
            switch (type)
            {
                case UpgradeDataType.SHIELD:
                    defaultValue = 0;
                    break;

                case UpgradeDataType.RANGE:
                    if (this.type == ItemType.WEAPON) defaultValue = 0.5f;
                    else defaultValue = 50;
                    break;

                case UpgradeDataType.REMAINTIME:
                    defaultValue = 0.1f;
                    break;

                case UpgradeDataType.MISSILES:
                    defaultValue = 1;
                    break;

                case UpgradeDataType.DELAY:
                    defaultValue = 0;
                    break;
            }
            return defaultValue;
        }

        return data[type].currentValue;
    }

    public Item(Item item)
    {
        type = item.type;
        name = item.name;
        cost = item.cost;
        description = item.description;
        available = item.available;
        data = new SerializableDictionary<UpgradeDataType, UpgradeData>();

        foreach (UpgradeDataType t in item.data.Keys)
        {
            data.Add(t, new UpgradeData(item.data[t]));
        }
    }
}

[Serializable]
public class UpgradeData
{
    public float defaultValue = -1;
    public float currentValue;
    public float upgradeValue = 0;
    public float maxValue;
    public int cost = 0;

    public UpgradeData()
    {
    }

    public UpgradeData(UpgradeData data)
    {
        defaultValue = data.defaultValue;
        currentValue = data.currentValue;
        upgradeValue = data.upgradeValue;
        maxValue = data.maxValue;
        cost = data.cost;
    }

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