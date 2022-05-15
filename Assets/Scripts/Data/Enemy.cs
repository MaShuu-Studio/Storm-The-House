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
    public float attackDelay = 1;
    public float attackRange = 0.5f;
    public float speed;
     
    public int money;
}