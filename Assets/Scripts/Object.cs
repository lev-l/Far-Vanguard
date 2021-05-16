using System;
using System.Collections.Generic;

public enum Structs
{
    None = 0,
    Grass = 1,
    Desert = 2,
    FatLand = 3,
    Sea = 4,
    TownCenter = 5,
    House = 6
}

public class Object
{
    public Dictionary<string, Structs> Items;
    public int x, y;

    public Object(int x, int y)
    {
        Items = new Dictionary<string, Structs>();
        this.x = x;
        this.y = y;

        Items.Add("land", Structs.Grass);
        Items.Add("town", Structs.None);
    }

    public Object(Dictionary<string, Structs> items, int x, int y)
    {
        Items = items;
        this.x = x;
        this.y = y;
    }
}
