using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    [SerializeField] private EnemyObject enemy;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AttackPoint")
        {
            enemy.Hit(other.gameObject);
        }
    }
}
