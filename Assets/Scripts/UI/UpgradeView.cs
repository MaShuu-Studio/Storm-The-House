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
    [SerializeField] private CustomButton dealButton;
    [SerializeField] private TextMeshProUGUI dealText;

    void Start()
    {
        dealButton.gameObject.SetActive(false);
    }

    public void SetUpgradeView(Item item)
    {
        nameText.text = item.name;
        costText.gameObject.SetActive(!item.available);
        costText.text = "$" + item.cost.ToString();
        descriptionText.text = item.description;
        upgradeItemImage.sprite = SpriteManager.GetImage(item.name);

        descriptionText.gameObject.SetActive(!item.available);
        if (item.available == false)
        {
            dealText.text = "BUY";
            dealButton.SetButton(ButtonType.BUY);
            dealButton.gameObject.SetActive(!item.available && (Player.Instance.Money > item.cost));
        }
        else if (item.type == ItemType.TOWER)
        {
            dealText.text = "SELL\n(90%)";
            dealButton.SetButton(ButtonType.SELL);
            dealButton.gameObject.SetActive(item.available);
        }
        else
        {
            dealButton.gameObject.SetActive(false);
        }

        int i = 0;
        if (item.available)
        {
            List<UpgradeDataType> names = item.data.Keys.ToList();

            int contentsAmount = 0;
            for (; i < item.data.Count; i++)
            {
                if (item.data[names[i]] == null || item.data[names[i]].defaultValue >= item.data[names[i]].maxValue)
                {
                    if (i > 2) continue;
                    upgradeButtons[i].gameObject.SetActive(false);
                    continue;
                }
                upgradeButtons[contentsAmount].gameObject.SetActive(true);
                upgradeButtons[contentsAmount].SetUpgradeData(names[i], item.data[names[i]]);
                contentsAmount++;
            }
        }

        for (; i < upgradeButtons.Count; i++)
        {
            upgradeButtons[i].gameObject.SetActive(false);
        }
    }
}
