using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class Player : MonoBehaviour
{
    public static Player Instance { get { return instance; } }
    private static Player instance;

    private BoxCollider area;
    private Dictionary<string, int> supporter;
    private Queue<GameObject> enemys;
    private Dictionary<string, IEnumerator> coroutines;

    private float hp;
    private float maxHp;
    private float shield;

    private int money;

    private bool infiniteMoney;
    private bool invincible;

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
        area.center = new Vector3(-20, 20, 0);
        area.size = new Vector3(30, 40, 25);

        enemys = new Queue<GameObject>();
    }

    void Start()
    {
        supporter = new Dictionary<string, int>();
        coroutines = new Dictionary<string, IEnumerator>();
    }

    void Update()
    {
        if (GameController.Instance.ProgressGame == false) return;

        if (hp <= 0)
        {
            GameController.Instance.GameOver();
        }

        if (infiniteMoney)
        {
            money = 9999999;
            UIController.Instance.UpdateMoney(money);
        }
    }

    public void NewGame(int mo, int wallLevel, bool inv, bool inf)
    {
        supporter.Clear();
        coroutines.Clear();

        foreach (string name in SupporterManager.Upgradable)
        {
            supporter.Add(name, 0);
        }

        foreach (string name in SupporterManager.Units)
        {
            supporter.Add(name, 0);
            coroutines.Add(name, null);
        }

        hp = maxHp = 100;
        for (int i = 1; i < wallLevel; i++)
        {
            Upgrade("UPGRADE WALL");
        }

        shield = 0;
        money = mo;

        invincible = inv;
        infiniteMoney = inf;

        UIController.Instance.UpdateMoney(money);
        UIController.Instance.SetHP(hp, maxHp);
        UpdateSupporterInfo();
    }
    #endregion

    #region Supporter
    private void UpdateSupporterInfo()
    {
        UIController.Instance.SetSupporter(shield, supporter["GUNMAN"], supporter["REPAIRMAN"]);
    }

    public void AddSupporter(string supporterName)
    {
        if (money < SupporterCost(supporterName)) return;

        SupporterType type = SupporterManager.Type(supporterName);
        SupporterAbilityType ablity = SupporterManager.Ability(supporterName);
        if (type == SupporterType.UNIT)
        {
            money -= SupporterCost(supporterName);
            UIController.Instance.UpdateMoney(money);

            supporter[supporterName]++;
            if (coroutines[supporterName] == null) ActiveSupporter(supporterName, ablity);
            UpdateSupporterInfo();
        }
        else if (type == SupporterType.HOUSE)
        {
            if (ablity == SupporterAbilityType.HEAL && hp < maxHp)
            {
                money -= SupporterCost(supporterName);
                UIController.Instance.UpdateMoney(money);

                Heal(SupporterManager.Value(supporterName));
            }
            else if (ablity == SupporterAbilityType.UPGRADE && Upgradable(supporterName))
            {
                money -= SupporterCost(supporterName);
                UIController.Instance.UpdateMoney(money);
                Upgrade(supporterName);
            }
        }

    }

    public int SupporterCost(string supporterName)
    {
        int amount = (supporter.ContainsKey(supporterName)) ? supporter[supporterName] : 0;
        int cost = SupporterManager.Cost(supporterName, amount);

        return cost;
    }

    public bool Upgradable(string supporterName)
    {
        bool b = true;
        if (supporter.ContainsKey(supporterName) && SupporterManager.MaxLevel(supporterName) > 0)
        {
            b = (supporter[supporterName] < SupporterManager.MaxLevel(supporterName));
        }
        return b;
    }

    private void Upgrade(string supporterName)
    {
        supporter[supporterName]++;

        int value = SupporterManager.Value(supporterName);
        // 밑의 코드를 변경하는 것으로 각종 업그레이드로 활용
        hp += value;
        maxHp += value;
        UIController.Instance.SetHP(hp, maxHp);
    }

    #region Unit

    private void ActiveSupporter(string supporterName, SupporterAbilityType ability)
    {
        if (!supporter.ContainsKey(supporterName) || hp <= 0) return;

        Debug.Log("[SYSTEM] ACTIVE SUPPROTER " + supporterName);
        coroutines[supporterName] = Active(supporterName, ability);
        StartCoroutine(coroutines[supporterName]);
    }

    IEnumerator Active(string supporterName, SupporterAbilityType ability)
    {
        // 실시간 동기화를 위해 코루틴 중간에 서포터 수 체크
        float time = 0;
        float waitingTime;
        do
        {
            waitingTime = SupporterManager.Delay(supporterName, supporter[supporterName]);
            time += Time.deltaTime;
            yield return null;
        } while (time < waitingTime);

        int value = SupporterManager.Value(supporterName);
        if (ability == SupporterAbilityType.ATTACK)
        {
            GameObject target;
            Vector3 pos = Vector3.zero;

            while (enemys.Count > 0)
            {
                target = enemys.Peek();

                if (target == null || target.transform.parent.gameObject.activeSelf == false) enemys.Dequeue();
                else
                {
                    pos = target.transform.position;

                    SoundController.Instance.PlayAudio("SUPPORTER " + supporterName);
                    AttackController.Instance.SupporterAttack(pos, value);
                    break;
                }
            }

            while (enemys.Count == 0) yield return null;

            if (enemys.Count > 0)
                ActiveSupporter(supporterName, ability);
        }
        else if (ability == SupporterAbilityType.HEAL)
        {
            while (hp >= maxHp) yield return null; // 체력이 풀일때는 회복 X
            Heal(value);
            ActiveSupporter(supporterName, ability);
        }
    }
    #endregion
    #endregion

    public void Heal(int value)
    {
        hp += value;
        if (hp > maxHp) hp = maxHp;
        UIController.Instance.UpdateHP(hp);
    }

    public void Damaged(int value)
    {
        if (invincible) return;
        hp -= value * (1 - (shield / 100));
        UIController.Instance.UpdateHP(hp);
    }

    public void Upgrade(ItemType itemType, int index, UpgradeDataType type)
    {
        if (itemType == ItemType.WEAPON)
            WeaponController.Instance.Upgrade(index, type, ref money);
        else
            TowerController.Instance.Upgrade(index, type, ref money);

        UIController.Instance.UpdateMoney(money);
    }

    public void BuyWeapon(int index)
    {
        WeaponController.Instance.BuyWeapon(index, ref money);
        UIController.Instance.UpdateMoney(money);
    }

    public bool ReadyBuyTower { get; private set; } = false;
    private string towerName = "";
    private int towerCost;

    public void StartRound()
    {
        ReadyBuyTower = false;
    }

    public void ReadyToBuyTower(int index)
    {
        string name = ItemManager.TowerNames[index];
        Item tower = ItemManager.Towers[name];
        towerName = tower.name;
        towerCost = tower.cost;

        if (money >= towerCost)
            ReadyBuyTower = true;
    }

    public void BuyTower(int index)
    {
        ReadyBuyTower = false;
        bool buy = TowerController.Instance.AddTower(index, towerName);

        if (buy == false) return;

        money -= towerCost;
        UIController.Instance.UpdateMoney(money);
    }

    public void UpdateShield(float s)
    {
        shield = s;
        UpdateSupporterInfo();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            enemys.Enqueue(other.gameObject);
        }
    }
}
