using System;
using UnityEngine;

public class TownTag : MonoBehaviour
{
    public Castle Creator;

    public void SetCreator(Castle creator)
    {
        Creator = creator;
    }

    private void Start()
    {
        gameObject.AddComponent<SquadsRoom>();
        gameObject.AddComponent<TownFight>();

        if(Creator.City == null)
        {
            throw new Exception("The method SetCreator did not called");
        }
        Invoke(nameof(AddToTownsList), 0.01f);
    }

    public void AddToTownsList()
    {
        TownsContainer.Towns
            .Add(Grid.VectorToGridPosition(transform.position),
                                                            this);
    }
}
