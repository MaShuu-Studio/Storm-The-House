using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SupporterItem : ItemButton
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private GameObject max;
    [SerializeField] private Image image;

    private string _name;
    public string Name { get { return _name; } }

    public override void SetIcon(string name)
    {
        _name = name;
        nameText.text = name;
        UpdateUpgradable(true);
        image.sprite = SpriteManager.GetIcon(name);
    }

    public void UpdateCost(int cost)
    {
        costText.text = "$ " + cost.ToString();
    }

    public void UpdateUpgradable(bool upgradable)
    {
        max.SetActive(!upgradable);
    }
}