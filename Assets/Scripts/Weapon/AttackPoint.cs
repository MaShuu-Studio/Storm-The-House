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
    private int _damage;
    public int Damage { get { return _damage; } }
    private bool _isRemain;
    private bool _isAttack;

    public void Attack(float time, Vector3 pos, bool isRemain = false)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        _isAttack = true;
        _isRemain = isRemain;

        transform.position = pos;
        gameObject.SetActive(true);

        _coroutine = AttackTimer(time);
        StartCoroutine(_coroutine);
    }

    IEnumerator AttackTimer(float time)
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }

    public void SetDamage(int damage)
    {
        _damage = damage;
    }

    public void EnemyDamaged(Enemy enemy)
    {
        if (_isAttack == false) return;

        enemy.Damaged(_damage);

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        gameObject.SetActive(false);
        _isAttack = false;
    }
}
