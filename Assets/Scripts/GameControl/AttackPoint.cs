using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

[RequireComponent(typeof(BoxCollider))]
public class AttackPoint : MonoBehaviour
{

    /*
    private IEnumerator _coroutine = null;
    private float _damage;
    public float Damage { get { return _damage; } }
    private float _remainTime;
    private float _accurancy;
    private bool _isRemain;
    private bool _isAttack;

    public void Attack(Vector3 pos, float time = 0)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        _isAttack = true;

        transform.position = pos;
        gameObject.SetActive(true);

        if (time == 0) time = _remainTime;

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

    public void SetData(Item weapon)
    {
        _damage = weapon.GetValue(UpgradeDataType.DAMAGE);
        _remainTime = weapon.GetValue(UpgradeDataType.REMAINTIME);
        _accurancy = weapon.GetValue(UpgradeDataType.ACCURANCY);
        _isRemain = weapon.isRemain;

        gameObject.name = _damage.ToString();
    }

    public void SetData(int damage)
    {
        _damage = damage;
    }

    private void ActiveFalse()
    {

    }*/
    /*
    public void EnemyDamaged(EnemyObject enemy)
    {
        if (_isAttack == false) return;

        enemy.Damaged(_damage);

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        if (_isRemain == false)
            gameObject.SetActive(false);

        _isAttack = false;
    }*/
}
