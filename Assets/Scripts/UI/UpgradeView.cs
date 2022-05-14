using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using EnumData;

public class UpgradeView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private List<UpgradeButton> upgradeButtons;
    [SerializeField] private Image upgradeItemImage;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private CustomButton buyButton;

    void Start()
    {
        buyButton.gameObject.SetActive(false);
    }

    public void SetUpgradeView(Weapon weapon)
    {
        nameText.text = weapon.name;
        costText.gameObject.SetActive(!weapon.available);
        costText.text = "$" + weapon.cost.ToString();
        descriptionText.gameObject.SetActive(!weapon.available);
        buyButton.gameObject.SetActive(!weapon.available);

        int i = 0;
        if (weapon.available)
        {
            List<WeaponDataType> names = weapon.data.Keys.ToList();

            for (; i < weapon.data.Count; i++)
            {
                upgradeButtons[i].gameObject.SetActive(true);
                upgradeButtons[i].SetUpgradeData(names[i], weapon.data[names[i]]);
            }
        }

        for (; i < upgradeButtons.Count; i++)
        {
            upgradeButtons[i].gameObject.SetActive(false);
        }
    }
}
