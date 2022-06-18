using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnumData;

[RequireComponent(typeof(Button))]
public class CustomButton : MonoBehaviour
{
    protected Button _button;
    protected ButtonType _type;
    protected int _index;
    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => ClickEvent());
    }

    protected virtual void ClickEvent()
    {
        UIController.Instance.PressButton(_type, _index);
    }

    public void SetButton(ButtonType type, int i = 0)
    {
        _index = i;
        _type = type;
    }
}
