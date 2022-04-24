using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SupporterManager
{
    // 추후 데이터 테이블 통해 관리
    public static List<string> supporterNames { get; private set; } =
        new List<string>()
        {
            "GUNMAN",
            "REPAIRMAN",
        };

    private static Dictionary<string, Supporter> supporters =
        new Dictionary<string, Supporter>()
        {
            {
                "GUNMAN", new Supporter()
                {
                    dmg = 1,
                    accurancy = 0.7f,
                    probablity = 0.7f,
                    delay = 5.0f
                }
            },
            {
                "REPAIRMAN", new Supporter()
                {
                    dmg = 1,
                    accurancy = 1,
                    probablity = 1,
                    delay = 5.0f
                }
            },
        };

    public static int Damage(string name)
    {
        if (!supporters.ContainsKey(name)) return -1;
        
        return supporters[name].dmg;
    }

    public static float Delay(string name, int amount)
    {
        if (!supporters.ContainsKey(name)) return -1;
        if (amount <= 0) return -1;
        return supporters[name].delay / amount;
    }
}
