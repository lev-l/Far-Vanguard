using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownFight : MonoBehaviour
{
    public float fortification { get; private set; }
    private Land _land;
    private (int x, int y) _selfGridPosition;

    void Start()
    {
        GridSystem gridSystem = FindObjectOfType<GridSystem>();
        LandsBonuses bonuses = gridSystem.GetComponent<LandsBonuses>();

        _selfGridPosition = Grid.VectorToGridPosition(gameObject.transform.position);

        Structs landType = gridSystem.grid.Cells
                                            [
                                                _selfGridPosition.x,
                                                _selfGridPosition.y
                                            ]
                                            .Items["land"];
        switch (landType)
        {
            case Structs.Grass:
                _land = bonuses.Lands[0];
                break;
            case Structs.Desert:
                _land = bonuses.Lands[1];
                break;
            case Structs.FatLand:
                _land = bonuses.Lands[2];
                break;
        }

        fortification = _land.fortification;
        StartCoroutine(RestoreFortification());
    }

    public IEnumerator RestoreFortification()
    {
        while (true)
        {
            if (fortification < _land.fortification)
            {
                fortification += 0.01f;
            }
            yield return new WaitForSecondsRealtime(5);
        }
    }

    public int Attacked(int strength)
    {
        float strengthPercentage = (float)strength / 100f;
        float selfStrength = Random.Range(0.1f, 0.7f);
        strengthPercentage -= selfStrength;
        if (strengthPercentage < 0)
        {
            return (int)((_land.fortification - fortification) * 100);
        }

        fortification -= strengthPercentage;

        if(fortification <= 0)
        {
            DestroySelf();
            return (int)((_land.fortification - fortification) * 100);
        }
        else
        {
            return (int)((_land.fortification - fortification) * 100);
        }
    }

    public void DestroySelf()
    {
        GridSystem gridSystem = FindObjectOfType<GridSystem>();
        GridView view = gridSystem.GetComponent<GridView>();

        gridSystem.grid.Cells[_selfGridPosition.x,
                                _selfGridPosition.y].Items["town"] = Structs.None;
        view.UpdateViewIn(_selfGridPosition.x,
                         _selfGridPosition.y);

        foreach (Object @object in gridSystem.grid.GetNearestCells(_selfGridPosition.x,
                                                                    _selfGridPosition.y))
        {
            if (@object.Items["town"] != Structs.TownCenter)
            {
                @object.Items["town"] = Structs.None;
                view.UpdateViewIn(@object.x,
                                    @object.y);
            }
        }

        TownsContainer.Towns.Remove(_selfGridPosition);
        Destroy(gameObject);
    }
}
