using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponTowerItem : CustomButton
{
    [SerializeField] private TextMeshProUGUI nameText;

    private string _name;
    public string Name { get { return _name; } }

    public override void SetIcon(string name)
    {
        _name = name;
        nameText.text = name;
    }
}