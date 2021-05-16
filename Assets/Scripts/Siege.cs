using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Siege : MonoBehaviour
{
    private Fighting _fighter;

    void Start()
    {
        _fighter = GetComponentInParent<Fighting>();
    }

    public void WentedToEnemyTown((int x, int y) enemyTownPosition)
    {
        TownFight town = TownsContainer.Towns[enemyTownPosition]
                                .GetComponent<TownFight>();
        _fighter.UnitsNum -= town.Attacked(_fighter.GetStrength());
        if(_fighter.UnitsNum < 0)
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
