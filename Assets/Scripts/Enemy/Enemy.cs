using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyType type;

    [SerializeField] private int _originHp;
    [SerializeField] private int _originDmg;
    [SerializeField] private int _originSpeed;

     private int _hp;
     private int _dmg;
     private int _speed;

    private bool _isMoving = true;
    private IEnumerator _motionCoroutine;

    // Start is called before the first frame update
    void Start()
    {
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

    private void Initialize()
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

    public void Damaged(int dmg)
    {
        _hp -= dmg;
        Debug.Log("[SYSTEM] ENEMY DAMAGED" + _hp);

        if (_hp <= 0)
        {
            ObjectPool.ReturnObject(type, gameObject);

            Initialize();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Attack Point")
        {
            WeaponController.Instance.APoint.EnemyDamaged(this);
        }
    }
}
