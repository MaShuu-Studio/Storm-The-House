using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class TowerController : MonoBehaviour
{
    public static TowerController Instance { get { return instance; } }
    private static TowerController instance;

    [SerializeField] private TowerFloor towerFloorPrefab;
    private Transform[] _towerPos;
    private Item[] _towers;
    private TowerObject[] _towerObjects;

    private float zpos = 4.5f;

    private int _selectedTower;
    public int SelectedTower { get { return _selectedTower; } }

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
        Initialize(3);
    }

    public void Initialize(int towerAmount)
    {
        _selectedTower = 0;
        _towerPos = new Transform[towerAmount];
        _towers = new Item[towerAmount];
        _towerObjects = new TowerObject[towerAmount];

        for (int i = 0; i < towerAmount; i++)
        {
            GameObject floorObject = Instantiate(towerFloorPrefab.gameObject);
            float z = zpos - (zpos * 2 * i / (towerAmount - 1));

            floorObject.transform.SetParent(this.transform);
            floorObject.transform.localPosition = new Vector3(0, floorObject.transform.localScale.y / 2, z);
            _towerPos[i] = floorObject.transform;

            TowerFloor tf = floorObject.GetComponent<TowerFloor>();
            tf.SetIndex(i);
        }
        /*
        if (_weapons != null) _weapons.Clear();
        _weapons = new List<Item>(ItemManager.Weapons);

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
        SwitchWeapon();*/
    }

    public void SelectTower()
    {

    }

    public bool AddTower(int index, string name)
    {
        if (_towers[index] != null) return false;

        _towers[index] = ItemManager.Towers[name];

        GameObject obj = ObjectPool.GetObject<Item>(name);
        obj.transform.SetParent(_towerPos[index]);

        _towerObjects[index] = obj.GetComponent<TowerObject>();

        Vector3 pos = _towerPos[index].position;
        pos.y = obj.transform.lossyScale.y / 2;

        obj.transform.position = pos;
        obj.transform.SetParent(_towerPos[index]);

        return true;
    }

    public void UpdateTower(int index)
    {
        _towerObjects[index].UpdateTower(_towers[index]);
    }

    public void Upgrade(int index, UpgradeDataType type, ref int money)
    {

    }
}
