using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public static class RoundManager
{
    private static List<Round> _rounds;

    public static void Initialize()
    {
        _rounds = DataManager.Deserialize<Round>();
        Debug.Log("[SYSTEM] LOAD ROUND DATA");
    }

    public static List<RoundEnemyData> GetData(int round)
    {
        int index = round - 1;

        if (_rounds.Count > index)
            return _rounds[index].enemies;

        return null;
    }
}
