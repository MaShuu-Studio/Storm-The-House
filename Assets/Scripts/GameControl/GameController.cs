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
        ItemManager.Initialize();
        EnemyManager.Initialize();
        RoundManager.Initialize();
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
}
