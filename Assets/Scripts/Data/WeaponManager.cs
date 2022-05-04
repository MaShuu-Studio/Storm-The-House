using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public static class WeaponManager
{
    public static List<Weapon> weapons;

    public static void Initialize()
    {
        Debug.Log("[SYSTEM] LOAD WEAPON");
        weapons = DataManager.Deserialize<Weapon>();
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
