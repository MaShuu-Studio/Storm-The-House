using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SupporterItem : ItemButton
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image image;

    private string _name;
    public string Name { get { return _name; } }

    public override void SetIcon(string name)
    {
        _name = name;
        nameText.text = name;
        image.sprite = SpriteManager.GetIcon(name);
    }
}