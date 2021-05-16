using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyPresenter : MonoBehaviour
{
    private Text MoneyView;

    void Start()
    {
        MoneyView = GameObject.Find("Money").GetComponent<Text>();
    }

    public void UpdateMoneyView(int moneyCount)
    {
        MoneyView.text = "Your Money: " + moneyCount;
    }
}
