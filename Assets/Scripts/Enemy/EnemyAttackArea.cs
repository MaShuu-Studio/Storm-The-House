using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackArea : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Barricade")
        {
            enemy.MoveOrAttack(false);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Barricade")
        {
            enemy.MoveOrAttack(true);
        }
    }
}
