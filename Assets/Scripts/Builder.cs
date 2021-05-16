using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : Movement
{
    public GameObject TownPrefub;
    private int _buildCost;
    private Castle _castle;
    private GridSystem _grid;
    private IState _state;

    public void SetParams(GridSystem grid, Vector2 target,
                        Castle castle, int buildCost)
    {
        _grid = grid;
        _target = new TargetsStackContainer(target);
        _castle = castle;
        _buildCost = buildCost;
    }
    
    void Start()
    {
        if(_grid == null)
        {
            throw new System.Exception("Method 'SetParams' had not called");
        }

        _self = GetComponent<Transform>();
        _state = new MoveState(_self, _target, _castle);
    }

    void Update()
    {
        _state = _state.Next();

        if (!(_state is MoveState))
        {
            (int x, int y) buildPosition = Grid.VectorToGridPosition(_self.position);
            GameObject town = Build(buildPosition.x, buildPosition.y);

            if (town)
            {
                TownExpand newTown = town.GetComponent<TownExpand>();
            }
            else
            {
                _castle.Creator.GiveMoney(_buildCost);
            }
            Destroy(gameObject);
        }
    }

    public GameObject Build(int x, int y)
    {
        if (_grid.grid.CreateTown(x, y))
        {
            _grid.UpdateIn(x, y);
            GameObject newTown = Instantiate(TownPrefub,
                                            Grid.GridPositionToVector((x, y)),
                                            Quaternion.identity);
            newTown.GetComponent<TownExpand>().town = new Town(_grid.grid, x, y);
            newTown.GetComponent<TownMoney>().Creator = _castle;

            _castle.Creator.AddFlagTo(newTown.transform);
            return newTown;
        }
        else
        {
            return null;
        }
    }
}
