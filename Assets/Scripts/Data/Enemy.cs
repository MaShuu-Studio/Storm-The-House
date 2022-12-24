using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Enemy
{
    public string name;

    public float hp;
    public int dmg;
    public float speed;
    public float attackdelay = 1;
    public float attackrange = 0.5f;
     
    public int money;
}