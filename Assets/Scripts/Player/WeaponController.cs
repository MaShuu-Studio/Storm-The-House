using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class WeaponController : MonoBehaviour
{
    public static WeaponController Instance { get { return instance; } }
    private static WeaponController instance;

    private List<Item> _weapons;
    private int[] _usingWeapon;
    private int[] _ammo;
    private int _curWeapon;
    private int CurrentUsingWeapon { get { return _usingWeapon[_curWeapon]; } }

    private Dictionary<WeaponTimerType, IEnumerator> _timer;
    public Dictionary<WeaponTimerType, bool> _canUse { get; private set; }
    public Item CurrentWeapon { get { return _weapons[CurrentUsingWeapon]; } }
    public int Ammo { get { return _ammo[_curWeapon]; } }
    public float Damage { get { return CurrentWeapon.GetValue(UpgradeDataType.DAMAGE); } }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        // DontDestroyOnLoad(gameObject);
    }

    public void Initialize()
    {
        if (_weapons != null) _weapons.Clear();
        _weapons = new List<Item>();

        foreach(Item weapon in ItemManager.Weapons)
        {
            _weapons.Add(new Item(weapon));
        }

        _usingWeapon = new int[2] { 0, -1 };
        _ammo = new int[2]
        {
            (int)_weapons[0].GetValue(UpgradeDataType.AMMO),
            0,
        };

        _timer = new Dictionary<WeaponTimerType, IEnumerator>()
        {
            { WeaponTimerType.FIRE, null },
            { WeaponTimerType.RELOAD, null },
        };

        _canUse = new Dictionary<WeaponTimerType, bool>()
        {
            { WeaponTimerType.FIRE, true },
            { WeaponTimerType.RELOAD, true },
        };

        _curWeapon = 1;
        SwitchWeapon();
    }

    public void RefillAmmo()
    {
        _ammo[_curWeapon] = (int)_weapons[CurrentUsingWeapon].GetValue(UpgradeDataType.AMMO);
        int nextIndex = (_curWeapon + 1) % 2;

        if (_usingWeapon[nextIndex] != -1)
        {
            int nextWeapon = _usingWeapon[nextIndex];
            _ammo[nextIndex] = (int)_weapons[nextWeapon].GetValue(UpgradeDataType.AMMO);
        }

        UIController.Instance.UpdateAmmo(_ammo[_curWeapon]);
    }

    public void SwitchWeapon()
    {
        int nextIndex = (_curWeapon + 1) % 2;
        // 발사 불가능 상태에서는 무기 교체 불가능.
        if (_canUse[WeaponTimerType.FIRE] == false || _usingWeapon[nextIndex] == -1) return;

        _curWeapon = nextIndex;

        _canUse[WeaponTimerType.FIRE] = true;
        if (_ammo[_curWeapon] <= 0)
        {
            _canUse[WeaponTimerType.RELOAD] = true;
        }

        if (_timer[WeaponTimerType.FIRE] != null) StopCoroutine(_timer[WeaponTimerType.FIRE]);
        if (_timer[WeaponTimerType.RELOAD] != null) StopCoroutine(_timer[WeaponTimerType.RELOAD]);

        _timer[WeaponTimerType.FIRE] = null;
        _timer[WeaponTimerType.RELOAD] = null;

        UpdateAmmoUI();
    }

    public bool Fire()
    {
        if (_canUse[WeaponTimerType.FIRE] == false) return false;

        if (_ammo[_curWeapon] > 0)
        {
            _canUse[WeaponTimerType.FIRE] = false;
            _ammo[_curWeapon]--;
            float fireTime = _weapons[CurrentUsingWeapon].GetValue(UpgradeDataType.FIRERATE);
            _timer[WeaponTimerType.FIRE] = Timer(WeaponTimerType.FIRE, fireTime);
            StartCoroutine(_timer[WeaponTimerType.FIRE]);
            UIController.Instance.UpdateAmmo(_ammo[_curWeapon]);

            return true;
        }
        else
        {
            // 총알이 부족하면 자동으로 장전
            Reload();
        }
        return false;
    }

    public void Reload()
    {
        if (_canUse[WeaponTimerType.RELOAD] == false) return;

        _ammo[_curWeapon] = 0;
        _canUse[WeaponTimerType.FIRE] = false;
        _canUse[WeaponTimerType.RELOAD] = false;

        float reloadTime = _weapons[CurrentUsingWeapon].GetValue(UpgradeDataType.RELOAD);
        _timer[WeaponTimerType.RELOAD] = Timer(WeaponTimerType.RELOAD, reloadTime);
        StartCoroutine(_timer[WeaponTimerType.RELOAD]);
        UIController.Instance.Reload(reloadTime);
    }

    IEnumerator Timer(WeaponTimerType type, float time)
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        if (type == WeaponTimerType.RELOAD)
        {
            _ammo[_curWeapon] = (int)_weapons[CurrentUsingWeapon].GetValue(UpgradeDataType.AMMO);
            UIController.Instance.UpdateAmmo(_ammo[_curWeapon]);
            _canUse[WeaponTimerType.FIRE] = true;
        }
        _canUse[type] = true;
    }

    public Item UsingWeapon(int index)
    {
        int i = _usingWeapon[index];
        if (i == -1) return null;
        return _weapons[i];
    }

    public int UsingWeaponIndex(int index)
    {
        return _usingWeapon[index];
    }

    public Item GetWeaponData(int index)
    {
        if (index < 0 || index >= _weapons.Count) return null;

        return _weapons[index];
    }

    public void SetWeapon(int index, int item)
    {
        int next = (index + 1) % 2;
        if (_usingWeapon[next] == item)
        {
            // 스왑을 해야하는 상황에서 다음 무기가 없으면 스왑X
            if (_usingWeapon[index] == -1) return;
            _usingWeapon[next] = _usingWeapon[index];
        }

        _usingWeapon[index] = item;

        // 웨폰 세팅을 했으니 총알 충전 및 재표기
        RefillAmmo();
        UpdateAmmoUI();
    }

    public void BuyWeapon(int index, ref int money)
    {
        if (_weapons[index].cost <= money)
        {
            _weapons[index].available = true;
            money -= _weapons[index].cost;
        }
    }

    public void Upgrade(int index, UpgradeDataType type, ref int money)
    {
        _weapons[index].data[type].Upgrade(ref money);

        RefillAmmo();
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        UIController.Instance.SetAmmo(_ammo[_curWeapon], (int)_weapons[CurrentUsingWeapon].GetValue(UpgradeDataType.AMMO));
    }
}
