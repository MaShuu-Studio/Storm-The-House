using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPoint : MonoBehaviour
{
    /*
    public static AttackPoint Instance { get { return instance; } }
    private static AttackPoint instance;
    */
    void Awake()
    {
        /*
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
        */
        _isRemain = false;
        gameObject.SetActive(false);
    }
    
    private IEnumerator _coroutine = null;
    private bool _isRemain;

    public void Attack(float time, Vector3 pos, bool isRemain = false)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        _isRemain = isRemain;

        transform.position = pos;
        gameObject.SetActive(true);

        _coroutine = AttackTimer(time);
        StartCoroutine(_coroutine);
    }

    IEnumerator AttackTimer(float time)
    {
        while(time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isRemain) return;

        if (other.tag == "Enemy")
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            gameObject.SetActive(false);
        }
    }
}
