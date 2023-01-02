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
            else if (Input.GetMouseButtonUp(0))
            {
                AttackController.Instance.EndClick();
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

    public void EndLoading()
    {
        UIController.Instance.OpenMainView();
    }

    private const int StartingMoney = 0;
    private const int StartingWall = 1;
    private const int StartingDay = 1;
    private const int AvailableTower = 3;

    private const int LastDay = 40;
    public int MaxRound { get { return LastDay; } }

    public void StartGame(GameMode mode)
    {
        int money = StartingMoney;
        int wall = StartingWall;
        int day = StartingDay;
        int tower = AvailableTower;

        bool inv = false;
        bool inf = false;

        if (mode == GameMode.SandBox)
        {
            money = SandBoxController.Instance.Money;
            wall = SandBoxController.Instance.Wall;
            day = SandBoxController.Instance.Day;
            tower = SandBoxController.Instance.Tower;
            inv = SandBoxController.Instance.Inv;
            inf = SandBoxController.Instance.Inf;
        }

        RoundController.Instance.NewGame(day);
        Player.Instance.NewGame(money, wall, inv, inf);
        TowerController.Instance.Initialize(tower);
        WeaponController.Instance.Initialize();

        ProgressGame = true;
    }

    public void ReturnToMain()
    {
        TowerController.Instance.ClearTower();
        RoundController.Instance.GameEnd();
    }

    public void GameOver()
    {
        ProgressGame = false;
        UIController.Instance.GameEnd(false);
    }

    public void GameClear()
    {
        ProgressGame = false;
        UIController.Instance.GameEnd(true);
    }
}
