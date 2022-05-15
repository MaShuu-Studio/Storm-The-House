using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class WeaponController : MonoBehaviour
{
    public static WeaponController Instance { get { return instance; } }
    private static WeaponController instance;

    private List<Weapon> _weapons;
    private int[] _usingWeapon;
    private int[] _ammo;
    private int _curWeapon;
    private int CurrentUsingWeapon { get { return _usingWeapon[_curWeapon]; } }

    private Dictionary<WeaponTimerType, IEnumerator> _timer;
    public Dictionary<WeaponTimerType, bool> _canUse { get; private set; }
    public int Ammo { get { return _ammo[_curWeapon]; } }
    public float Damage { get { return _weapons[CurrentUsingWeapon].GetValue(WeaponDataType.DAMAGE); } }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (_weapons != null) _weapons.Clear();
        _weapons = new List<Weapon>(WeaponManager.Weapons);

        _usingWeapon = new int[2] { 0, -1 };
        _ammo = new int[2]
        {
            (int)_weapons[0].GetValue(WeaponDataType.AMMO),
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

        UIController.Instance.SetAmmo(_ammo[_curWeapon], (int)_weapons[CurrentUsingWeapon].GetValue(WeaponDataType.AMMO));
        AttackController.Instance.SetDamage();
    }

    public bool Fire()
    {
        if (_canUse[WeaponTimerType.FIRE] == false) return false;

        if (_ammo[_curWeapon] > 0)
        {
            _canUse[WeaponTimerType.FIRE] = false;
            _ammo[_curWeapon]--;
            float fireTime = _weapons[CurrentUsingWeapon].GetValue(WeaponDataType.FIRERATE);
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

        float reloadTime = _weapons[CurrentUsingWeapon].GetValue(WeaponDataType.RELOAD);
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
            _ammo[_curWeapon] = (int)_weapons[CurrentUsingWeapon].GetValue(WeaponDataType.AMMO);
            UIController.Instance.UpdateAmmo(_ammo[_curWeapon]);
            _canUse[WeaponTimerType.FIRE] = true;
        }
        _canUse[type] = true;
    }

    public bool WeaponIsAvailable(int index)
    {
        return _weapons[index].available;
    }

    public Weapon GetWeaponData(int index)
    {
        return _weapons[index];
    }

    public void BuyWeapon(int index, ref int money)
    {
        if (_weapons[index].cost <= money)
        {
            _weapons[index].available = true;
            money -= _weapons[index].cost;
        }
    }

    public void Upgrade(int index, WeaponDataType type, ref int money)
    {
        _weapons[index].data[type].Upgrade(ref money);

        UIController.Instance.SetAmmo(_ammo[_curWeapon], (int)_weapons[CurrentUsingWeapon].GetValue(WeaponDataType.AMMO));
    }
}
