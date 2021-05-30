using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandartAI : MonoBehaviour, ICashTaker
{
    public GameObject BuilderPrefab;
    public GameObject SoldierPrefab;
    private Transform _self;
    private (int x, int y) _gridPosition;
    private GridSystem _gridSystem;
    private List<Squad> _squads;
    private float[,] _values;
    private Dictionary<Structs, float> _landsValues;
    private int _cash;
    private int _income;
    private int _trainCost, _buildCost;
    private bool _isTherePlaceToBuild;
    delegate void MainAction();

    void Start()
    {
        _squads = new List<Squad>();
        _landsValues = new Dictionary<Structs, float>();
        _landsValues.Add(Structs.Grass, 100);
        _landsValues.Add(Structs.FatLand, 1000);
        _landsValues.Add(Structs.Desert, 10);
        _landsValues.Add(Structs.Sea, 0);
        _landsValues.Add(Structs.House, 0);
        _landsValues.Add(Structs.TownCenter, 0);

        CentralCastle castle = FindObjectOfType<CentralCastle>();
        _trainCost = castle.GetCosts().forTrain;
        _buildCost = castle.GetCosts().forBuild;
        _cash = 90;

        _self = GetComponent<Transform>();
        _gridSystem = FindObjectOfType<GridSystem>();
        _values = new float[_gridSystem.width, _gridSystem.height];
        
        foreach(Object cell in _gridSystem.grid.Cells)
        {
            if(cell.Items["town"] == Structs.TownCenter
                && (cell.x, cell.y) != Grid.VectorToGridPosition(
                                            castle
                                            .transform
                                            .position)
                                        )
            {
                _self.position = Grid.GridPositionToVector((cell.x, cell.y));
                _gridPosition = (cell.x, cell.y);
            }
        }

        for(int x = 0; x < _values.GetLength(0); x++)
        {
            for(int y = 0; y < _values.GetLength(1); y++)
            {
                if (_gridSystem.grid.Cells[x, y].Items["town"] == Structs.None)
                {
                    _values[x, y] = _landsValues[_gridSystem.grid.Cells[x, y].Items["land"]]
                                    / (Mathf.Abs(_gridPosition.x - x) + Mathf.Abs(_gridPosition.y - y));
                }
                else
                {
                    _values[x, y] = 0;
                }
            }
        }

        _isTherePlaceToBuild = true;
        NullingCells(_gridPosition.x, _gridPosition.y);
        StartCoroutine(CalculateIncome());
        StartCoroutine(Flow());
    }

    public IEnumerator Flow()
    {
        MainAction action = Building;
        while (true)
        {
            action();
            if (_income > 15)
            {
                action = Training;
            }
            else if (_income < 10)
            {
                action = Building;
            }
            yield return new WaitForSeconds(5);
        }
    }

    public void Building()
    {
        if (_isTherePlaceToBuild
               && _cash >= _buildCost + (5 * _squads.Count)
               && CanBuild())
        {
            BuildTown();
        }
    }

    public void Training()
    {
        if (_cash >= _trainCost + (5 * _squads.Count)
            && CanTrain())
        {
            Squad newSquad = Instantiate(SoldierPrefab, _self.position, Quaternion.identity)
                                        .GetComponentInChildren<Squad>();
            newSquad.SetSquadFirst(new Castle(this, gameObject));
            newSquad.gameObject.AddComponent<SquadAI>().Setup(new Castle(this, gameObject));

            _income += 200;
            _squads.Add(newSquad);
        }
    }

    private void BuildTown(int turn = 0)
    {
        //the position what we use for create town
        (int x, int y) maxPosition = (0, 0);

        //getting the max value in values
        float max = 0;
        for (int x = 0; x < _values.GetLength(0); x++)
        {
            for (int y = 0; y < _values.GetLength(1); y++)
            {
                if (_values[x, y] > max)
                {
                    max = _values[x, y];
                }
            }
        }

        if(max == 0)
        {
            _isTherePlaceToBuild = false;
            return;
        }

        //find random, but close place for build town
        max -= Random.Range(0.25f, 16);
        for (int x = 0; x < _values.GetLength(0); x++)
        {
            for (int y = 0; y < _values.GetLength(1); y++)
            {
                if (_values[x, y] + 10f > max
                    && _values[x, y] - 10f < max
                    && (_values[x, y] != 0))
                {
                    maxPosition = (x, y);
                    break;
                }
            }
        }

        if(maxPosition == (0, 0))
        {
            return;
        }

        //nulling if there are a town, and try one more position
        TownTag value;
        if(TownsContainer.Towns.TryGetValue(maxPosition, out value)
            && turn < 50)
        {
            NullingCells(maxPosition.x, maxPosition.y, 0);
            turn++;
            BuildTown(turn);
            return;
        }

        //if we do not find this position, try one more
        if((maxPosition.x == 0 && maxPosition.y == 0)
            && turn < 50)
        {
            turn++;
            BuildTown(turn);
            return;
        }

        //the towns mustn`t be too close one other
        NullingCells(maxPosition.x, maxPosition.y);

        //create a bulider, who will build town
        GameObject newBuilder = MonoBehaviour.Instantiate(BuilderPrefab,
            _self.position, Quaternion.identity);
        newBuilder.GetComponent<Builder>()
            .SetParams(_gridSystem, Grid.GridPositionToVector(maxPosition),
                            new Castle(this, gameObject), _buildCost);
    }

    private void NullingCells(int x, int y, int layer = 0)
    {
        int numOfLayers = 6;
        layer += Random.Range((int)2, 5);

        _values[x, y] = 0;
        foreach (Object cell in _gridSystem.grid.GetNearestCells(x, y))
        {
            _values[cell.x, cell.y] = 0;
            if (layer < numOfLayers)
            {
                NullingCells(cell.x, cell.y, layer);
            }
        }
    }

    private IEnumerator CalculateIncome()
    {
        while (true)
        {
            int earlyIncome = _income;
            int earlyCash = _cash;
            yield return new WaitForSecondsRealtime(7);
            _income = ((_cash - earlyCash) + earlyIncome) / 2;
        }
    }

    public void AddFlagTo(Transform subject)
    {
    }

    public int GiveMoney(int amount)
    {
        if(amount < 0 && _cash + amount < 0)
        {
            _cash = 0;
            return amount;
        }
        _cash += amount;
        return 0;
    }

    public bool Pay(int cost)
    {
        if (cost <= _cash)
        {
            _cash -= cost;
            return true;
        }
        return false;
    }

    public bool CanTrain()
    {
        return Pay(_trainCost);
    }

    public bool CanBuild()
    {
        return Pay(_buildCost);
    }
}
