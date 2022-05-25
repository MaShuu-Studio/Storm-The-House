using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Round
{
    public List<RoundEnemyData> enemies;
}

[Serializable]
public class RoundEnemyData
{
    public string name;
    public float delay;
    public float delayRange;
}