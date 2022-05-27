using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumData;

public class UpgradeButton : CustomButton
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Slider upgradeSlider;
    [SerializeField] private TextMeshProUGUI upgradeText;
    [SerializeField] private TextMeshProUGUI costText;

    UpgradeDataType _dataType;
    UpgradeData _data;

    public override void SetIcon(string name)
    {
        nameText.text = name;
    }

    public void SetUpgradeData(UpgradeDataType type, UpgradeData data)
    {
        _dataType = type;
        _data = data;

        nameText.text = type.ToString();
        upgradeSlider.minValue = data.defaultValue;
        upgradeSlider.value = data.currentValue;
        upgradeText.text = data.currentValue.ToString();
        upgradeSlider.maxValue = data.maxValue;
        costText.text = "$" + data.cost.ToString();
    }

    protected override void ClickEvent()
    {
        if (_data == null) return;

        UIController.Instance.Upgrade(_dataType);

        upgradeSlider.value = _data.currentValue;
        upgradeText.text = _data.currentValue.ToString();
        costText.text = "$" + _data.cost.ToString();
    }
}
