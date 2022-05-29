using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerObject : MonoBehaviour
{
    [SerializeField] private TowerAttackArea _attackArea;
    private Item _tower;

    void Awake()
    {
        name = name.Substring(0, name.Length - 7);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateTower(Item tower)
    {
        _tower = tower;
    }

    public void ActiveTower()
    {

    }

    /*
    IEnumerator attackCoroutine;
    
    private void Attack()
    {
        StopAttack();

        attackCoroutine = AttackTimer();
        StartCoroutine(AttackTimer());
    }

    private void StopAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }
    IEnumerator AttackTimer()
    {
        yield return new WaitForSeconds(_enemy.attackDelay);

        if (_isMoving == false)
        {
            Player.Instance.Damaged(_enemy.dmg);
            Attack();
        }
    }
    */
}
