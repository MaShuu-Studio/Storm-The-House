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
        if (RoundController.Instance.ProgressRound)
        {
            if (Input.GetMouseButton(0))
            {
                AttackController.Instance.Click(true);
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
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                AttackController.Instance.Click(false);
            }
        }
    }
}
