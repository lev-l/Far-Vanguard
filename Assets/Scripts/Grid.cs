using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public Object[,] Cells { get; private set; }

    public Grid(Object[,] cells)
    {
        Cells = cells;

    }

    public bool CreateTown(int x, int y)
    {
        if (Cells[x, y].Items["town"] == Structs.None)
        {
            Cells[x, y].Items["town"] = Structs.TownCenter;
            return true;
        }
        return false;
    }

    public Object[] GetNearestCells(int cellX, int cellY)
    {
        List<Object> nearestCells = new List<Object>();

        for (int x = cellX - 1; x <= cellX + 1; x++)
        {
            if (x < 0 || x > (Cells.GetLength(0) - 1))
            {
                continue;
            }
            for (int y = cellY - 1; y <= cellY + 1; y++)
            {
                if (y < 0 || y > (Cells.GetLength(1) - 1) || (x == cellX && y == cellY))
                {
                    continue;
                }
                nearestCells.Add(Cells[x, y]);
            }
        }
        return nearestCells.ToArray();
    }

    public Landscape GetNearestLand(int x, int y)
    {
        Landscape land = new Landscape();

        land.Up = SetObjectInPositionSafely(x: x,
                                            y: y + 1,
                                            axis: 1,
                                            defoultX: x,
                                            defoultY: y);
        land.Left = SetObjectInPositionSafely(x: x - 1,
                                                y: y,
                                                axis: 0,
                                                defoultX: x,
                                                defoultY: y);
        land.Right = SetObjectInPositionSafely(x: x + 1,
                                                y: y,
                                                axis: 0,
                                                defoultX: x,
                                                defoultY: y);
        land.Down = SetObjectInPositionSafely(x: x,
                                                y: y - 1,
                                                axis: 1,
                                                defoultX: x,
                                                defoultY: y);

        return land;
    }

    private Object SetObjectInPositionSafely(int x,
                                            int y,
                                            int axis,
                                            int defoultX,
                                            int defoultY)
    {
        if(PositionInRange(x, y, Cells.GetLength(axis)))
        {
            return Cells[x, y];
        }
        else
        {
            return Cells[defoultX, defoultY];
        }
    }

    private bool PositionInRange(int x, int y, int maxRange)
    {
        return (x > 0 && y < maxRange)
             && (y > 0 && y < maxRange);
    }

    public static Vector3 GridPositionToVector((int x, int y) position)
    {
        Vector3 vector = new Vector3();

        vector.x = Mathf.Abs(position.x * 0.25f);
        vector.y = Mathf.Abs(position.y * 0.25f);

        return vector;
    }

    public static (int x, int y) VectorToGridPosition(Vector3 vector)
    {
        (int x, int y) position = (0, 0);

        position.x = Mathf.Abs(
                        Mathf.RoundToInt(vector.x / 0.25f));
        position.y = Mathf.Abs(
                        Mathf.RoundToInt(vector.y / 0.25f));
        
        return position;
    }

    public static Vector3 RoundVectorToVectorOnGrid(Vector3 vector)
    {
        return Grid.GridPositionToVector(Grid.VectorToGridPosition(vector));
    }

    public static (int x, int y) GetDistance((int x, int y) first,
                                            (int x, int y) second)
    {
        return (Mathf.Abs(first.x - second.x),
                Mathf.Abs(first.y - second.y));
    }
}

public class Landscape
{
    public Object Up;
    public Object Left;
    public Object Right;
    public Object Down;

    public Landscape(Object up,
                    Object left,
                    Object right,
                    Object down)
    {
        Up = up;
        Left = left;
        Right = right;
        Down = down;
    }

    public Landscape()
    {
        Up = new Object(0, 0);
        Left = new Object(0, 0);
        Right = new Object(0, 0);
        Down = new Object(0, 0);
    }
}