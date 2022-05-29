using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumData;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get { return instance; } }
    private static UIController instance;

    [Header("Base Object")]
    [SerializeField] private GameObject canvas;

    [Header("Data")]
    [SerializeField] private Slider ammoSlider;
    [SerializeField] private TextMeshProUGUI ammoText;
    private int maxAmmo;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;
    private int maxHp;

    [SerializeField] private TextMeshProUGUI moneyText;

    [Space]
    [SerializeField] private TextMeshProUGUI baseText;

    [Header("Shop")]
    [SerializeField] private GameObject shopObject;

    [Space]
    [SerializeField] private Transform supporterGrid;
    [SerializeField] private Transform weaponsGrid;
    [SerializeField] private Transform towersGrid;
    
    [Space]
    [SerializeField] private GameObject supporterPrefab;
    [SerializeField] private GameObject itemPrefab;
    private ItemType selectedItemType;
    private int selectedItem;

    [SerializeField] private UpgradeView upgradeView;
    [SerializeField] private CustomButton shopDoneButton;


    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(canvas);
    }

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        selectedItemType = ItemType.WEAPON;
        selectedItem = 0;
        UpdateUpgradeView();

        for (int i = 0; i < SupporterManager.Types.Count; i++)
        {
            GameObject go = Instantiate(supporterPrefab);
            ItemButton si = go.GetComponent<ItemButton>();

            si.SetButton(ButtonType.SUPPORTER, i);
            si.SetIcon(SupporterManager.Types[i]);

            go.transform.SetParent(supporterGrid);
        }

        for (int i = 0; i < ItemManager.Weapons.Count; i++)
        {
            GameObject go = Instantiate(itemPrefab);
            ItemButton si = go.GetComponent<ItemButton>();

            si.SetButton(ButtonType.WEAPON, i);
            si.SetIcon(ItemManager.Weapons[i].name);

            go.transform.SetParent(weaponsGrid);
        }

        for (int i = 0; i < ItemManager.Towers.Count; i++)
        {
            string name = ItemManager.TowerNames[i];
            GameObject go = Instantiate(itemPrefab);
            ItemButton si = go.GetComponent<ItemButton>();

            si.SetButton(ButtonType.TOWER, i);
            si.SetIcon(ItemManager.Towers[name].name);

            go.transform.SetParent(towersGrid);
        }

        shopDoneButton.SetButton(ButtonType.ROUNDSTART);
        OpenShop(true);
    }

    #region Upside Panel

    public void SetAmmo(int cur, int max)
    {
        if (rlcoroutine != null)
        {
            StopCoroutine(rlcoroutine);
        }

        maxAmmo = max;
        ammoSlider.maxValue = max;


        UpdateAmmo(cur);
    }

    public void UpdateAmmo(int cur)
    {
        ammoText.text = cur + "/" + maxAmmo;
        ammoSlider.value = cur;
    }

    IEnumerator rlcoroutine = null;

    public void Reload(float time)
    {
        if (rlcoroutine != null)
        {
            StopCoroutine(rlcoroutine);
        }

        rlcoroutine = Reloading(time);
        StartCoroutine(rlcoroutine);
    }

    IEnumerator Reloading(float time)
    {
        float t = 0;
        while (time > t)
        {
            t += Time.deltaTime;
            if (t > time) t = time;
            ammoSlider.value = t / time * maxAmmo;
            yield return null;
        }
    }
    public void UpdateMoney(int money)
    {
        moneyText.text = "$" + money;
    }

    public void SetHP(int cur, int max)
    {
        maxHp = max;
        hpSlider.maxValue = max;

        UpdateHP(cur);
    }
    public void UpdateHP(int cur)
    {
        hpText.text = cur + "/" + maxHp;
        hpSlider.value = cur;
    }

    public void SetSupporter(int shield, int gun, int repair)
    {
        baseText.text = shield + "%" + "\n" + gun.ToString() + "\n" + repair.ToString();
    }
    #endregion

    public void OpenShop(bool b)
    {
        shopObject.SetActive(b);
        SwitchTap(true);
    }

    public void SwitchTap(bool isWeapon)
    {
        weaponsGrid.gameObject.SetActive(isWeapon);
        towersGrid.gameObject.SetActive(!isWeapon);
    }

    public void PressButton(ButtonType type, int index)
    {
        switch(type)
        {
            case ButtonType.SUPPORTER:
                string name = SupporterManager.Types[index];
                Player.Instance.AddSupporter(name);
                break;

            case ButtonType.TOWER:
            case ButtonType.WEAPON:
                if (type == ButtonType.WEAPON)
                    selectedItemType = ItemType.WEAPON;
                else
                    selectedItemType = ItemType.TOWER;
                selectedItem = index;
                UpdateUpgradeView();
                break;

            case ButtonType.BUY:
                if (type == ButtonType.WEAPON)
                    Player.Instance.BuyWeapon(selectedItem);
                else
                    Player.Instance.ReadyToBuyTower(selectedItem);

                UpdateUpgradeView();
                break;

            case ButtonType.ROUNDSTART:
                OpenShop(false);
                RoundController.Instance.NextRound();
                break;
        }
    }

    public void Upgrade(UpgradeDataType type)
    {
        Player.Instance.Upgrade(selectedItemType, selectedItem, type);
    }

    private void UpdateUpgradeView()
    {
        Item item;
        // 업그레이드 관련 창 표기
        if (selectedItemType == ItemType.WEAPON)
            item = WeaponController.Instance.GetWeaponData(selectedItem);
        else
        {
            string name = ItemManager.TowerNames[selectedItem];
            item = ItemManager.Towers[name];
        }

        upgradeView.SetUpgradeView(item);
    }
}