using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadAI : MonoBehaviour
{
    private Castle _castle;
    private Movement _movement;
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

        StartCoroutine(Attacking());
    }

    public IEnumerator Attacking()
    {
        while (true)
        {
            _gridPosition = Grid.VectorToGridPosition(_self.position);

            // find all enemy towns
            List<(int x, int y)> enemyTownsPositions = new List<(int x, int y)>();
            foreach (KeyValuePair<(int x, int y), TownTag> town
                                                        in TownsContainer.Towns)
            {
                TownMoney townMoney = town.Value.GetComponent<TownMoney>();
                if (townMoney
                    && (townMoney.Creator.City != _castle.City
                        || town.Value.gameObject == _castle.City))
                {
                    enemyTownsPositions.Add(town.Key);
                }
            }

            // Get min distance to squad from towns
            (int x, int y) minDistance = GetDistanceGrid(_gridPosition,
                                                        enemyTownsPositions[0]);
            foreach((int x, int y) position in enemyTownsPositions)
            {
                (int x, int y) nextDistance = GetDistanceGrid(_gridPosition, position);
                if (minDistance.x > nextDistance.x
                    && minDistance.y > nextDistance.y)
                {
                    minDistance = nextDistance;
                }
            }

            _movement.GoTo(minDistance.x, minDistance.y);

            yield return new WaitForSeconds(10);
        }
    }

    private (int x, int y) GetDistanceGrid((int x, int y) first,
                                            (int x, int y) second)
    {
        return (Mathf.Abs(first.x - second.x),
                Mathf.Abs(first.y - second.y));
    }
}
