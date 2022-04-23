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


    private Dictionary<WeaponTimerType, IEnumerator> _timer;
    public Dictionary<WeaponTimerType, bool> _canUse { get; private set; }
    public int Ammo { get { return _ammo[_curWeapon]; } }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);


        // 후에 데이터 테이블을 통해 외부에서 불러올 예정
        _weapons = new List<Weapon>()
        {
            new Weapon(7, 1.4f, 1.5f, 1, 1),
            new Weapon(20, 2, 1.5f, 2, 1)
        };

        _usingWeapon = new int[2] { 0, -1 };
        _ammo = new int[2]
        {
            _weapons[0].Ammo,
            0,
        };

        _curWeapon = 0;

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
    }

    public void Initialize()
    {
        _canUse[WeaponTimerType.FIRE] = true;
        if (_ammo[_curWeapon] <= 0)
        {
            _canUse[WeaponTimerType.RELOAD] = true;
        }

        StopCoroutine(_timer[WeaponTimerType.FIRE]);
        StopCoroutine(_timer[WeaponTimerType.RELOAD]);

        _timer[WeaponTimerType.FIRE] = null;
        _timer[WeaponTimerType.RELOAD] = null;
    }
    public void SwitchWeapon()
    {
        int nextIndex = (_curWeapon + 1) % 2;
        // 발사 불가능 상태에서는 무기 교체 불가능.
        if (_canUse[WeaponTimerType.FIRE] == false || _usingWeapon[nextIndex] == -1) return;

        _curWeapon = nextIndex;
        Initialize();
    }

    public bool Fire()
    {
        if (_ammo[_curWeapon] > 0 && _canUse[WeaponTimerType.FIRE])
        {
            _canUse[WeaponTimerType.FIRE] = false;
            _ammo[_curWeapon]--;
            float fireTime = _weapons[_usingWeapon[_curWeapon]].FireTime;
            _timer[WeaponTimerType.FIRE] = Timer(WeaponTimerType.FIRE, fireTime);
            StartCoroutine(_timer[WeaponTimerType.FIRE]);
            return true;
        }
        else
        {
            // 총알이 부족하면 자동으로 장전
            Reload();
            return false;
        }
    }

    public void Reload()
    {
        if (_canUse[WeaponTimerType.RELOAD] == false) return;

        _ammo[_curWeapon] = 0;
        _canUse[WeaponTimerType.FIRE] = false;
        _canUse[WeaponTimerType.RELOAD] = false;

        float reloadTime = _weapons[_usingWeapon[_curWeapon]].ReloadTime;
        _timer[WeaponTimerType.RELOAD] = Timer(WeaponTimerType.RELOAD, reloadTime);
        StartCoroutine(_timer[WeaponTimerType.RELOAD]);
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
            _ammo[_curWeapon] = _weapons[_curWeapon].Ammo;
            _canUse[WeaponTimerType.FIRE] = true;
        }
        _canUse[type] = true;
    }
}
