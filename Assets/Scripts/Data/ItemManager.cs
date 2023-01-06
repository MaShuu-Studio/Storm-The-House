using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public static class ItemManager
{
    public static Dictionary<string, Item> Weapons { get { return weapons; } }
    private static Dictionary<string, Item> weapons;
    public static Dictionary<string, Item> Towers { get { return towers; } }
    private static Dictionary<string, Item> towers;
    public static List<string> TowerNames { get; private set; }
    public static List<string> WeaponNames { get; private set; }

    public static void Initialize()
    {
        List<Item> list = DataManager.Deserialize<Item>();

        weapons = new Dictionary<string, Item>();
        towers = new Dictionary<string, Item>();

        foreach (Item item in list)
        {
            if (item.data.ContainsKey(EnumData.UpgradeDataType.REMAINTIME)) item.isRemain = true;
            if (item.effect == null) item.effect = "";

            if (item.type == EnumData.ItemType.WEAPON)
                weapons.Add(item.name, item);
            else if (item.type == EnumData.ItemType.TOWER)
                towers.Add(item.name, item);
        }
        WeaponNames = new List<string>(weapons.Keys.ToList());
        TowerNames = new List<string>(towers.Keys.ToList());

        Debug.Log("[SYSTEM] LOAD ITEMS");
    }

    public static float FireRate(float firerate)
    {
        if (firerate == 0) return 0;
        return 10 / firerate;
    }

    public static float Reload(float reload)
    {
        if (reload == 0) reload = 1;
        return 3 / reload;
    }

    public static int Value(Item item)
    {
        int value = item.cost;
        Item tmpItem = new Item((item.type == EnumData.ItemType.WEAPON) ? Weapons[item.name] : Towers[item.name]);

        foreach (EnumData.UpgradeDataType type in tmpItem.data.Keys)
        {
            UpgradeData curData = tmpItem.data[type];
            UpgradeData itemData = item.data[type];
            while (curData.currentValue < itemData.currentValue)
            {
                int infmoney = 9999999;
                value += curData.cost;
                curData.Upgrade(ref infmoney);
            }
        }

        return value;
    }
}
