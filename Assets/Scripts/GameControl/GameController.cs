using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;
using Data;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get { return instance; } }
    private static GameController instance;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        SupporterManager.Initialize();
        WeaponManager.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            AttackController.Instance.Fire();
        }

        if (Input.GetButtonDown("RELOAD"))
        {
            WeaponController.Instance.Reload();
        }
    }

    void FixedUpdate()
    {
    }
    /*
    GUIStyle style = new GUIStyle();

    private void OnGUI()
    {
        string ammo = WeaponController.Instance.Ammo.ToString();
        bool canFire = WeaponController.Instance._canUse[WeaponTimerType.FIRE];
        bool isReloading = WeaponController.Instance._canUse[WeaponTimerType.RELOAD];

        style.fontSize = 50;
        GUI.Label(new Rect(10, 60, 50, 50), ammo, style);
        GUI.Label(new Rect(10, 110, 50, 50), canFire ? "CAN SHOOT" : "SHOOTING", style);
        GUI.Label(new Rect(10, 160, 50, 50), isReloading ? "CAN RELOAD" : "RELOADING", style);

        if (GUI.Button(new Rect(10, 10, 100, 50), "Add Enemy"))
        {
            int zpos = Random.Range(6, -6);

            Vector3 pos = new Vector3(-30, 0.75f, zpos);

            GameObject obj = ObjectPool.GetObject(EnumData.EnemyType.DUMMY);

            obj.transform.position = pos;
        }
    }*/
}
