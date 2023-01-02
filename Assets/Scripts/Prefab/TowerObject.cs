using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class TowerObject : MonoBehaviour
{
    [SerializeField] private TowerAttackArea _attackArea;
    [SerializeField] private TowerHitBox _hitbox;
    private Item _tower;
    private Dictionary<UpgradeDataType, float> _attackTypes = new Dictionary<UpgradeDataType, float>();
    private bool _isAttackTower;
    private bool _shield;

    private List<GameObject> _enemies = new List<GameObject>();

    public void UpdateTower(Item tower, int index)
    {
        _tower = tower;
        name = _tower.name;
        _isAttackTower = false;

        foreach (UpgradeDataType type in tower.data.Keys)
        {
            if (type == UpgradeDataType.DAMAGE)
                _isAttackTower = true;
            else if (type == UpgradeDataType.SHIELD)
                _shield = true;
            else if (type >= UpgradeDataType.SLOW)
            {
                _attackTypes.Add(type, tower.GetValue(type));
            }
        }

        _hitbox.Initialize(index);
        _attackArea.Initialize(this, _tower.GetValue(UpgradeDataType.RANGE));
    }

    public void Upgrade()
    {
        _attackArea.UpdateRange(_tower.GetValue(UpgradeDataType.RANGE));
    }

    IEnumerator coroutine;

    public void ActiveTower()
    {
        if (_shield) return;

        if (_enemies.Count > 0 && coroutine == null)
        {
            coroutine = Active();
            StartCoroutine(coroutine);
        }
    }

    public void RemoveTower()
    {
        StopAllCoroutines();

        ObjectPool.ReturnObject(name, gameObject);
    }

    IEnumerator Active()
    {
        int count = _enemies.Count;
        float dmg = 0;

        if (_isAttackTower) dmg = _tower.GetValue(UpgradeDataType.DAMAGE);

        for (int i = 0; i < count; i++)
        {
            GameObject targetHitbox = _enemies[i];
            if (targetHitbox == null || targetHitbox.transform.parent.gameObject.activeSelf == false)
            {
                _enemies.RemoveAt(i);
                continue;
            }

            if (_isAttackTower)
            {
                AttackController.Instance.TowerAttack(targetHitbox.transform.position, dmg, _attackTypes, _tower.name);
                break;
            }
        }
        if (_isAttackTower == false)
            SoundController.Instance.PlayAudio(_tower.name.ToUpper(), Player.Instance.transform);

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
            for (int i = 0; i < _enemies.Count; i++)
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
