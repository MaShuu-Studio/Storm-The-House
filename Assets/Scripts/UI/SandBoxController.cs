using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SandBoxController : MonoBehaviour
{
    public static SandBoxController Instance { get { return instance; } }
    private static SandBoxController instance;

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

    [SerializeField] private TMP_InputField money;
    [SerializeField] private TMP_InputField wall;
    [SerializeField] private TMP_InputField startday;
    [SerializeField] private TMP_InputField tower;

    [SerializeField] private Toggle invincibile;
    [SerializeField] private Toggle infiniteMoney;

    #region Field
    public int Money
    {
        get
        {
            if (money.text == "") return 0;
            return int.Parse(money.text);
        }
    }

    public int Wall
    {
        get
        {
            if (wall.text == "") return 1;
            else if (int.Parse(wall.text) < 1) wall.text = "1";
            return int.Parse(wall.text);
        }
    }
    public int Day
    {
        get
        {
            if (startday.text == "") return 1;
            else if (int.Parse(startday.text) < 1) startday.text = "1";
            return int.Parse(startday.text);
        }
    }
    public int Tower
    {
        get
        {
            if (tower.text == "") return 3;
            else if (int.Parse(tower.text) < 3) tower.text = "3";
            return int.Parse(tower.text);
        }
    }
    public bool Inv { get { return invincibile.isOn; } }
    public bool Inf { get { return infiniteMoney.isOn; } }
    #endregion

    private const int MaxMoney = 9999999;
    private const int MaxWall = 6;
    private const int MaxDay = 40;
    private const int MaxTower = 7;

    private void Update()
    {
        if (Money > 9999999) money.text = MaxMoney.ToString();
        if (Wall > MaxWall) wall.text = MaxWall.ToString();
        if (Day > MaxDay) startday.text = MaxDay.ToString(); ;
        if (Tower > MaxTower) tower.text = MaxTower.ToString();
    }
}
