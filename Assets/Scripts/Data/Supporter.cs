using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

[Serializable]
public class Supporter
{
    public string name;
    public SupporterType type;
    public SupporterAbilityType ability;
    public int value = 0;
    public float accurancy = 1;
    public float probablity = 1;
    public float delay = 1;

    public int cost = 0;
    public int upgradecost = 0;
    public float multiplyupgradecost = 0;
    public int maxupgrade = 0;
}