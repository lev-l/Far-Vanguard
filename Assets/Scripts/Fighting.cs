using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighting : MonoBehaviour
{
    public float FightDelay;
    public Sprite[] ScinsForNums;
    public bool IsBloked;
    public int UnitsNum;
    private Transform _self;
    private (int x, int y) _gridPosition;
    private Movement _movement;
    private Castle _creator;
    private StandartAI _ai;
    private SpriteRenderer _renderer;

    void Start()
    {
        _self = transform.GetChild(0).GetComponent<Transform>();
        _movement = GetComponentInChildren<Movement>();
        _creator = GetComponentInChildren<Squad>().Creator;
        _ai = FindObjectOfType<StandartAI>();
        _renderer = GetComponentInChildren<SpriteRenderer>();

        StartCoroutine(SetScin());
        StartCoroutine(AIAttackingMessage());
    }

    public void SeeEnemy(Collider2D collision)
    {
        Squad otherSquad = collision.GetComponent<Squad>();
        if (!IsBloked && !otherSquad.GetComponentInParent<Fighting>().IsBloked)
        {
            if (otherSquad.Creator.City
                    != _creator.City)
            {
                Vector3 distance = _self.position - otherSquad.transform.position;
                Vector3 halfWay = distance / 2;
                Vector3 target = _self.position - halfWay;
                (int x, int y) targetOnGrid = Grid.VectorToGridPosition(target);

                while(!_movement.GoTo(targetOnGrid.x, targetOnGrid.y))
                {
                    targetOnGrid.x -= 1;
                }
                otherSquad.GetComponent<Movement>()
                        .GoTo(targetOnGrid.x, targetOnGrid.y);
                otherSquad.GetComponentInParent<Fighting>().IsBloked = true;

                StartCoroutine(Depart(Grid.VectorToGridPosition(GetComponentInChildren<Squad>()
                                                                        .transform.position)));
                otherSquad
                    .StartCoroutine(otherSquad.GetComponentInParent<Fighting>()
                                                .Depart(Grid.VectorToGridPosition
                                                        (
                                                        otherSquad.transform.position
                                                        )
                    ));

                IsBloked = true;
                Invoke("Unbloke", FightDelay);

                StartCoroutine(Batle(otherSquad.GetComponentInParent<Fighting>()));
            }
        }
        else
        {
            Invoke("Unbloke", FightDelay);
        }
    }

    public void Unbloke()
    {
        IsBloked = false;
    }

    public IEnumerator Depart((int x, int y) departPosition)
    {
        yield return new WaitForSeconds(FightDelay);

        GetComponentInChildren<Movement>().GoTo(departPosition.x,
                                        departPosition.y);
    }

    public IEnumerator Batle(Fighting other)
    {
        yield return new WaitForSeconds(FightDelay);

        int selfStrength = GetStrength();
        int otherStrength = other.GetStrength();

        int myEarlierNum = UnitsNum;
        (int unitsNum, int otherStrenght, int strength) statistic
            = FightWith(other.UnitsNum, otherStrength,
                        UnitsNum, selfStrength);
        UnitsNum = statistic.unitsNum;
        selfStrength = statistic.strength;
        otherStrength = statistic.otherStrenght;
        
        if (UnitsNum <= 0)
        {
            Destroy(transform.gameObject, 0.1f);
        }

        other.UnitsNum = FightWith(myEarlierNum, selfStrength,
                                    other.UnitsNum, otherStrength)
                        .unitsNum;
        if(other.UnitsNum <= 0)
        {
            Destroy(other.transform.gameObject, 0.1f);
        }
    }

    private IEnumerator SetScin()
    {
        while (true)
        {
            SetScinForThis();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator AIAttackingMessage()
    {
        if (!(_creator.Creator is StandartAI))
        {
            while (true)
            {
                _gridPosition = Grid.VectorToGridPosition(_self.position);

                (int x, int y)[] enemyTowns = GetEnemyTowns();
                (int x, int y) closestPosition = GetClosestPointFromArray(enemyTowns);

                (int x, int y) minDistance = Grid.GetDistance(_gridPosition, closestPosition);
                if (minDistance.x + minDistance.y
                    < 20)
                {
                    _ai.Attacked(closestPosition);
                }
                yield return new WaitForSecondsRealtime(2);
            }
        }
        yield return null;
    }

    private void SetScinForThis()
    {
        if (UnitsNum > 70)
        {
            _renderer.sprite = ScinsForNums[0];
        }
        else if (UnitsNum > 50)
        {
            _renderer.sprite = ScinsForNums[1];
        }
        else if (UnitsNum > 25)
        {
            _renderer.sprite = ScinsForNums[2];
        }
        else if (UnitsNum > 10)
        {
            _renderer.sprite = ScinsForNums[3];
        }
        else if(UnitsNum > 0)
        {
            _renderer.sprite = ScinsForNums[4];
        }
    }

    public static (int unitsNum, int otherStrenght, int strength)
            FightWith(int unitsNum, int strength,
            int selfUnitsNum, int selfStrength)
    {
        int notArmed = selfUnitsNum - selfStrength;
        if (strength > notArmed)
        {
            while (selfStrength != 0
                    && strength > notArmed)
            {
                strength -= 2;
                selfStrength -= 1;
            }
        }
        if(strength > selfUnitsNum)
        {
            return (0, strength, selfStrength);
        }

        selfUnitsNum -= strength;

        return (selfUnitsNum, strength, selfStrength);
    }

    public int GetStrength()
    {
        int min = Mathf.RoundToInt(Mathf.Pow((UnitsNum / 20), 2));
        if(min > UnitsNum)
        {
            min = 0;
        }
        int strength = Random.Range(min, UnitsNum + 1);

        return strength;
    }

    private (int x, int y)[] GetEnemyTowns()
    {
        List<(int x, int y)> enemyTownsPositions = new List<(int x, int y)>();
        foreach (KeyValuePair<(int x, int y), TownTag> town
                                                    in TownsContainer.Towns)
        {
            if (town.Value.Creator.City != _creator.City)
            {
                enemyTownsPositions.Add(town.Key);
            }
        }

        return enemyTownsPositions.ToArray();
    }

    private (int x, int y) GetClosestPointFromArray((int x, int y)[] array)
    {
        if (array.Length > 0)
        {
            (int x, int y) closestPosition = array[0];
            (int x, int y) minDistance = Grid.GetDistance(_gridPosition,
                                                        array[0]);
            foreach ((int x, int y) position in array)
            {
                (int x, int y) nextDistance = Grid.GetDistance(_gridPosition, position);
                if ((minDistance.x + minDistance.y)
                    > (nextDistance.x + nextDistance.y))
                {
                    minDistance = nextDistance;
                    closestPosition = position;
                }
            }

            return closestPosition;
        }
        return (100, 100);
    }
}
