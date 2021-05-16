using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomataClose : MonoBehaviour
{
    void Start()
    {
        TownsMenu.Menu = gameObject;
        gameObject.SetActive(false);
    }
}
