using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : ObjectPool
{
    protected override void Initialize()
    {
        _pool = new Dictionary<string, Queue<GameObject>>();
        _poolParent = new Dictionary<string, Transform>();

        for (int i = 0; i < EnemyManager.Enemies.Count; i++)
        {
            string name = EnemyManager.Types[i];

            GameObject p = new GameObject(name);
            p.transform.SetParent(transform);

            _pool.Add(name, new Queue<GameObject>());
            _poolParent.Add(name, p.transform);

            for (int j = 0; j < 10; j++)
            {
                CreateNewObject<Enemy>(name);
            }
        }
    }
}
