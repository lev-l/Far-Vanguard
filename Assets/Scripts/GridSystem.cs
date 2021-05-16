using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GridSystem : MonoBehaviour
{
    public string Map;
    public int width { get; private set; }
    public int height { get; private set; }
    public GameObject CentralCastlePrefub;
    public Grid grid { get; private set; }
    private GridView _gridView;

    void Awake()
    {
        _gridView = GetComponent<GridView>();

        string[] mapData = File.ReadAllLines(Map + "(formated).txt");
        width = mapData[0].Length - 1;
        height = mapData.Length;
        Object[,] objects = new Object[width, height];

        for (int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                Dictionary<string, Structs> land = new Dictionary<string, Structs>();
                string cell = mapData[y][x].ToString();
                land.Add("land",  (Structs)Convert.ToInt32(cell));
                land.Add("town", Structs.None);

                objects[x, y] = new Object(land, x, y);
            }
        }

        grid = new Grid(objects);

        bool isCastleNotBuilded = true;
        foreach (Object cell in grid.Cells)
        {
            if (cell.Items["land"] == Structs.TownCenter
                && isCastleNotBuilded)
            {
                cell.Items["land"] = Structs.Desert;
                cell.Items["town"] = Structs.TownCenter;
                BuildCastle(cell.x, cell.y);
                isCastleNotBuilded = false;
            }
            else if(cell.Items["land"] == Structs.TownCenter)
            {
                cell.Items["land"] = Structs.Desert;
                cell.Items["town"] = Structs.TownCenter;
                StartCoroutine(FirstUpdate(cell.x, cell.y));
            }
        }
    }

    public void BuildCastle(int x, int y)
    {
        grid.Cells[x, y].Items["town"] = Structs.TownCenter;
        StartCoroutine(FirstUpdate(x, y));
        Instantiate(CentralCastlePrefub,
                                    Grid.GridPositionToVector((x, y)),
                                    Quaternion.identity);
    }

    public void UpdateIn(int x, int y)
    {
        _gridView.UpdateViewIn(x, y);
    }

    public IEnumerator FirstUpdate(int x, int y)
    {
        yield return new WaitForSeconds(0.1f);
        _gridView.UpdateViewIn(x, y);
    }
}
