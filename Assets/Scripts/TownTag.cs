using System;
using UnityEngine;

public class TownTag : MonoBehaviour
{
    private void Start()
    {
        gameObject.AddComponent<SquadsRoom>();
        gameObject.AddComponent<TownFight>();
        Invoke(nameof(AddToTownsList), 0.01f);
    }

    public void AddToTownsList()
    {
        TownsContainer.Towns
            .Add(Grid.VectorToGridPosition(transform.position),
                                                            this);
    }
}
