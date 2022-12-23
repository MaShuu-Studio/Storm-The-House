using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UsedWeaponItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image image;
    private string wName;
    private int _index;

    public void SetIndex(int index)
    {
        _index = index;
    }
    public void SetIcon(string name)
    {
        wName = name;
        image.sprite = SpriteManager.GetIcon(name);
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (wName == "") return;

        UIController.Instance.BeginDrag(WeaponController.Instance.UsingWeaponIndex(_index));
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        UIController.Instance.Dragging(eventData.position);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        UIController.Instance.EndDrag();
    }

    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        UIController.Instance.DropItem(_index);
    }
}
