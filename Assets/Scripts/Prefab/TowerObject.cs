using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class TowerObject : MonoBehaviour
{
    [SerializeField] private TowerAttackArea _attackArea;
    [SerializeField] private TowerHitBox _hitbox;
    private Item _tower;
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
        float dmg = 0;

        if (_isAttackTower) dmg = _tower.GetValue(UpgradeDataType.DAMAGE);

        int missiles = (_tower.data.ContainsKey(UpgradeDataType.MISSILES)) ? (int)(Mathf.Round(_tower.data[UpgradeDataType.MISSILES].currentValue)) : 1;
        float term = (missiles == 1) ? 0 : 0.15f;
        float delay = (_tower.data.ContainsKey(UpgradeDataType.DELAY)) ? _tower.data[UpgradeDataType.DELAY].currentValue : 0;

        for (int m = 0; m < missiles; m++)
        // 동시 발사 탄환이 아닌 연달아 발사되는 미사일(미사일터렛)
        // 정확한 적의 위치에 공격해야 하기 떄문에 Active 내부에서 딜레이를 가짐.
        {
            if (_isAttackTower == false)
            {
                // 모든 적에게 공격
                for (int i = 0; i < _enemies.Count; i++)
                {
                    GameObject targetHitbox = _enemies[i];
                    if (targetHitbox == null || targetHitbox.transform.parent.gameObject.activeSelf == false)
                    {
                        _enemies.RemoveAt(i);
                        continue;
                    }
                }
            }
            else
            {
                // 랜덤한 적에게 공격
                while (_enemies.Count > 0)
                {
                    int index = Random.Range(0, _enemies.Count);
                    Debug.Log(index);
                    GameObject targetHitbox = _enemies[index];
                    if (targetHitbox == null || targetHitbox.transform.parent.gameObject.activeSelf == false)
                    {
                        _enemies.RemoveAt(index);
                        continue;
                    }

                    StartCoroutine(Attack(term * m, delay, targetHitbox.transform, dmg));
                    break;
                }
            }
        }

        yield return new WaitForSeconds(ItemManager.FireRate(_tower.GetValue(UpgradeDataType.FIRERATE)));

        coroutine = null;
        if (_enemies.Count > 0) ActiveTower();
    }

    private IEnumerator Attack(float term, float delay, Transform enemy, float dmg)
    {
        while (term > 0)
        {
            term -= Time.deltaTime;
            yield return null;
        }

        SoundController.Instance.PlayAudio(_tower.name.ToUpper(), Player.Instance.transform);

        while (delay > 0)
        {
            delay -= Time.deltaTime;
            yield return null;
        }

        AttackController.Instance.TowerAttack(enemy.position, dmg, _tower);
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
