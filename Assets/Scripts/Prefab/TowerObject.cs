using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class TowerObject : MonoBehaviour
{
    [SerializeField] private TowerAttackArea _attackArea;
    private Item _tower;
    private Dictionary<UpgradeDataType, float> _attackTypes = new Dictionary<UpgradeDataType, float>();
    private bool _attackTower;
    private bool _shield;

    private List<GameObject> _enemies = new List<GameObject>();

    public void UpdateTower(Item tower)
    {
        _tower = tower;
        name = _tower.name;
        _attackTower = false;

        foreach(UpgradeDataType type in tower.data.Keys)
        {
            if (type == UpgradeDataType.DAMAGE)
                _attackTower = true;
            else if (type == UpgradeDataType.SHIELD)
                _shield = true;
            else if (type >= UpgradeDataType.SLOW)
            {
                _attackTypes.Add(type, tower.GetValue(type));
            }
        }

        _attackArea.Initialize(this, tower.GetValue(UpgradeDataType.RANGE));
    }

    IEnumerator coroutine;

    public void ActiveTower()
    {
        if (_enemies.Count > 0 && coroutine == null)
        {
            coroutine = Active();
            StartCoroutine(coroutine);
        }
    }

    IEnumerator Active()
    {
        int count = _enemies.Count;
        float dmg = 0;
        for (int i = 0; i < count; i++)
        {
            if (_attackTower) dmg = _tower.GetValue(UpgradeDataType.DAMAGE);
            AttackController.Instance.TowerAttack(_enemies[i].transform.position, dmg, _attackTypes);
        }

        yield return new WaitForSeconds(1 / _tower.GetValue(UpgradeDataType.FIRERATE));

        coroutine = null;
        if (_enemies.Count > 0) ActiveTower();
    }

    public void AddEnemy(GameObject enemy, bool add)
    {
        if (add)
        {
            _enemies.Add(enemy);
            ActiveTower();
        }
        else
        {
            for(int i =0; i< _enemies.Count; i++)
            {
                if (_enemies[i] == enemy)
                {
                    _enemies.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
