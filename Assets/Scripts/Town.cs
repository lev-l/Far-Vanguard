using System;
using System.Collections.Generic;
using UnityEngine;

public interface ITown
{
    (int x, int y) CreateHouse();
    int GetMoney();
}

public class Town : ITown
{
    public float IncomeMutator;
    public int StartIncomeMutator;
    public int x { get; private set; }
    public int y { get; private set; }
    private Grid _grid;
    private int _houseCount;

    public Town(Grid grid, int x, int y)
    {
        _houseCount = 1;
        _grid = grid;
        this.x = x;
        this.y = y;
    }

    public (int x, int y) CreateHouse()
    {
        List<Object> objects = new List<Object>();
        objects.AddRange(_grid.GetNearestCells(x, y));

        for(int i = 0; i < objects.Count; i++)
        {
            if(objects[i].Items["land"] == Structs.Sea)
            {
                objects.RemoveAt(i);
                i--;
            }
        }

        Object[] groundObjects = objects.ToArray();
        // проверка на наличие свободных мест рядом
        int numOfBuidings = 0;
        foreach (Object ground in groundObjects)
        {
            if (ground.Items["town"] == Structs.None)
            {
                break;
            }
            else
            {
                numOfBuidings++;
            }
            if (numOfBuidings == groundObjects.Length)
            {
                // возврат значения, означающего окончание корутины роста
                // поселения
                return (-1, -1);
            }
        }
        // поиск свободной рандомной точки рядом
        bool hasHouseNotSpawned = true;
        while (hasHouseNotSpawned)
        {
            Object newHouseCell = groundObjects[UnityEngine.Random.Range(0, groundObjects.Length)];
            if (newHouseCell.Items["town"] == Structs.None)
            {
                // спавн в ней домика
                _grid.Cells[newHouseCell.x, newHouseCell.y].Items["town"] = Structs.House;
                _houseCount++;
                hasHouseNotSpawned = false;
                return (newHouseCell.x, newHouseCell.y);
            }
        }
        throw new Exception("Why are you here?");
        return (0, 0);
    }

    public int GetMoney()
    {
        return Mathf.RoundToInt((_houseCount * IncomeMutator) + StartIncomeMutator);
    }
}