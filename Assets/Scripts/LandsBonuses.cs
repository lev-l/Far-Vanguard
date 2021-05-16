using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Land
{
    public float buildSpeedBonus;
    public int regenerationBonuse;
    public float moneyBonus;
    public float fortification;
}

public class LandsBonuses : MonoBehaviour
{
    public Land[] Lands;
}
