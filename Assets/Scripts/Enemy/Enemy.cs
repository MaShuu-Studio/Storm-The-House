using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int _hp;
    [SerializeField] private int _dmg;
    [SerializeField] private int _speed;

    private bool _isMoving = true;
    private IEnumerator _motionCoroutine;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_hp <= 0)
        {
            Destroy(gameObject);
        }

        if (_isMoving)
        {
            Move();
        }
        else
        {
            Attack();
        }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Attack Point")
        {
            _hp--;
            Debug.Log("[SYSTEM] ENEMY HP: " + _hp);
            other.gameObject.SetActive(false);
        }
    }
}
