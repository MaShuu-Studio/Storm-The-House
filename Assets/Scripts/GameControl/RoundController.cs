using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundController : MonoBehaviour
{
    public static RoundController Instance { get { return instance; } }
    private static RoundController instance;

    [SerializeField] private GameObject sun;
    private float maxXpos = 35;
    private float minYpos = -5;
    private float maxYpos = 15;
    private float zpos = 10;

    IEnumerator _roundCoroutine;

    private int _round;
    private float _roundTime = 90;
    private bool _progressRound;
    public bool ProgressRound { get { return _progressRound; } }

    private GameObject enemiesParent;
    private Dictionary<string, IEnumerator> _spawnCoroutines;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(sun);

        _spawnCoroutines = new Dictionary<string, IEnumerator>();
        enemiesParent = new GameObject("Enemies");

        //sun.transform.position = new Vector3(-maxXpos, minYpos, zpos);
        //sun.transform.LookAt(transform.position);
    }

    // Start is called before the first frame update
    void Start()
    {
        NewRound();
    }

    public void NewRound()
    {
        _round = 0;
    }

    public void NextRound()
    {
        _round++;
        _progressRound = true;
        Player.Instance.StartRound();

        if (_roundCoroutine != null)
        {
            StopCoroutine(_roundCoroutine);
            _roundCoroutine = null;
        }
        _roundCoroutine = Progress();

        StartCoroutine(_roundCoroutine);
    }

    private void StopSpawn()
    {
        foreach (IEnumerator coroutine in _spawnCoroutines.Values)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }
        _spawnCoroutines.Clear();
    }

    private void SetData()
    {
        StopSpawn();

        List<RoundEnemyData> roundData = RoundManager.GetData(_round);

        if (roundData != null)
            foreach (RoundEnemyData data in roundData)
            {
                _spawnCoroutines.Add(data.name, Spawn(data.name, data.amount));
                StartCoroutine(_spawnCoroutines[data.name]);
            }
    }

    private void RoundEnd()
    {
        UIController.Instance.OpenShop(true);
        WeaponController.Instance.RefillAmmo();

        StopSpawn();

        while(enemiesParent.transform.childCount > 0)
        {
            GameObject child = enemiesParent.transform.GetChild(0).gameObject;

            ObjectPool.ReturnObject(child.name, child);
        }

        _progressRound = false;
    }

    private IEnumerator Progress()
    {
        SetData();

        float time = 0;
        while (_roundTime > time)
        {
            time += Time.deltaTime;

            float radian = time / _roundTime * Mathf.PI;

            sun.transform.position = new Vector3(maxXpos * Mathf.Cos(radian), minYpos + maxYpos * Mathf.Sin(radian), zpos);
            sun.transform.LookAt(transform.position);

            yield return null;
        }

        RoundEnd();
    }

    private IEnumerator Spawn(string name, int amount)
    {
        float delay = _roundTime / (float)amount;
        float delayRange = delay * 0.2f;
        float time = Random.Range(delay - delayRange, delay + delayRange);

        yield return new WaitForSeconds(time);

        float zpos = Random.Range(5f, -5f);

        Vector3 pos = new Vector3(-30, 0.75f, zpos);

        GameObject obj = ObjectPool.GetObject<Enemy>(name);
        obj.transform.SetParent(enemiesParent.transform);
        obj.transform.position = pos;

        _spawnCoroutines[name] = Spawn(name, amount);
        StartCoroutine(_spawnCoroutines[name]);
    }
}
