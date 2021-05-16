using UnityEngine;

public interface ICashTaker
{
    int GiveMoney(int amount);
    bool Pay(int cost);
    bool CanTrain();
    bool CanBuild();
    void AddFlagTo(Transform subject);
}