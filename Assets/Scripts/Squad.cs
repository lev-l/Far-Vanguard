using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squad : MonoBehaviour
{
    public Castle Creator;
    private Siege _townAttack;
    private Fighting _fighter;
    protected Transform _self;
    protected GridSystem _gridSystem;

    public void SetSquadFirst(Castle creator)
    {
        Creator = creator;
    }

    private void Start()
    {
        if(Creator == null)
        {
            throw new System.Exception("method named SetSquadFirst does not called");
        }

        if (Creator.Creator != null)
        {
        }

        _self = GetComponent<Transform>();
        _gridSystem = FindObjectOfType<GridSystem>();
        _townAttack = GetComponent<Siege>();
        _fighter = GetComponentInParent<Fighting>();
    }

    public void TargetAchived()
    {
        if (_self != null)
        {
            if (IsMyPositionTown()
                || IsMyPositionCastle())
            {
                TownsContainer.Towns[Grid.VectorToGridPosition(_self.position)]
                                    .GetComponent<SquadsRoom>().EnterSquad(this);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        _fighter.SeeEnemy(other);
    }

    protected bool IsMyPositionCastle()
    {
        (int x, int y) positionOnGrid = Grid.VectorToGridPosition(_self.position);

        if (TownsContainer.Towns.ContainsKey(positionOnGrid))
        {
            GameObject castle = TownsContainer.Towns[positionOnGrid].gameObject;
            if(castle == Creator.City)
            {
                return true;
            }
            else
            {
                if (!castle.GetComponent<TownExpand>())
                {
                    _townAttack.WentedToEnemyTown(positionOnGrid);
                }
                return false;
            }
        }
        return false;
    }

    protected bool IsMyPositionTown()
    {
        (int x, int y) positionOnGrid = Grid.VectorToGridPosition(_self.position);

        TownMoney town;
        if (ThereIsTownAndItIsntACastle(positionOnGrid))
        {
            town = TownsContainer.Towns[positionOnGrid]
                                        .GetComponent<TownMoney>();
            // return true only if master of city the same
            // (we must not enter to the enemi`s city)
            if(town.Creator.City == Creator.City)
            {
                return true;
            }
            else
            {
                _townAttack.WentedToEnemyTown(positionOnGrid);
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    protected bool ThereIsTownAndItIsntACastle((int x, int y) position)
    {
        return TownsContainer.Towns.ContainsKey(position)
            && TownsContainer.Towns[position].GetComponent<TownExpand>();
    }
}
