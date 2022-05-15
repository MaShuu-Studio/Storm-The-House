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
    [SerializeField] private Transform supporterGrid;
    [SerializeField] private Transform weaponsGrid;
    [SerializeField] private Transform towersGrid;
    
    [Space]
    [SerializeField] private GameObject supporterPrefab;
    [SerializeField] private GameObject itemPrefab;
    private ButtonType selectedType;
    private int selectedItem;

    [SerializeField] private UpgradeView upgradeView;


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

    private void Initialize()
    {
        selectedType = ButtonType.WEAPON;
        selectedItem = 0;
        UpdateUpgradeView();

        for (int i = 0; i < SupporterManager.Types.Count; i++)
        {
            GameObject go = Instantiate(supporterPrefab);
            CustomButton si = go.GetComponent<CustomButton>();

            si.SetButton(ButtonType.SUPPORTER, i);
            si.SetIcon(SupporterManager.Types[i]);

            go.transform.SetParent(supporterGrid);
        }
        
        for (int i = 0; i < WeaponManager.Weapons.Count; i++)
        {
            GameObject go = Instantiate(itemPrefab);
            CustomButton si = go.GetComponent<CustomButton>();

            si.SetButton(ButtonType.WEAPON, i);
            si.SetIcon(WeaponManager.Weapons[i].name);

            go.transform.SetParent(weaponsGrid);
        }
    }

    public void PressButton(ButtonType type, int index)
    {
        switch(type)
        {
            case ButtonType.SUPPORTER:
                string name = SupporterManager.Types[index];
                Player.Instance.AddSupporter(name);
                break;
            case ButtonType.WEAPON:
                selectedItem = index;
                UpdateUpgradeView();
                break;
            case ButtonType.TOWER:
                break;
            case ButtonType.BUY:
                Player.Instance.BuyWeapon(selectedItem);
                UpdateUpgradeView();
                break;
        }
    }

    public void Upgrade(WeaponDataType type)
    {
        // 타워인지 무기인지에 따라 다른 설정
        Player.Instance.Upgrade(selectedItem, type);
    }

    private void UpdateUpgradeView()
    {
        // 업그레이드 관련 창 표기
        Weapon weapon = WeaponController.Instance.GetWeaponData(selectedItem);
        upgradeView.SetUpgradeView(weapon);
    }
}