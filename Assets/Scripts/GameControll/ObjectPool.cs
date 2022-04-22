using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class ObjectPool : MonoBehaviour
{
    private static ObjectPool instance;
    public static ObjectPool Instance { get { return instance; } }

    [SerializeField] private List<GameObject> _objectPrefabs;
    public Dictionary<EnemyType, Queue<GameObject>> _pool;
    public Dictionary<EnemyType, Transform> _poolParent;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        Initialize();
    }

    private void Initialize()
    {
        int enemyCount = System.Enum.GetValues(typeof(EnemyType)).Length;

        _pool = new Dictionary<EnemyType, Queue<GameObject>>();
        _poolParent = new Dictionary<EnemyType, Transform>();

        for (int i = 0; i < enemyCount; i++)
        {
            EnemyType type = (EnemyType)i;
            GameObject p = new GameObject(((EnemyType)i).ToString());
            p.transform.SetParent(transform);

            _pool.Add(type, new Queue<GameObject>());
            _poolParent.Add(type, p.transform);

            for (int j = 0; j < 10; j++)
            {
                CreateNewObject(type);
            }
        }
    }

    private static void CreateNewObject(EnemyType type)
    {
        int index = (int)type;
        GameObject gbo = Instantiate(Instance._objectPrefabs[index]);

        gbo.transform.SetParent(Instance._poolParent[type]);
        gbo.SetActive(false);

        Instance._pool[type].Enqueue(gbo);
    }

    public static GameObject GetObject(EnemyType type)
    {
        if (Instance._pool[type].Count <= 0)
            CreateNewObject(type);

        GameObject gbo = Instance._pool[type].Dequeue();
        gbo.transform.SetParent(null);
        gbo.SetActive(true);

        return gbo;
    }

    public static void ReturnObject(EnemyType type, GameObject gbo)
    {
        int index = (int)type;

        gbo.transform.SetParent(Instance._poolParent[type]);
        gbo.SetActive(false);

        Instance._pool[type].Enqueue(gbo);
    }
}
