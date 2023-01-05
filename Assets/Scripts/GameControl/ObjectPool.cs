using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class ObjectPool : MonoBehaviour
{
    protected static ObjectPool instance;
    public static ObjectPool Instance { get { return instance; } }

    public Dictionary<string, Queue<GameObject>> Pool { get { return _pool; } }
    public Dictionary<string, Transform> PoolParent { get { return _poolParent; } }

    protected Dictionary<string, Queue<GameObject>> _pool;
    protected Dictionary<string, Transform> _poolParent;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    public void Initialize()
    {
        _pool = new Dictionary<string, Queue<GameObject>>();
        _poolParent = new Dictionary<string, Transform>();

        // Enemy
        GameObject pp = new GameObject("ENEMIES");
        pp.transform.SetParent(transform);
        for (int i = 0; i < EnemyManager.Enemies.Count; i++)
        {
            string name = EnemyManager.Types[i];

            GameObject p = new GameObject(name);
            p.transform.SetParent(pp.transform);

            _pool.Add(name, new Queue<GameObject>());
            _poolParent.Add(name, p.transform);

            for (int j = 0; j < 10; j++)
            {
                CreateNewObject<Enemy>(name);
            }
        }

        // Tower
        pp = new GameObject("TOWERS");
        pp.transform.SetParent(transform);
        for (int i = 0; i < ItemManager.Towers.Count; i++)
        {
            string name = ItemManager.TowerNames[i];

            GameObject p = new GameObject(name);
            p.transform.SetParent(pp.transform);

            _pool.Add(name, new Queue<GameObject>());
            _poolParent.Add(name, p.transform);

            CreateNewObject<Item>(name);
        }

        // AttackPoint
        {
            string name = AttackController.pointName.ToUpper();

            GameObject p = new GameObject(name);
            p.transform.SetParent(transform);

            _pool.Add(name, new Queue<GameObject>());
            _poolParent.Add(name, p.transform);
            for (int i = 0; i < 50; i++)
            {
                CreateNewObject<GameObject>(name);
            }
        }

        // Effect
        pp = new GameObject("EFFECTS");
        pp.transform.SetParent(transform);
        for (int i = 0; i < EffectManager.Effects.Count; i++)
        {
            string name = EffectManager.effectNames[i];

            GameObject p = new GameObject(name);
            p.transform.SetParent(pp.transform);

            _pool.Add(name, new Queue<GameObject>());
            _poolParent.Add(name, p.transform);

            for (int j = 0; j < 10; j++)
            {
                CreateNewObject<Effect>(name);
            }
        }
    }

    protected static void CreateNewObject<T>(string name)
    {
        name = name.ToUpper();
        GameObject bo = DataManager.GetPrefab<T>(name);
        if (bo == null) return;

        GameObject gbo = Instantiate(bo);

        gbo.transform.SetParent(Instance.PoolParent[name]);
        gbo.SetActive(false);

        Instance.Pool[name].Enqueue(gbo);
    }

    public static GameObject GetObject<T>(string name)
    {
        name = name.ToUpper();
        if (Instance.Pool.ContainsKey(name) == false) return null;
        if (Instance.Pool[name].Count <= 0)
            CreateNewObject<T>(name);

        GameObject gbo = Instance.Pool[name].Dequeue();
        gbo.transform.SetParent(null);
        gbo.SetActive(true);

        return gbo;
    }

    public static void ReturnObject(string name, GameObject gbo)
    {
        name = name.ToUpper();
        gbo.transform.SetParent(Instance.PoolParent[name]);
        gbo.transform.localPosition = Vector3.zero;
        gbo.SetActive(false);

        Instance.Pool[name].Enqueue(gbo);
    }
}
