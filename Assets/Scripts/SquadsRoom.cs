using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquadsRoom : MonoBehaviour, Build
{
    public int NumOfSquads { get; private set; }
    private Vector2 _selfPosition;
    private int _regenerationRate;
    private List<Squad> _enteredSquads;
    private List<Fighting> _enteredFighters;
    private GameObject _armyInBord;

    private void Start()
    {
        _selfPosition = GetComponent<Transform>().position;
        _enteredSquads = new List<Squad>();
        _enteredFighters = new List<Fighting>();

        (int x, int y) selfGridPosition = Grid.VectorToGridPosition(transform.position);
        GridSystem gridSystem = FindObjectOfType<GridSystem>();

        LandsBonuses bonuses = gridSystem.GetComponent<LandsBonuses>();
        Structs selfLand = gridSystem.grid.Cells
                                [
                                    selfGridPosition.x,
                                    selfGridPosition.y
                                ].Items["land"];
        switch (selfLand)
        {
            case Structs.Desert:
                _regenerationRate = bonuses.Lands[1].regenerationBonuse;
                break;
            case Structs.FatLand:
                _regenerationRate = bonuses.Lands[2].regenerationBonuse;
                break;
            default:
                _regenerationRate = bonuses.Lands[0].regenerationBonuse;
                break;
        }

        StartCoroutine(RegenSquads());
    }

    public void EnterSquad(Squad squad)
    {
        if (squad.gameObject.activeSelf)
        {
            squad.gameObject.SetActive(false);
            SquadAdd(squad);
            NumOfSquads = _enteredSquads.Count;
        }
    }

    public void ExitSquad()
    {
        if (_enteredSquads.Count > 0)
        {
            if (_enteredSquads[0])
            {
                _enteredSquads[0].gameObject.SetActive(true);
                SquadRemove();
            }
            else
            {
                SquadRemove();
            }
        }
        NumOfSquads = _enteredSquads.Count;
    }

    private void SquadAdd(Squad squad)
    {
        if(_enteredSquads.Count == 0)
        {
            _armyInBord = Instantiate(Resources.Load<GameObject>("ArmyInBording"));
            _armyInBord.transform.position = _selfPosition;
        }
        _enteredSquads.Add(squad);
        _enteredFighters.Add(squad.GetComponentInParent<Fighting>());
    }

    private void SquadRemove()
    {
        _enteredSquads.RemoveAt(0);
        _enteredFighters.RemoveAt(0);

        if(_enteredSquads.Count == 0)
        {
            Destroy(_armyInBord);
        }
    }

    public void DestroySquads()
    {
        Destroy(_armyInBord);
        for (int i = 0; i != _enteredFighters.Count;)
        {
            if (_enteredFighters[i] != null)
            {
                Destroy(_enteredFighters[i].gameObject);
            }
            _enteredFighters.RemoveAt(i);
        }
    }

    public IEnumerator RegenSquads()
    {
        while (true)
        {
            foreach(Fighting fighter in _enteredFighters)
            {
                if(fighter.UnitsNum < 100)
                {
                    fighter.UnitsNum += _regenerationRate;
                }
                if(fighter.UnitsNum > 100)
                {
                    fighter.UnitsNum = 100;
                }
            }
            yield return new WaitForSecondsRealtime(1);
        }
    }
}
