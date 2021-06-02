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
    public GameObject AIPrefab;
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
                if(cell == "c")
                {
                    land.Add("land", Structs.Desert);
                    land.Add("town", Structs.TownCenter);
                    StartCoroutine(BuildCastle(x, y));
                }
                else if (cell == "a")
                {
                    land.Add("land", Structs.Desert);
                    land.Add("town", Structs.TownCenter);
                    StartCoroutine(CreateNewAI(x, y));
                    StartCoroutine(FirstUpdate(x, y));
                }
                else
                {
                    land.Add("land", (Structs)Convert.ToInt32(cell));
                    land.Add("town", Structs.None);
                }

                objects[x, y] = new Object(land, x, y);
            }
        }

        grid = new Grid(objects);
    }

    public IEnumerator BuildCastle(int x, int y)
    {
        yield return new WaitForSeconds(0.01f);

        grid.Cells[x, y].Items["town"] = Structs.TownCenter;
        StartCoroutine(FirstUpdate(x, y));
        Instantiate(CentralCastlePrefub,
                                    Grid.GridPositionToVector((x, y)),
                                    Quaternion.identity);
    }

    public IEnumerator CreateNewAI(int x, int y)
    {
        yield return new WaitForSeconds(0.01f);

        grid.Cells[x, y].Items["town"] = Structs.TownCenter;
        StartCoroutine(FirstUpdate(x, y));
        Instantiate(AIPrefab,
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
