using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public static class ItemManager
{
    public static List<Item> Weapons { get { return weapons; } }
    private static List<Item> weapons;
    public static Dictionary<string, Item> Towers { get { return towers; } }
    private static Dictionary<string, Item> towers;
    public static List<string> TowerNames { get; private set; }

    public static void Initialize()
    {
        List<Item> list = DataManager.Deserialize<Item>();

        weapons = new List<Item>();
        towers = new Dictionary<string, Item>();

        foreach (Item item in list)
        {
            if (item.type == EnumData.ItemType.WEAPON)
                weapons.Add(item);
            else if (item.type == EnumData.ItemType.TOWER)
                towers.Add(item.name, item);
        }
        TowerNames = new List<string>(towers.Keys.ToList());

        Debug.Log("[SYSTEM] LOAD ITEMS");
    }

    public static float FireRate(int firerate)
    {
        return 1 / firerate;
    }

    public static float Reload(int reload)
    {
        return 3 / reload;
    }
}
