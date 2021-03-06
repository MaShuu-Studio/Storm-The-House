using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class WeaponTowerItem : ItemButton, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TextMeshProUGUI nameText;

    private string _name;
    public string Name { get { return _name; } }

    public override void SetIcon(string name)
    {
        _name = name;
        nameText.text = name;
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        UIController.Instance.BeginDrag(_index);
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        UIController.Instance.Dragging(eventData.position);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        UIController.Instance.EndDrag();
    }
}