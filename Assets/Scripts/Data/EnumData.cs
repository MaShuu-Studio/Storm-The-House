using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnumData
{
    public enum GameMode { Campaign = 0, SandBox }
    public enum ItemType { WEAPON = 0, TOWER }
    public enum ButtonType { SUPPORTER = 0, WEAPON, TOWER, BUY, SELL, UPGRADE, ROUNDSTART, GAMESTART };
    public enum SupporterType { UNIT = 0, HOUSE }
    public enum SupporterAbilityType { ATTACK = 0, HEAL = 1, UPGRADE = 2 }
    public enum UpgradeDataType { AMMO = 0, ACCURANCY, DAMAGE, FIRERATE, RELOAD, RANGE, REMAINTIME, MISSILES, DELAY, SHIELD, SLOW, DOWN, FLAME }
    public enum WeaponTimerType { FIRE = 0, RELOAD }
}