using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerHitBox : MonoBehaviour
{
    private int _index;
    public void Initialize(int index)
    {
        _index = index;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AttackPoint")
        { 
            TowerController.Instance.SelectTower(_index);
        }
    }
}
