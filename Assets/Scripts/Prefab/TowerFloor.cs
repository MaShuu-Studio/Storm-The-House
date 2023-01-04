using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerFloor : MonoBehaviour
{
    private int index;

    public void SetIndex(int i)
    {
        index = i;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AttackPoint")
        {
            if (Player.Instance.ReadyBuyTower)
                Player.Instance.BuyTower(index);

            TowerController.Instance.SelectTower(index);
        }
    }
}
