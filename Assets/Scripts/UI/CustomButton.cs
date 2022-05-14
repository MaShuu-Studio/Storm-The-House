using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnumData;

[RequireComponent(typeof(Button))]
public abstract class CustomButton : MonoBehaviour
{
    private Button _button;
    private ButtonType _type;
    private int _index;
    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => ClickEvent());
    }

    protected virtual void ClickEvent()
    {
        UIController.Instacne.PressButton(_type, _index);
    }

    public void SetButton(ButtonType type, int i)
    {
        _index = i;
        _type = type;
    }

    public abstract void SetIcon(string name);
}
