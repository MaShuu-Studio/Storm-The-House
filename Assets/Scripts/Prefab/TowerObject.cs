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
    [SerializeField] private Laser laser;

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
        _attackArea.Initialize(this, _tower.GetValue(UpgradeDataType.DISTANCE));
    }

    public void Upgrade()
    {
        _attackArea.UpdateRange(_tower.GetValue(UpgradeDataType.DISTANCE));
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
    private IEnumerator Missile(Vector3 startpos, Transform endtrans, float delay)
    {
        float waitTime = 0;
        string effect = "MISSILE TRAIL";
        startpos.y += 2;
        GameObject effectObj = ObjectPool.GetObject<Effect>(effect);
        while (delay > waitTime)
        {
            Vector3 endpos = endtrans.position;
            Vector3 dir = (endpos - startpos);

            float amount = Mathf.PI / delay * waitTime;
            effectObj.transform.SetParent(null);
            Vector3 pos = startpos + dir * waitTime / delay; // endpos에 도착할 수 있도록 (sin(pi/2) = 1)
            pos.y += 5 * Mathf.Sin(amount); // 위로 갔다가 떨어지도록 (Sin(pi) = 0)
            effectObj.transform.position = pos;
            waitTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator Attack(float term, float delay, Transform enemy, float dmg)
    {
        while (term > 0)
        {
            term -= Time.deltaTime;
            yield return null;
        }

        SoundController.Instance.PlayAudio(_tower.name.ToUpper(), Player.Instance.transform);

        Vector3 startpos = transform.position;
        startpos.y += 2;
        if (_tower.effect.ToUpper() == "MISSILE") StartCoroutine(Missile(startpos, enemy.transform, delay - 0.3f));

        while (delay > 0)
        {
            delay -= Time.deltaTime;
            yield return null;
        }

        if (_tower.effect.ToUpper() == "LASER") laser.Setposition(startpos, enemy.position);
        AttackController.Instance.TowerAttack(startpos, enemy.position, dmg, _tower);
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
