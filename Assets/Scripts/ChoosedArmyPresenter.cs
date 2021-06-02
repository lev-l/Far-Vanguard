using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoosedArmyPresenter : MonoBehaviour
{
    public Sprite HaveSelected, HaventSelected;
    public GameObject Pointer;
    public Vector2 Offset;
    private GameObject _instancePointer;
    private Image _table;

    void Awake()
    {
        _table = transform.GetChild(1).GetComponent<Image>();

        _instancePointer = Instantiate(Pointer);
        _instancePointer.SetActive(false);
    }

    public void IsSelected(GameObject choosed)
    {
        if (!_instancePointer)
        {
            _instancePointer = Instantiate(Pointer);
            _instancePointer.SetActive(false);
        }
        _instancePointer.transform.SetParent(choosed.transform);
        _instancePointer.transform.localPosition = Offset;
        _instancePointer.SetActive(true);

        _table.sprite = HaveSelected;
    }

    public void IsUnselected()
    {
        if (!_instancePointer)
        {
            _instancePointer = Instantiate(Pointer);
        }
        _instancePointer.SetActive(false);

        _table.sprite = HaventSelected;
    }
}
