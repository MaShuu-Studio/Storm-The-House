using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public static class WeaponManager
{
    public static List<Weapon> Weapons { get { return weapons; } }
    private static List<Weapon> weapons;

    public static void Initialize()
    {
        weapons = DataManager.Deserialize<Weapon>();
        Debug.Log("[SYSTEM] LOAD WEAPON");
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
