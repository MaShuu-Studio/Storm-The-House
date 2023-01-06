using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;
using System;

public class AttackController : MonoBehaviour
{
    public static string pointName = "ATTACKPOINT";

    public static AttackController Instance { get { return instance; } }
    private static AttackController instance;

    private Camera cam;

    private Dictionary<GameObject, GameObject> pointEffect;
    private Queue<Tuple<EnemyObject, GameObject>> enemyQueue;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        cam = Camera.main;
        pointEffect = new Dictionary<GameObject, GameObject>();
        enemyQueue = new Queue<Tuple<EnemyObject, GameObject>>();
    }

    // 마우스 상호작용으로 작동하는 형태
    public void Click(bool isFire)
    {
        bool canClick = (!isFire || (isFire && WeaponController.Instance.Fire()));

        if (canClick)
        {
            Vector3 pos = ClickPos();
            Item weapon = WeaponController.Instance.CurrentWeapon;
            string effect = weapon.effect;
            float accurancy = weapon.GetValue(UpgradeDataType.ACCURANCY);
            float dmg = weapon.GetValue(UpgradeDataType.DAMAGE);
            float remainTime = weapon.GetValue(UpgradeDataType.REMAINTIME);
            float range = weapon.GetValue(UpgradeDataType.RANGE);
            int missiles = (int)(Mathf.Round(weapon.GetValue(UpgradeDataType.MISSILES)));
            float delay = weapon.GetValue(UpgradeDataType.DELAY);
            bool remain = weapon.isRemain;

            if (isFire)
            {
                Vector3 startpos = Player.Instance.transform.position;
                if (effect.ToUpper() == "MISSILE")
                    startpos += new Vector3(10, 7, 0);
                SoundController.Instance.PlayAudio(weapon.name.ToUpper());
                StartCoroutine(Attack(weapon.name.ToUpper(), effect.ToUpper(), startpos, pos, dmg, range, accurancy, missiles, delay, remainTime, remain, weapon.data));
            }
            else // 일반적인 클릭 상황
            {
                StartCoroutine(Attack("", "", Vector3.zero, pos, 0, 1, 1000, 1, 0, 0.1f, false));
            }
        }
    }

    public void EndClick()
    {
        WeaponController.Instance.ReleaseTrigger();
    }

    // 서포터의 공격을 통해 작동하는 형태
    public void SupporterAttack(Vector3 pos, int dmg, string supportName)
    {
        float accurancy = 1;
        float remainTime = 0.1f;
        float range = 0.5f;
        bool remain = false;

        SoundController.Instance.PlayAudio(supportName.ToUpper(), Player.Instance.transform);
        StartCoroutine(Attack(supportName, "DUST", Player.Instance.transform.position, pos, dmg, range, accurancy, 1, 0f, remainTime, remain));
    }

    public void TowerAttack(Vector3 startpos, Vector3 pos, float damge, Item tower)
    {
        float dmg = damge;
        float accurancy = 100;
        float remainTime = 0.1f;
        float range = tower.GetValue(UpgradeDataType.RANGE);
        bool remain = false;

        StartCoroutine(Attack(tower.name, tower.effect.ToUpper(), startpos, pos, dmg, range, accurancy, 1, 0f, remainTime, remain, tower.data));
    }

    public void EnemyBoom(Vector3 pos) // 자폭차
    {
        float dmg = 50;
        float accurancy = 100;
        float remainTime = 0.3f;
        float range = 8;
        bool remain = true;

        StartCoroutine(Attack("", "EXPLOSION", Vector3.zero, pos, dmg, range, accurancy, 1, 0f, remainTime, remain));
    }

    private IEnumerator Missile(Vector3 startpos, Vector3 endpos, float delay)
    {
        float waitTime = 0;
        string effect = "MISSILE TRAIL";
        GameObject effectObj = ObjectPool.GetObject<Effect>(effect);
        Vector3 dir = (endpos - startpos);
        while (delay > waitTime)
        {
            float amount = Mathf.PI / delay * waitTime;
            effectObj.transform.SetParent(null);
            Vector3 pos = startpos + dir * waitTime / delay; // endpos에 도착할 수 있도록 (sin(pi/2) = 1)
            pos.y += 5 * Mathf.Sin(amount); // 위로 갔다가 떨어지도록 (Sin(pi) = 0)
            effectObj.transform.position = pos;
            waitTime += Time.deltaTime;
            yield return null;
        }
    }

    // 결과적으로 작동시킬 코루틴
    private IEnumerator Attack(string attackName, string effect, Vector3 startpos, Vector3 pos, float dmg, float range, float accurancy,
        int missiles, float delay, float time, bool remain = false, Dictionary<UpgradeDataType, UpgradeData> data = null)
    {
        effect = effect.ToUpper();
        float waitTime = 0;
        if (effect == "MISSILE" && delay != 0) StartCoroutine(Missile(startpos, pos, delay));

        if (effect == "ELEC")
        {
            GameObject ef = ObjectPool.GetObject<Effect>(effect);
            ef.transform.SetParent(null);
            ef.transform.position = startpos;
        }

        while (delay > waitTime)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }

        List<GameObject> points = new List<GameObject>();

        for (int i = 0; i < missiles; i++)
        {
            GameObject point = ObjectPool.GetObject<GameObject>(pointName);

            point.name = (remain ? "T" : "F");
            if (data != null)
            {
                foreach (UpgradeDataType type in data.Keys)
                {
                    if (type == UpgradeDataType.SLOW)
                    {
                        point.name += "S";
                        point.name += string.Format("{0:0.00}", data[type].currentValue);
                    }

                    if (type == UpgradeDataType.DOWN)
                    {
                        point.name += "D";
                    }

                    if (type == UpgradeDataType.FLAME)
                    {
                        point.name += "B";
                    }
                }
            }
            point.name += ":";
            point.name += effect;
            point.name += dmg.ToString();
            point.transform.SetParent(null);
            point.transform.position = pos;
            float accAmount = 3f;
            if (i != 0) accAmount = 1.5f;
            point.transform.position = new Vector3(
                pos.x + UnityEngine.Random.Range(-1 / (accurancy * accAmount), 1 / (accurancy * accAmount)),
                pos.y + UnityEngine.Random.Range(-1 / (accurancy * accAmount), 1 / (accurancy * accAmount)),
                pos.z + UnityEngine.Random.Range(-1 / (accurancy * accAmount), 1 / (accurancy * accAmount)));
            point.transform.localScale = new Vector3(range, range, range);

            points.Add(point);

            if (effect != "" && effect != "LASER")
            {
                GameObject effectObj = ObjectPool.GetObject<Effect>(effect);
                effectObj.transform.SetParent(null);
                effectObj.transform.position = point.transform.position;
                pointEffect.Add(point, effectObj);
            }

        }

        // 폭발음
        SoundController.Instance.PlayAudio((attackName + " BOOM").ToUpper(), points[0].transform);

        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < points.Count; i++)
        {
            if (points[i].activeSelf) ReturnPoint(points[i]);
        }
    }

    private void ReturnPoint(GameObject obj)
    {
        obj.name = pointName;
        pointEffect.Remove(obj);
        ObjectPool.ReturnObject(pointName, obj);
    }

    private Vector3 ClickPos()
    {
        Vector3 mpos = Input.mousePosition;
        /* 
        // Perspective Camera
        mpos.z = cam.transform.position.z * -4;
        Vector3 dir = Vector3.Normalize(cam.ScreenToWorldPoint(mpos) - cam.transform.position);
        */

        // Orthographic Camera
        mpos = cam.ScreenToWorldPoint(mpos);

        Vector3 dirpos = mpos;
        dirpos.z -= 1;
        dirpos.y += Mathf.Tan(Mathf.Deg2Rad * cam.transform.rotation.eulerAngles.x);
        dirpos.x -= Mathf.Tan(Mathf.Deg2Rad * cam.transform.rotation.eulerAngles.y);
        Vector3 dir = Vector3.Normalize(mpos - dirpos);

        RaycastHit raycastHit;
        LayerMask mask = LayerMask.GetMask("Floor") | LayerMask.GetMask("Object");
        Physics.Raycast(mpos, dir, out raycastHit, Mathf.Infinity, mask);

        Vector3 pos = Vector3.zero;

        if (raycastHit.transform != null)
        {
            if (raycastHit.collider.gameObject.tag == "Object")
                pos = raycastHit.collider.transform.position;
            else
                pos = raycastHit.point;

        }

        // 더 안쪽으로 파고들도록 약간의 보정 작업
        pos += dir / 5;

        return pos;
    }

    public void EnemyDamaged(EnemyObject enemy, GameObject point)
    {
        enemyQueue.Enqueue(new Tuple<EnemyObject, GameObject>(enemy, point));

        if (flushCoroutine == null)
        {
            flushCoroutine = Flush();
            StartCoroutine(Flush());
        }
    }

    public static void ParsingPoint(GameObject point, EnemyObject enemy)
    {
        string pn = point.name;

        float slow = 0;
        bool down = false;
        bool burn = false;
        string effect = "";
        float dmg = 0;

        int j = 0;
        char c = pn[j++]; // 가장 앞자리 스킵

        for (; j < pn.Length && c > '9'; j++)
        {
            c = pn[j];

            if (c == 'S')
            {
                j++;
                float.TryParse(pn.Substring(j, j + 3), out slow);
                j += 3;
            }
            else if (c == 'D') down = true;
            else if (c == 'B') burn = true;
            else if (c == ':')
            {
                j++;
                break;
            }
        }

        // 이펙트 이름 처리
        int es = j;
        for (; j < pn.Length && c > '9'; j++)
        {
            c = pn[j];
        }
        j--;
        effect = pn.Substring(es, j - es);
        if (float.TryParse(pn.Substring(j), out dmg) == false) dmg = 0;

        if (slow != 0) enemy.Slow(slow);
        if (down) enemy.Down();
        if (burn)
        {
            enemy.Burn(dmg);
        }
        else
        {
            enemy.Damage(dmg);
            if (effect == "DUST")
                Instance.ChangeEffect(point, enemy.type, effect);
        }
    }

    public void ChangeEffect(GameObject point, string type, string effect)
    {
        GameObject newEffect = ObjectPool.GetObject<Effect>(effect + " " + type);
        newEffect.transform.SetParent(null);
        newEffect.transform.position = pointEffect[point].transform.position;
        ObjectPool.ReturnObject(pointEffect[point].name, pointEffect[point]);
        pointEffect[point] = newEffect;
    }

    IEnumerator flushCoroutine;

    private IEnumerator Flush()
    {
        int count = enemyQueue.Count;
        for (int i = 0; i < count; i++)
        {
            Tuple<EnemyObject, GameObject> kv = enemyQueue.Dequeue();
            EnemyObject enemy = kv.Item1;
            GameObject point = kv.Item2;
            string pn = point.name;

            if (pn != pointName)
            {
                ParsingPoint(point, enemy);

                ReturnPoint(point);
            }

        }
        yield return null;

        if (enemyQueue.Count > 0)
        {
            flushCoroutine = Flush();
            StartCoroutine(flushCoroutine);
        }
        else flushCoroutine = null;
    }
}
