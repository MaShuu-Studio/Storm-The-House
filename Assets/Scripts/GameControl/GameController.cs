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
        EnemyManager.Initialize();
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

        if (Input.GetButtonDown("SWITCH"))
        {
            WeaponController.Instance.SwitchWeapon();
        }
    }

    void FixedUpdate()
    {
    }
    
    GUIStyle style = new GUIStyle();

    private void OnGUI()
    {
        string ammo = WeaponController.Instance.Ammo.ToString();
        bool canFire = WeaponController.Instance._canUse[WeaponTimerType.FIRE];
        bool isReloading = WeaponController.Instance._canUse[WeaponTimerType.RELOAD];

        style.fontSize = 50;

        if (GUI.Button(new Rect(10, 10, 100, 50), "Add Enemy"))
        {
            int zpos = Random.Range(6, -6); 

            Vector3 pos = new Vector3(-30, 0.75f, zpos);

            GameObject obj = ObjectPool.GetObject<Enemy>(EnemyManager.Types[0]);

            obj.transform.position = pos;
        }
    }
}
