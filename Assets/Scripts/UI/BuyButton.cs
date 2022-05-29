using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class BuyButton : CustomButton
{
    void Start()
    {
       SetButton(ButtonType.BUY, 0);
    }
}
