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

    public void SetUpgradeView(Item item)
    {
        nameText.text = item.name;
        costText.gameObject.SetActive(!item.available);
        costText.text = "$" + item.cost.ToString();
        descriptionText.text = item.description;
        descriptionText.gameObject.SetActive(!item.available);
        buyButton.gameObject.SetActive(!item.available);

        int i = 0;
        if (item.available)
        {
            List<UpgradeDataType> names = item.data.Keys.ToList();

            for (; i < item.data.Count; i++)
            {
                upgradeButtons[i].gameObject.SetActive(true);
                upgradeButtons[i].SetUpgradeData(names[i], item.data[names[i]]);
            }
        }

        for (; i < upgradeButtons.Count; i++)
        {
            upgradeButtons[i].gameObject.SetActive(false);
        }
    }
}
