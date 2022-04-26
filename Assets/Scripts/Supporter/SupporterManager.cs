using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Data;

public static class SupporterManager
{
    private static Dictionary<string, Supporter> _supporters;
    public static List<string> Types { get; private set; }

    public static void Initialize()
    {
        Debug.Log("[SYSTEM] LOAD SUPPORTER");
        List<Supporter> list = DataManager.Deserialize<Supporter>();

        _supporters = new Dictionary<string, Supporter>();

        for(int i = 0; i < list.Count; i++)
        {
            _supporters.Add(list[i].name, list[i]);
        }

        Types = _supporters.Keys.ToList();
    }

    public static int Damage(string name)
    {
        if (!_supporters.ContainsKey(name)) return -1;
        
        return _supporters[name].dmg;
    }

    public static float Delay(string name, int amount)
    {
        if (!_supporters.ContainsKey(name)) return -1;
        if (amount <= 0) return -1;
        return _supporters[name].delay / amount;
    }
}
