using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnumData
{
    public enum ItemType { WEAPON = 0, TOWER }
    public enum ButtonType { SUPPORTER = 0, WEAPON, TOWER, BUY, UPGRADE, ROUNDSTART };
    public enum SupporterType { ATTACK = 0, DEFENCE }
    public enum UpgradeDataType { AMMO = 0, ACCURANCY, RELOAD, FIRERATE, DAMAGE, RANGE, REMAINTIME, MISSILES, SHIELD, SLOW, DOWN }
    public enum WeaponTimerType { FIRE = 0, RELOAD }
}