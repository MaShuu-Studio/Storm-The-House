using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TowerAttackArea : MonoBehaviour
{
    private TowerObject _tower;
    private BoxCollider _collider;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }

    public void Initialize(TowerObject tower, float size)
    {
        _tower = tower;

        _collider.center = Vector3.left * size / 20;
        _collider.size = new Vector3(size / 10, 1, 15);
        transform.localPosition = new Vector3(0, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            _tower.AddEnemy(other.gameObject, true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            _tower.AddEnemy(other.gameObject, false);
        }
    }
}
