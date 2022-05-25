using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundController : MonoBehaviour
{
    private int _round;
    private Dictionary<string, IEnumerator> _spawnCoroutines;

    // Start is called before the first frame update
    void Start()
    {
        _spawnCoroutines = new Dictionary<string, IEnumerator>();
        SetRound(1);
        SetData();
    }

    public void SetRound(int round)
    {
        _round = round;
    }
    public void SetData()
    {
        foreach (IEnumerator coroutine in _spawnCoroutines.Values)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
        _spawnCoroutines.Clear();

        List<RoundEnemyData> roundData = RoundManager.GetData(_round);

        foreach(RoundEnemyData data in roundData)
        {
            _spawnCoroutines.Add(data.name, Spawn(data.name, data.delay, data.delayRange));
            StartCoroutine(_spawnCoroutines[data.name]);
        }
    }

    IEnumerator Spawn(string name, float delay, float delayRange)
    {
        float time = Random.Range(delay - delayRange, delay + delayRange);
        yield return new WaitForSeconds(time);

        float zpos = Random.Range(5f, -5f);

        Vector3 pos = new Vector3(-30, 0.75f, zpos);

        GameObject obj = ObjectPool.GetObject<Enemy>(name);
        obj.transform.position = pos;

        _spawnCoroutines[name] = Spawn(name, delay, delayRange);
        StartCoroutine(_spawnCoroutines[name]);
    }
}
