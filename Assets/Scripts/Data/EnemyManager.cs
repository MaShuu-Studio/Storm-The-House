using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class EnemyManager
{
    public static Dictionary<string, Enemy> Enemies { get { return _enemies; } }
    private static Dictionary<string, Enemy> _enemies;
    public static List<string> Types { get; private set; }

    public static void Initialize()
    {
        List<Enemy> list = DataManager.Deserialize<Enemy>();
        _enemies = new Dictionary<string, Enemy>();

        foreach (Enemy enemy in list)
        {
            _enemies.Add(enemy.name, enemy);
        }

        Types = new List<string>(_enemies.Keys.ToList());

        Debug.Log("[SYSTEM] LOAD ENEMIES");
    }
}
