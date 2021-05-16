using System;
using UnityEngine;

public class TownTag : MonoBehaviour
{
    private void Start()
    {
        gameObject.AddComponent<SquadsRoom>();
        gameObject.AddComponent<TownFight>();
        TownsContainer.Towns
            .Add(Grid.VectorToGridPosition(transform.position),
                                                            this);
    }
}
