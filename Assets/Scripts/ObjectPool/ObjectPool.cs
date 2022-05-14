using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public abstract class ObjectPool : MonoBehaviour
{
    private static ObjectPool instance;
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
        instance = this;
    }

    void Start()
    {
        Initialize();
    }

    protected abstract void Initialize();
    protected static void CreateNewObject<T>(string name)
    {
        GameObject gbo = Instantiate(DataManager.GetPrefab<T>(name));

        gbo.transform.SetParent(Instance.PoolParent[name]);
        gbo.SetActive(false);

        Instance.Pool[name].Enqueue(gbo);
    }

    public static GameObject GetObject<T>(string name)
    {
        if (Instance.Pool[name].Count <= 0)
            CreateNewObject<T>(name);

        GameObject gbo = Instance.Pool[name].Dequeue();
        gbo.transform.SetParent(null);
        gbo.SetActive(true);

        return gbo;
    }

    public static void ReturnObject(string name, GameObject gbo)
    {
        gbo.transform.SetParent(Instance.PoolParent[name]);
        gbo.SetActive(false);

        Instance.Pool[name].Enqueue(gbo);
    }
}
