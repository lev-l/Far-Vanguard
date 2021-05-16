using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoosedArmyPresenter : MonoBehaviour
{
    public Sprite HaveSelected, HaventSelected;
    private Image _table;

    void Start()
    {
        _table = transform.GetChild(1).GetComponent<Image>();
    }

    public void IsSelected()
    {
        _table.sprite = HaveSelected;
    }

    public void IsUnselected()
    {
        _table.sprite = HaventSelected;
    }
}
