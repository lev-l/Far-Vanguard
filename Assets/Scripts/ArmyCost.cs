using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyCost : MonoBehaviour
{
    public int Cost;
    public int PaymentPeriodSpeed;
    private Castle _creator;

    void Start()
    {
        _creator = GetComponentInChildren<Squad>().Creator;
        StartCoroutine(Payment());
    }

    public IEnumerator Payment()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(PaymentPeriodSpeed);
            if (!_creator.Creator.Pay(Cost))
            {
                Destroy(gameObject);
            }
        }
    }
}
