using System;
using UnityEngine;

[Serializable]
public class Castle
{
    public ICashTaker Creator;
    public GameObject City;

    public Castle(ICashTaker creator, GameObject city)
    {
        Creator = creator;
        City = city;
    }
}
