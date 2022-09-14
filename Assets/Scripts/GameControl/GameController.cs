using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;
using Data;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get { return instance; } }
    private static GameController instance;

    public bool ProgressGame = false;

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
        if (ProgressGame == false) return;

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

    public void StartGame()
    {
        RoundController.Instance.NewGame();
        Player.Instance.NewGame();
        TowerController.Instance.Initialize(3);
        WeaponController.Instance.Initialize();

        ProgressGame = true;
    }

    public void ReturnToMain()
    {
        RoundController.Instance.GameEnd();
    }

    public void GameOver()
    {
        ProgressGame = false;
        UIController.Instance.GameOver();
    }
}
