using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponTowerItem : ItemButton, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image image;

    private string _name;
    public string Name { get { return _name; } }

    public override void SetIcon(string name)
    {
        _name = name;
        image.sprite = SpriteManager.GetIcon(name);
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