using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyAttackArea : MonoBehaviour
{
    private EnemyObject _enemy;
    private SphereCollider _collider;

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
    }

    public void Initialize(EnemyObject enemy, float size)
    {
        _enemy = enemy;

        _collider.radius = size;
        transform.localPosition = new Vector3(0, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Barricade")
        {
            _enemy.MeetBrricade(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Barricade")
        {
            _enemy.MeetBrricade(false);
        }
    }
}
