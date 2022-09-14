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

    [Header("View")]
    [SerializeField] private GameObject mainView;
    [SerializeField] private GameObject gameView;
    [SerializeField] private GameObject gameOverView;
    [Space]

    [Header("Data")]
    [SerializeField] private Slider ammoSlider;
    [SerializeField] private TextMeshProUGUI ammoText;
    private int maxAmmo;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;
    private float maxHp;

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
    [SerializeField] private UsedWeaponItem[] usedItems;
    [SerializeField] private DraggedItem draggedItemObject;
    private string draggedItem;
    private int draggedItemIndex;
    
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

        OpenMainView();
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

        for(int i = 0; i < usedItems.Length; i++)
        {
            usedItems[i].SetIndex(i);
        }
        UpdateUsedWeapons();

        draggedItemObject.gameObject.SetActive(false);

        shopDoneButton.SetButton(ButtonType.ROUNDSTART);
    }

    public void OpenMainView()
    {
        SwitchView("MAIN");
    }

    public void StartGame()
    {
        GameController.Instance.StartGame();
        SwitchView("GAME");
        Initialize();
        OpenShop(true);
    }

    public void GameOver()
    {
        SwitchView("OVER");
    }

    public void ReturnToMain()
    {
        GameController.Instance.ReturnToMain();
        OpenMainView();
    }

    private void SwitchView(string str)
    {
        mainView.SetActive(false);
        gameView.SetActive(false);
        gameOverView.SetActive(false);

        switch (str.ToUpper())
        {
            case "MAIN": mainView.SetActive(true);
                break;
            case "GAME": gameView.SetActive(true);
                break;
            case "OVER": gameOverView.SetActive(true);
                break;
            default:
                break;
        }
    }

    #region Main View
    
    #endregion

    #region Game View

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

    public void SetHP(float cur, float max)
    {
        maxHp = max;
        hpSlider.maxValue = max;

        UpdateHP(cur);
    }
    public void UpdateHP(float cur)
    {
        hpText.text = string.Format("{0:#}", cur) + "/" + string.Format("{0:#}", maxHp);
        hpSlider.value = cur;
    }

    public void SetSupporter(float shield, int gun, int repair)
    {
        baseText.text = string.Format("{0:#}", shield) + "%" + "\n" + gun.ToString() + "\n" + repair.ToString();
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
                if (selectedItemType == ItemType.WEAPON)
                    Player.Instance.BuyWeapon(selectedItem);
                else
                    Player.Instance.ReadyToBuyTower(selectedItem);

                UpdateUpgradeView();
                break;

            case ButtonType.ROUNDSTART:
                OpenShop(false);
                RoundController.Instance.NextRound();
                break;

            case ButtonType.GAMESTART:
                GameController.Instance.StartGame();
                break;
        }
    }

    public void UpdateTowerUpgradeView()
    {
        selectedItemType = ItemType.TOWER;
        selectedItem = TowerController.Instance.SelectedTowerIndex;
        upgradeView.SetUpgradeView(TowerController.Instance.SelectedTower);
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

    public void BeginDrag(int index)
    {
        Item item = WeaponController.Instance.GetWeaponData(index);
        if (draggedItem == null || item.available == false) return;

        draggedItem = item.name;
        draggedItemIndex = index;

        draggedItemObject.SetIcon(draggedItem);
        draggedItemObject.gameObject.SetActive(true);
    }

    public void Dragging(Vector3 pos)
    {
        draggedItemObject.transform.position = pos;
    }

    public void EndDrag()
    {
        draggedItem = "";
        draggedItemIndex = 0;
        draggedItemObject.gameObject.SetActive(false);
    }

    public void DropItem(int pos)
    {
        WeaponController.Instance.SetWeapon(pos, draggedItemIndex);
        UpdateUsedWeapons();
    }

    private void UpdateUsedWeapons()
    {
        for (int i = 0; i < usedItems.Length; i++)
        {
            Item item = WeaponController.Instance.UsingWeapon(i);
            string name = "";
            if (item != null) name = item.name;
            usedItems[i].SetIcon(name);
        }
        
    }

#endregion
}