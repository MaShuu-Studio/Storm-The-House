using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class EnemyObject : MonoBehaviour
{
    [SerializeField] private EnemyAttackArea _attackArea;
    private Enemy _enemy;

    private float _hp;

    private bool _isMoving = true;

    void Awake()
    {
        name = name.Substring(0, name.Length - 7);
    }

    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isMoving)
            Move();
    }

    public void Initialize()
    {
        Enemy enemy = EnemyManager.Enemies[name];

        _enemy = enemy;
        _attackArea.Initialize(this, enemy.attackRange);

        _hp = _enemy.hp;
    }

    private void Recover()
    {
        _hp = _enemy.hp;
    }

    private void Move()
    {
        transform.position += Vector3.right * _enemy.speed * Time.deltaTime;
    }

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

    public void MoveOrAttack(bool isMoving)
    {
        _isMoving = isMoving;

        if (_isMoving == false)
            Attack();
        else StopAttack();
    }

    public void Damage(float dmg)
    {
        _hp -= dmg;
        Debug.Log("[SYSTEM] ENEMY DAMAGED" + _hp);

        if (_hp <= 0)
        {
            StopAttack();

            ObjectPool.ReturnObject(name, gameObject);

            Recover();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AttackPoint")
        {
            AttackController.Instance.EnemyDamaged(this, other.gameObject);
        }
    }
}
