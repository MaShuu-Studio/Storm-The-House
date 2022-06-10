using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class AttackController : MonoBehaviour
{
    public static string pointName = "ATTACKPOINT";

    public static AttackController Instance { get { return instance; } }
    private static AttackController instance;

    private Camera cam;

    private Queue<KeyValuePair<EnemyObject, GameObject>> enemyQueue;

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
        enemyQueue = new Queue<KeyValuePair<EnemyObject, GameObject>>();
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

        StartCoroutine(Attack(pos, dmg, range, accurancy, remainTime, remain));
    }

    public void TowerAttack(Vector3 pos, float damge, List<UpgradeDataType> attackTypes)
    {
        float dmg = damge;
        float accurancy = 100;
        float remainTime = 0.1f;
        float range = 0.5f;
        bool remain = false;

        StartCoroutine(Attack(pos, dmg, range, accurancy, remainTime, remain, attackTypes));

    }

    // ��������� �۵���ų �ڷ�ƾ
    private IEnumerator Attack(Vector3 pos, float dmg, float range, float accurancy, float time, bool remain = false, List<UpgradeDataType> attackTypes = null)
    {
        GameObject point = ObjectPool.GetObject<GameObject>(pointName);

        point.name = "";
        if (attackTypes != null)
        {
            foreach (UpgradeDataType type in attackTypes)
            {
                if (type == UpgradeDataType.SLOW)
                {
                    point.name += "S";
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
            pos.x + Random.Range(-1 / (accurancy * 5), 1 / (accurancy * 5)),
            pos.y + Random.Range(-1 / (accurancy * 5), 1 / (accurancy * 5)),
            pos.z + Random.Range(-1 / (accurancy * 5), 1 / (accurancy * 5)));
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
        enemyQueue.Enqueue(new KeyValuePair<EnemyObject, GameObject>(enemy, point));

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
            KeyValuePair<EnemyObject, GameObject> kv = enemyQueue.Dequeue();
            EnemyObject enemy = kv.Key;
            GameObject point = kv.Value;

            if (point.name != pointName)
            {
                bool remain = false;
                bool slow = false;
                bool down = false;
                char c = 'a';

                int j = 0;

                for(; j < point.name.Length && c > '9'; j++)
                {
                    c = point.name[j];

                    if (c == 'S') slow = true;
                    else if (c == 'D') down = true;
                    else if (c == 'T')
                    {
                        remain = true;
                        break;
                    }
                    else if (c == 'F') break;
                }


                float dmg;

                if (slow) enemy.Slow();
                if (down) enemy.Down();

                if (remain == false) ReturnPoint(point);

                if (float.TryParse(point.name.Substring(j), out dmg) == false) continue;
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
