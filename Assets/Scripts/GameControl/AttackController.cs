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

    // ���콺 ��ȣ�ۿ����� �۵��ϴ� ����
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
            bool remain = weapon.isRemain;
            StartCoroutine(Attack(pos, dmg, range, accurancy, remainTime, remain));
        }
    }

    // �������� ������ ���� �۵��ϴ� ����
    public void SupporterAttack(Vector3 pos, string type)
    {
        float dmg = SupporterManager.Damage(type);
        float accurancy = 1;
        float remainTime = 0.1f;
        float range = 0.5f;
        bool remain = false;
        string weaponName = "";

        StartCoroutine(Attack(pos, dmg, range, accurancy, remainTime, remain));
    }

    public void TowerAttack(Vector3 pos, float damge, Dictionary<UpgradeDataType, float> attackTypes)
    {
        float dmg = damge;
        float accurancy = 100;
        float remainTime = 0.1f;
        float range = 0.5f;
        bool remain = false;
        string weaponName = "";

        StartCoroutine(Attack(pos, dmg, range, accurancy, remainTime, remain, attackTypes));

    }

    // ��������� �۵���ų �ڷ�ƾ
    private IEnumerator Attack(Vector3 pos, float dmg, float range, float accurancy, float time, bool remain = false, Dictionary<UpgradeDataType, float> attackTypes = null)
    {
        GameObject point = ObjectPool.GetObject<GameObject>(pointName);

        point.name = "";
        if (attackTypes != null)
        {
            foreach (UpgradeDataType type in attackTypes.Keys)
            {
                if (type == UpgradeDataType.SLOW)
                {
                    point.name += "S";
                    point.name += string.Format("{0:0.00}", attackTypes[type]);
                }

                if (type == UpgradeDataType.DOWN)
                {
                    point.name += "D";
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

        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        ReturnPoint(point);
    }

    private void ReturnPoint(GameObject obj)
    {
        obj.name = pointName;
        ObjectPool.ReturnObject(pointName, obj);
    }

    private Vector3 ClickPos()
    {
        Vector3 mpos = Input.mousePosition;
        mpos.z = cam.transform.position.z * -4;
        Vector3 dir = Vector3.Normalize(cam.ScreenToWorldPoint(mpos) - cam.transform.position);

        RaycastHit raycastHit;
        LayerMask mask = LayerMask.GetMask("Floor") | LayerMask.GetMask("Enemy");
        Physics.Raycast(cam.transform.position, dir, out raycastHit, Mathf.Infinity, mask);

        Vector3 pos = Vector3.zero;

        if (raycastHit.transform != null)
            pos = raycastHit.point;

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
                else enemy.Damage(dmg);
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
