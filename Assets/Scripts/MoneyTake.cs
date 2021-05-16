    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISimpleState
{
    ISimpleState Next();
}

public abstract class MoneyTakerState : ISimpleState
{
    protected MoveState _move;
    protected Transform _self;
    protected RotateToTarget _rotater;
    protected MoneyTake _moneyTake;

    protected MoneyTakerState(Transform self, MoneyTake moneyTake,
                                MoveState move, RotateToTarget rotater)
    {
        _move = move;
        _self = self;
        _moneyTake = moneyTake;
        _rotater = rotater;
    }

    public virtual ISimpleState Next()
    {
        return this;
    }
}

public class ToCastle : MoneyTakerState
{
    public ToCastle(Transform self, MoneyTake moneyTake,
                    MoveState move, RotateToTarget rotater)
                                        : base(self, moneyTake, move, rotater)
    {
    }

    public override ISimpleState Next()
    {
        if(!(_move.Next() is MoveState))
        {
            _moneyTake.Money = _moneyTake.Creator.GiveMoney(_moneyTake.Money);
            if(_moneyTake.Money < 0)
            {
                _self.GetComponentInParent<TownFight>().DestroySelf();
            }
            _move = new MoveState(_self, new TargetsStackContainer(
                                                _moneyTake.MyTownPosition
                                                ),
                                        _moneyTake.Castle
                                );
            return new ToTown(_self, _moneyTake, _move, _rotater);
        }
        else
        {
            _rotater.Rotate(_move.GetTarget());
            return this;
        }
    }
}

public class ToTown : MoneyTakerState
{
    public ToTown(Transform self, MoneyTake moneyTake,
                    MoveState move, RotateToTarget rotater)
                                        : base(self, moneyTake, move, rotater)
    {
    }

    public override ISimpleState Next()
    {
        if (!(_move.Next() is MoveState))
        {
            int getedMoney = _moneyTake.TownMoney.GiveMoney();

            if (getedMoney != 0)
            {
                _moneyTake.Money = getedMoney;
            }

            _move = new MoveState(_self, new TargetsStackContainer(
                                            _moneyTake.CenterCastlePostion),
                                            _moneyTake.Castle
                                 );
            return new ToCastle(_self, _moneyTake, _move, _rotater);
        }
        else
        {
            _rotater.Rotate(_move.GetTarget());
            return this;
        }
    }
}

public class MoneyTake : MonoBehaviour
{
    public Castle Castle;
    public TownMoney TownMoney { get; private set; }
    public ICashTaker Creator { get; private set; }
    public Vector2 CenterCastlePostion { get; private set; }
    public Vector2 MyTownPosition { get; private set; }
    public int Money;
    private Transform _self;
    private TargetContainer _target;
    private ISimpleState _state;

    void Start()
    {
        _self = GetComponent<Transform>();
        _target = new TargetContainer(_self.position);
        _state = new ToCastle(_self,
                                this,
                                new MoveState(_self,
                                            new TargetsStackContainer(_target.target),
                                            Castle),
                                GetComponent<RotateToTarget>());

        CenterCastlePostion = Castle.City.transform.position;
        Creator = Castle.Creator;

        TownMoney = transform.parent.GetComponent<TownMoney>();
        MyTownPosition = transform.parent.position;
    }

    void Update()
    {
        _state = _state.Next();
    }
}
