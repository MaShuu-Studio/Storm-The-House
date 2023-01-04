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

    private Queue<Tuple<EnemyObject, GameObject, string>> enemyQueue;

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
        enemyQueue = new Queue<Tuple<EnemyObject, GameObject, string>>();
    }

    // 마우스 상호작용으로 작동하는 형태
    public void Click(bool isFire)
    {
        bool canClick = (!isFire || (isFire && WeaponController.Instance.Fire()));

        if (canClick)
        {
            Vector3 pos = ClickPos();
            Item weapon = WeaponController.Instance.CurrentWeapon;
            float accurancy = weapon.GetValue(UpgradeDataType.ACCURANCY);
            float dmg = weapon.GetValue(UpgradeDataType.DAMAGE);
            float remainTime = weapon.GetValue(UpgradeDataType.REMAINTIME);
            float range = weapon.GetValue(UpgradeDataType.RANGE);
            int missiles = (int)(Mathf.Round(weapon.GetValue(UpgradeDataType.MISSILES)));
            float delay = weapon.GetValue(UpgradeDataType.DELAY);
            bool remain = weapon.isRemain;

            if (isFire)
            {
                SoundController.Instance.PlayAudio(weapon.name.ToUpper());
                StartCoroutine(Attack(weapon.name.ToUpper(), pos, dmg, range, accurancy, missiles, delay, remainTime, remain, weapon.data));
            }
            else // 일반적인 클릭 상황
            {
                StartCoroutine(Attack("", pos, 0, 1, 1000, 1, 0, 0.1f, false));
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
        StartCoroutine(Attack(supportName, pos, dmg, range, accurancy, 1, 0f, remainTime, remain));
    }

    public void TowerAttack(Vector3 pos, float damge, Item tower)
    {
        float dmg = damge;
        float accurancy = 100;
        float remainTime = 0.1f;
        float range = 0.5f;
        bool remain = false;

        StartCoroutine(Attack(tower.name, pos, dmg, range, accurancy, 1, 0f, remainTime, remain, tower.data));
    }

    // 결과적으로 작동시킬 코루틴
    private IEnumerator Attack(string attackName, Vector3 pos, float dmg, float range, float accurancy,
        int missiles, float delay, float time, bool remain = false, Dictionary<UpgradeDataType, UpgradeData> data = null)
    {
        float waitTime = 0;
        while (delay > waitTime)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }

        List<GameObject> points = new List<GameObject>();

        for (int i = 0; i < missiles; i++)
        {
            GameObject point = ObjectPool.GetObject<GameObject>(pointName);

            point.name = "";
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

            point.name += (remain ? "T" : "F");
            point.name += dmg.ToString();
            point.transform.SetParent(null);
            point.transform.position = new Vector3(
                pos.x + UnityEngine.Random.Range(-1 / (accurancy * 5), 1 / (accurancy * 5)),
                pos.y + UnityEngine.Random.Range(-1 / (accurancy * 5), 1 / (accurancy * 5)),
                pos.z + UnityEngine.Random.Range(-1 / (accurancy * 5), 1 / (accurancy * 5)));
            point.transform.localScale = new Vector3(range, range, range);
            points.Add(point);
        }

        // 폭발음
        SoundController.Instance.PlayAudio((attackName + " BOOM").ToUpper(), points[0].transform);

        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        foreach (GameObject point in points)
        {
            ReturnPoint(point);
        }
    }

    private void ReturnPoint(GameObject obj)
    {
        obj.name = pointName;
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
            pos = raycastHit.point;

        // 더 안쪽으로 파고들도록 약간의 보정 작업
        pos += dir / 5;

        return pos;
    }

    public void EnemyDamaged(EnemyObject enemy, GameObject point)
    {
        enemyQueue.Enqueue(new Tuple<EnemyObject, GameObject, string>(enemy, point, point.name));

        if (flushCoroutine == null)
        {
            flushCoroutine = Flush();
            StartCoroutine(Flush());
        }
    }

    IEnumerator flushCoroutine;

    private IEnumerator Flush()
    {
        int count = enemyQueue.Count;
        for (int i = 0; i < count; i++)
        {
            Tuple<EnemyObject, GameObject, string> kv = enemyQueue.Dequeue();
            EnemyObject enemy = kv.Item1;
            GameObject point = kv.Item2;
            string pn = kv.Item3;

            if (pn != pointName)
            {
                bool remain = false;
                float slowAmount = 0;
                bool down = false;
                bool burn = false;
                char c = 'a';

                int j = 0;
                for (; j < pn.Length && c > '9'; j++)
                {
                    c = pn[j];

                    if (c == 'S')
                    {
                        j++;
                        float.TryParse(pn.Substring(j, j + 3), out slowAmount);
                        j += 3;
                    }
                    else if (c == 'D') down = true;
                    else if (c == 'B') burn = true;
                    else if (c == 'T')
                    {
                        remain = true;
                        j++;
                        break;
                    }
                    else if (c == 'F')
                    {
                        j++;
                        break;
                    }
                }

                float dmg = 0;

                if (slowAmount != 0) enemy.Slow(slowAmount);
                if (down) enemy.Down();

                if (remain == false) ReturnPoint(point);

                if (float.TryParse(pn.Substring(j), out dmg) == false && dmg != 0) continue;
                else
                {
                    if (burn) enemy.Burn(dmg);
                    else enemy.Damage(dmg);
                }
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
