using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using System.Linq;

public static class EffectManager
{
    public static Dictionary<string, GameObject> Effects { get { return effects; } }
    private static Dictionary<string, GameObject> effects;
    public static List<string> effectNames { get; private set; }

    public static void Initialize()
    {
        List<GameObject> list = DataManager.GetObjects<GameObject>(DataManager.EffectPath);

        effects = new Dictionary<string, GameObject>();

        foreach (GameObject effect in list)
        {
            effects.Add(effect.name.ToUpper(), effect);
        }

        effectNames = effects.Keys.ToList();

        Debug.Log("[SYSTEM] LOAD EFFECTS");
    }
}
