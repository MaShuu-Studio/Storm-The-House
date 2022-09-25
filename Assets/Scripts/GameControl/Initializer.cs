using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    public static Initializer Instance { get { return instance; } }
    private static Initializer instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        SupporterManager.Initialize();
        ItemManager.Initialize();
        EnemyManager.Initialize();
        RoundManager.Initialize();
        SoundManager.Initialize();
    }
    // Start is called before the first frame update
    void Start()
    {
        ObjectPool.Instance.Initialize();
        SoundController.Instance.Initialize();
    }
}
