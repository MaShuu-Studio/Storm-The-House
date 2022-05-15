using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

[RequireComponent(typeof(BoxCollider))]
public class Player : MonoBehaviour
{
    public static Player Instance { get { return instance; } }
    private static Player instance;

    private BoxCollider area;
    private Dictionary<string, int> supporter;
    private Dictionary<string, AttackPoint> attackPoints;
    private Queue<GameObject> enemys;
    private Dictionary<string, IEnumerator> coroutines;

    private int hp;
    private int maxHp;
    private float shield;

    private int money;

    public int Money { get { return money; } }

    #region Awake, Start, Update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        area = GetComponent<BoxCollider>();
        if (area == null)
            area = gameObject.AddComponent<BoxCollider>();

        area.isTrigger = true;
        area.center = new Vector3(-4, 0, 0);
        area.size = new Vector3(8, 2, 2);

        enemys = new Queue<GameObject>();
    }

    void Start()
    {
        money = 100;
        UIController.Instance.UpdateMoney(money);

        supporter = new Dictionary<string, int>();
        coroutines = new Dictionary<string, IEnumerator>();
        attackPoints = new Dictionary<string, AttackPoint>();

        foreach (string name in SupporterManager.Types)
        {
            supporter.Add(name, 0);
            coroutines.Add(name, null);
            if (SupporterManager.SupportType(name) == SupporterType.ATTACK)
            {
                attackPoints.Add(name, AttackPointManager.Instance.MakeAttackPoint());
                attackPoints[name].SetDamage(SupporterManager.Damage(name));
            }
        }

        // 테스트용 임시 코드
        hp = maxHp = 100;
        shield = 0;
        UIController.Instance.SetHP(hp, maxHp);
        UIController.Instance.SetSupporter((int)shield, supporter["GUNMAN"], supporter["REPAIRMAN"]);
    }

    void Update()
    {
        for (int i = 0; i < SupporterManager.Types.Count; i++)
        {
            string type = SupporterManager.Types[i];
            if (coroutines[type] == null && supporter[type] > 0)
                ActiveSupporter(type);
        }
    }
    #endregion

    #region Supporter
    public void AddSupporter(string type)
    {
        if (!supporter.ContainsKey(type)) return;

        supporter[type]++;

        UIController.Instance.SetSupporter((int)shield, supporter["GUNMAN"], supporter["REPAIRMAN"]);
    }

    private void ActiveSupporter(string type)
    {
        if (!supporter.ContainsKey(type)) return;

        Debug.Log("[SYSTEM] ACTIVE SUPPROTER " + type);
        coroutines[type] = Active(type);
        StartCoroutine(coroutines[type]);
    }

    IEnumerator Active(string type)
    {
        // 실시간 동기화를 위해 코루틴 중간에 서포터 수 체크
        float time = 0;
        float waitingTime;
        do
        {
            waitingTime = SupporterManager.Delay(type, supporter[type]);
            time += Time.deltaTime;
            yield return null;
        } while (time < waitingTime);

        GameObject target;
        Vector3 pos = Vector3.zero;

        while (enemys.Count > 0)
        {
            target = enemys.Peek();

            if (target == null || target.activeSelf == false) enemys.Dequeue();
            else
            {
                pos = target.transform.position;
                attackPoints[type].Attack(0.1f, pos);
                break;
            }
        }

        if (enemys.Count > 0)
            ActiveSupporter(type);
        else
            coroutines[type] = null;
    }
    #endregion

    public void Damaged(int dmg)
    {
        hp -= dmg;
        UIController.Instance.UpdateHP(hp);
    }

    public void Upgrade(int index, WeaponDataType type)
    {
        WeaponController.Instance.Upgrade(index, type, ref money);
        UIController.Instance.UpdateMoney(money);
    }

    public void BuyWeapon(int index)
    {
        WeaponController.Instance.BuyWeapon(index, ref money);
        UIController.Instance.UpdateMoney(money);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            enemys.Enqueue(other.gameObject);
        }
    }

    GUIStyle style = new GUIStyle();
    private void OnGUI()
    {
        style.fontSize = 0;

        if (GUI.Button(new Rect(1810, 310 + 120, 100, 50), "ADD MONEY 100"))
        {
            money += 1000;
            UIController.Instance.UpdateMoney(money);
        }
    }
}
