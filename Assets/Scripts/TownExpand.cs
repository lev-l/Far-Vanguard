using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownExpand : MonoBehaviour
{
    public float SpeedOfExpand;
    public  Town town;
    private LandsBonuses _landsBonuses;
    private GridView _gridView;
    private GridSystem _gridSystem;

    void Start()
    {
        GameObject grid = FindObjectOfType<GridView>().gameObject;
        _landsBonuses = grid.GetComponent<LandsBonuses>();
        _gridView = grid.GetComponent<GridView>();
        _gridSystem = grid.GetComponent<GridSystem>();

        switch (_gridSystem.grid.Cells[town.x, town.y].Items["land"])
        {
            case Structs.Grass:
                Land selfLand = _landsBonuses.Lands[0];
                SpeedOfExpand *= selfLand.buildSpeedBonus;
                town.IncomeMutator = selfLand.moneyBonus;
                town.StartIncomeMutator = 0;
                break;
            case Structs.Desert:
                selfLand = _landsBonuses.Lands[1];
                SpeedOfExpand *= selfLand.buildSpeedBonus;
                town.IncomeMutator = selfLand.moneyBonus;
                town.StartIncomeMutator = -3;
                break;
            case Structs.FatLand:
                selfLand = _landsBonuses.Lands[2];
                SpeedOfExpand *= selfLand.buildSpeedBonus;
                town.IncomeMutator = selfLand.moneyBonus;
                town.StartIncomeMutator = 0;
                break;
        }
        StartCoroutine(ExpandTowns());
    }

    public IEnumerator ExpandTowns()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(SpeedOfExpand);
            (int x, int y) newTownPosition = town.CreateHouse();
            if (newTownPosition.x >= 0 && newTownPosition.y >= 0)
            {
                _gridView.UpdateViewIn(newTownPosition.x, newTownPosition.y);
            }
            else
            {
                break;
            }
        }
        yield break;
    }
}
