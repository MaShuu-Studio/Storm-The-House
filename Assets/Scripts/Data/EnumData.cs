using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnumData
{
    public enum ButtonType { SUPPORTER = 0, WEAPON, TOWER, BUY, UPGRADE };
    public enum SupporterType { ATTACK = 0, DEFENCE }
    public enum WeaponDataType { AMMO = 0, ACCURANCY, RELOAD, FIRERATE, DAMAGE, RANGE }
    public enum WeaponTimerType { FIRE, RELOAD }
}