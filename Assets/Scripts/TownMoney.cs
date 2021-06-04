using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownMoney : MonoBehaviour
{
    public GameObject MoneyTakerPrefub;
    private TownExpand _town;
    private TownTag _tag;
    private GameObject _selfMoneyTaker;
    private Vector2 _selfPosition;
    private int Cash = 0;

    private void Start()
    {
        _selfPosition = GetComponent<Transform>().position;
        _town = GetComponent<TownExpand>();
        _tag = GetComponent<TownTag>();
        _selfMoneyTaker = CreateMoneyTaker();
        StartCoroutine(CreateCash());
    }

    private void Update()
    {
        if(_selfMoneyTaker == null)
        {
            _selfMoneyTaker = CreateMoneyTaker();
        }
    }

    protected virtual GameObject CreateMoneyTaker()
    {
        GameObject moneyTaker = Instantiate(MoneyTakerPrefub, _selfPosition, Quaternion.identity);
        moneyTaker.GetComponent<Transform>().SetParent(transform);
        moneyTaker.GetComponent<MoneyTake>().Castle = _tag.Creator;

        return moneyTaker;
    }

    public int GiveMoney()
    {
        int money = Cash;
        Cash = 0;
        return money;
    }

    public IEnumerator CreateCash()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(3);
            Cash += _town.town.GetMoney();
        }
    }
}
