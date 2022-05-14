using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;
using System;

public class EnemyObject : MonoBehaviour
{
    private float _originHp;
    private float _originDmg;
    private float _originSpeed;

    private float _hp;
    private float _dmg;
    private float _speed;

    private bool _isMoving = true;

    void Awake()
    {
        name = name.Substring(0, name.Length - 7);
        Initialize();
    }
    // Update is called once per frame
    void Update()
    {
        if (_isMoving)
        {
            Move();
        }
        else
        {
            Attack();
        }
    }

    public void Initialize()
    {
        Enemy enemy = EnemyManager.Enemies[name];

        _hp = enemy.hp;
        _dmg = enemy.dmg;
        _speed = enemy.speed;
    }

    private void Recover()
    {
        _hp = _originHp;
        _dmg = _originDmg;
        _speed = _originSpeed;
    }

    private void Move()
    {
        transform.position += Vector3.right * _speed * Time.deltaTime;
    }

    private void Attack()
    {

    }

    public void MoveOrAttack(bool isMoving)
    {
        _isMoving = isMoving;
    }

    public void Damaged(float dmg)
    {
        _hp -= dmg;
        Debug.Log("[SYSTEM] ENEMY DAMAGED" + _hp);

        if (_hp <= 0)
        {
            ObjectPool.ReturnObject(name, gameObject);

            Recover();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AttackPoint")
        {
            AttackPointManager.Instance.EnemyDamaged(this, other.gameObject);
        }
    }
}
