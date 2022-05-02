using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

[Serializable]
public class Supporter
{
    public string name;
    public SupporterType supportType;
    public int dmg;
    public float accurancy;
    public float probablity;
    public float delay;
}