﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadAI : MonoBehaviour
{
    private Castle _castle;
    private Movement _movement;
    private Fighting _figter;
    private Transform _self;
    private (int x, int y) _gridPosition;

    public void Setup(Castle castle)
    {
        _castle = castle;
    }

    void Start()
    {
        if (_castle == null)
        {
            throw new System.Exception("The setup method hadn`t called");
        }
        _self = GetComponent<Transform>();
        _movement = GetComponent<Movement>();
        _figter = GetComponentInParent<Fighting>();

        StartCoroutine(Attacking());
    }

    public IEnumerator Attacking()
    {
        while (true)
        {
            (int x, int y) enemyTownPosition = GetClosestEnemyTown();
            _movement.GoTo(enemyTownPosition.x, enemyTownPosition.y);

            yield return new WaitForSeconds(4);

            if(_figter.UnitsNum < 30)
            {
                (int x, int y) friendlyTownPosition = GetClosestFriendlyTown();
                _movement.GoTo(friendlyTownPosition.x, friendlyTownPosition.y);
            }
        }
    }

    private (int x, int y) GetClosestEnemyTown()
    {
        _gridPosition = Grid.VectorToGridPosition(_self.position);

        (int x, int y)[] enemyTowns = GetEnemyTowns();
        return GetClosestPointFromArray(enemyTowns);
    }

    private (int x, int y) GetClosestFriendlyTown()
    {
        _gridPosition = Grid.VectorToGridPosition(_self.position);

        (int x, int y)[] friendlyTowns = GetFriendlyTowns();
        return GetClosestPointFromArray(friendlyTowns);
    }

    private (int x, int y)[] GetEnemyTowns()
    {
        List<(int x, int y)> enemyTownsPositions = new List<(int x, int y)>();
        foreach (KeyValuePair<(int x, int y), TownTag> town
                                                    in TownsContainer.Towns)
        {
            TownMoney townMoney = town.Value.GetComponent<TownMoney>();
            if ((townMoney && townMoney.Creator.City != _castle.City)
                || town.Value.gameObject != _castle.City)
            {
                enemyTownsPositions.Add(town.Key);
            }
        }

        return enemyTownsPositions.ToArray();
    }

    private (int x, int y)[] GetFriendlyTowns()
    {
        List<(int x, int y)> friendlyTownsPositons = new List<(int x, int y)>();
        foreach (KeyValuePair<(int x, int y), TownTag> town
                                                    in TownsContainer.Towns)
        {
            TownMoney townMoney = town.Value.GetComponent<TownMoney>();
            if ((townMoney && townMoney.Creator.City == _castle.City)
                || town.Value.gameObject == _castle.City)
            {
                friendlyTownsPositons.Add(town.Key);
            }
        }

        return friendlyTownsPositons.ToArray();
    }

    private (int x, int y) GetClosestPointFromArray((int x, int y)[] array)
    {
        (int x, int y) closestPosition = array[0];
        (int x, int y) minDistance = GetDistanceGrid(_gridPosition,
                                                    array[0]);
        foreach ((int x, int y) position in array)
        {
            (int x, int y) nextDistance = GetDistanceGrid(_gridPosition, position);
            if ((minDistance.x + minDistance.y)
                > (nextDistance.x + nextDistance.y))
            {
                minDistance = nextDistance;
                closestPosition = position;
            }
        }

        return closestPosition;
    }

    private (int x, int y) GetDistanceGrid((int x, int y) first,
                                            (int x, int y) second)
    {
        return (Mathf.Abs(first.x - second.x),
                Mathf.Abs(first.y - second.y));
    }
}
