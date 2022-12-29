using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Data;
using EnumData;

public static class SupporterManager
{
    private static Dictionary<string, Supporter> _supporters;
    public static List<string> Types { get; private set; }
    public static List<string> Units { get; private set; }
    public static List<string> Upgradable { get; private set; }
    
    public static void Initialize()
    {
        List<Supporter> list = DataManager.Deserialize<Supporter>();

        _supporters = new Dictionary<string, Supporter>();
        Units = new List<string>();
        Upgradable = new List<string>();

        for (int i = 0; i < list.Count; i++)
        {
            _supporters.Add(list[i].name, list[i]);
            if (list[i].type == SupporterType.UNIT) Units.Add(list[i].name);
            else if (list[i].maxupgrade > 0) Upgradable.Add(list[i].name);
        }

        Types = _supporters.Keys.ToList();
        Debug.Log("[SYSTEM] LOAD SUPPORTER");
    }

    public static int Value(string name)
    {
        if (!_supporters.ContainsKey(name)) return -1;
        
        return _supporters[name].value;
    }

    public static float Delay(string name, int amount)
    {
        if (!_supporters.ContainsKey(name)) return -1;
        if (amount <= 0) return -1;
        return _supporters[name].delay / amount;
    }

    public static SupporterType Type(string name)
    {
        return _supporters[name].type;
    }

    public static SupporterAbilityType Ability(string name)
    {
        return _supporters[name].ability;
    }

    public static int MaxLevel(string name)
    {
        return _supporters[name].maxupgrade;
    }

    public static int Cost(string name, int amount)
    {
        int cost = _supporters[name].cost;
        if (_supporters[name].upgradecost != 0)
        {
            for (int i = 0; i < amount; i++)
            {
                cost += _supporters[name].upgradecost;
            }
        }

        if (_supporters[name].multiplyupgradecost != 0)
        {
            for (int i = 0; i < amount && i < MaxLevel(name) - 1; i++)
            {
                cost = (int)(cost * _supporters[name].multiplyupgradecost);
            }
        }

        return cost;
    }
}
