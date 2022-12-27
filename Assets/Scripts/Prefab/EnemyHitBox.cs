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
            Debug.Log($"[SYSTEM] HIT {enemy.name}");
            AttackController.Instance.EnemyDamaged(enemy, other.gameObject);
        }
    }
}
