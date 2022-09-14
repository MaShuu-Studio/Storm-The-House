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

    private int _selectedTowerIndex;
    public int SelectedTowerIndex { get { return _selectedTowerIndex; } }
    public Item SelectedTower { get { return _towers[_selectedTowerIndex]; } }

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

    public void Initialize(int towerAmount)
    {
        _selectedTowerIndex = 0;
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
    }

    public void SelectTower(int index)
    {
        _selectedTowerIndex = index;

        if (_towers[index] == null) return;

        UIController.Instance.UpdateTowerUpgradeView();
    }

    public bool AddTower(int index, string name)
    {
        if (_towers[index] != null) return false;

        _towers[index] = new Item(ItemManager.Towers[name]);
        _towers[index].available = true;

        GameObject obj = ObjectPool.GetObject<Item>(name);
        obj.transform.SetParent(_towerPos[index]);

        _towerObjects[index] = obj.GetComponent<TowerObject>();

        Vector3 pos = _towerPos[index].position;
        pos.y = obj.transform.lossyScale.y / 2;

        obj.transform.position = pos;
        obj.transform.SetParent(_towerPos[index].parent);

        _towerObjects[index].UpdateTower(_towers[index]);

        UpdateShield();

        return true;
    }

    public void RemoveTower()
    {
        _towers[_selectedTowerIndex] = null;
        Destroy(_towerObjects[_selectedTowerIndex]);
    }

    public void Upgrade(int index, UpgradeDataType type, ref int money)
    {
        _towers[index].data[type].Upgrade(ref money);

        if (type == UpgradeDataType.SHIELD) UpdateShield();
    }

    private void UpdateShield()
    {
        float shield = 1;

        for (int i = 0; i < _towers.Length; i++)
        {
            if (_towers[i] == null) continue;

            float ts = 1 - _towers[i].GetValue(UpgradeDataType.SHIELD) / 100;

            shield *= ts;
        }
        shield = (1 - shield) * 100;
        Player.Instance.UpdateShield(shield);
    }
}
