using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#region States

public abstract class CastleState : IState
{
    protected Vector3 _selfPosition;
    protected GameObject _selfMenu;
    protected Button[] _buttons;

    public CastleState(Vector3 selfPosition,
                            GameObject selfMenu,
                            Button[] buttons)
    {
        _selfPosition = selfPosition;
        _selfMenu = selfMenu;
        _buttons = buttons;
    }

    public virtual IState Next()
    {
        return this;
    }

    public virtual IState OnMouseClick(float x, float y)
    {
        return this;
    }

    protected bool IsClickedOn(float x, float y)
    {
        return (x + 0.11f > _selfPosition.x
                    && x - 0.11f < _selfPosition.x)
                && (y + 0.11f > _selfPosition.y
                    && y - 0.11f < _selfPosition.y);
    }
}

public class CalmCastleState : CastleState
{
    public CalmCastleState(Vector3 selfPosition,
        GameObject selfMenu,
        Button[] buttons) : base(selfPosition, selfMenu, buttons)
    {
    }

    public override IState OnMouseClick(float x, float y)
    {
        if (IsClickedOn(x, y))
        {
            return new MenuState(_selfPosition, _selfMenu, _buttons);
        }
        return this;
    }
}

public class MenuState : CastleState
{
    private CentralCastle _castle;
    private bool _isBuilding;
    private Text _numOfSquadsText;
    private SquadsRoom _room;

    public MenuState(Vector3 selfPosition,
        GameObject selfMenu,
        Button[] buttons) : base(selfPosition, selfMenu, buttons)
    {
        _castle = MonoBehaviour.FindObjectOfType<CentralCastle>();
        _room = _castle.GetComponent<SquadsRoom>();
        _numOfSquadsText = _buttons[2]
                                .transform
                                .parent
                                .GetComponentInChildren<Text>();
        OpenMenu();
    }

    public override IState Next()
    {
        _numOfSquadsText.text = _room.NumOfSquads.ToString();
        if (_isBuilding && _castle.CanBuild())
        {
            return new BuildingState(_selfPosition,
                                    _selfMenu,
                                    _buttons,
                                    _castle);
        }
        return this;
    }

    public override IState OnMouseClick(float x, float y)
    {
        CloseMenu();
        return new CalmCastleState(_selfPosition,
                                    _selfMenu,
                                    _buttons);
    }

    public void SetBuildingMode()
    {
        CloseMenu();
        _isBuilding = true;
    }

    protected void CloseMenu()
    {
        _selfMenu.SetActive(false);
    }

    public void InstantiateArmy()
    {
        if (_castle.CanTrain())
        {
            GameObject newArmy = MonoBehaviour.Instantiate(
                                                Resources.Load<GameObject>("Soldier"),
                                                _selfPosition,
                                                Quaternion.identity);
            _castle.AddFlagTo(newArmy.GetComponentInChildren<Squad>().transform);

            newArmy.GetComponentInChildren<Squad>().Creator = new Castle(_castle,
                                                        _castle.gameObject);
        }
    }

    private void OpenMenu()
    {
        _selfMenu.SetActive(true);

        _buttons[0].onClick.RemoveAllListeners();
        _buttons[0].onClick.AddListener(delegate { SetBuildingMode(); });
        _buttons[0].GetComponentInChildren<Text>().text = "Builder";

        _buttons[1].onClick.RemoveAllListeners();
        _buttons[1].onClick.AddListener(delegate { InstantiateArmy(); });
        _buttons[1].GetComponentInChildren<Text>().text = "Squad";

        _buttons[2].onClick.RemoveAllListeners();
        _buttons[2].onClick.AddListener(delegate { _castle.
                                                    GetComponent<SquadsRoom>()
                                                    .ExitSquad(); });
    }
}

public class BuildingState : CastleState
{
    private GameObject BuiderPrefub;
    private GridSystem _gridSystem;
    private CentralCastle _centralCastle;

    public BuildingState(Vector3 selfPosition,
        GameObject selfMenu,
        Button[] buttons,
        CentralCastle castle) : base(selfPosition, selfMenu, buttons)
    {
        BuiderPrefub = Resources.Load<GameObject>("Builder");
        _gridSystem = GameObject.Find("Grid").GetComponent<GridSystem>();
        _centralCastle = castle;
    }

    public override IState OnMouseClick(float x, float y)
    {
        (int x, int y) position = Grid.VectorToGridPosition(new Vector3(x, y));

        if (_gridSystem.grid.Cells[position.x, position.y].Items["land"] == Structs.Sea)
        {
            _centralCastle.GiveMoney(50);
            return new CalmCastleState(_selfPosition,
                                        _selfMenu,
                                        _buttons);
        }
        GameObject newBuilder = MonoBehaviour.Instantiate(BuiderPrefub,
            _selfPosition, Quaternion.identity);
        _centralCastle.AddFlagTo(newBuilder.transform);

        newBuilder.GetComponent<Builder>()
            .SetParams(_gridSystem,
                        Grid.RoundVectorToVectorOnGrid(new Vector3(x, y)),
                        new Castle(_centralCastle, _centralCastle.gameObject),
                        _centralCastle.GetCosts().forBuild);

        return new CalmCastleState(_selfPosition,
                                    _selfMenu,
                                    _buttons);
    }
}

#endregion

public static class TownsContainer
{
    public static Dictionary<(int x, int y), TownTag> Towns =
                new Dictionary<(int x, int y), TownTag>();
}

public class CentralCastle : MonoBehaviour, ICashTaker
{
    public GameObject Flag;
    public GameObject CastleMenu { get; private set; }
    [SerializeField] private int _trainCost;
    [SerializeField] private int _buildCost;
    [SerializeField] private int Cash;
    private Vector3 _selfPosition;
    private Button[] _buttons;
    private Camera _camera;
    private MoneyPresenter _presenter;
    private IState _currentState;

    private void Start()
    {
        _selfPosition = GetComponent<Transform>().position;
        _camera = Camera.main;
        _presenter = FindObjectOfType<MoneyPresenter>();

        CastleMenu = GameObject.FindGameObjectWithTag("CastleMenuPanel");
        _buttons = CastleMenu.GetComponentsInChildren<Button>();
        CastleMenu.SetActive(false);

        _currentState = new CalmCastleState(_selfPosition,
                                            CastleMenu,
                                            _buttons);

        AddFlagTo(transform);
    }

    private void Update()
    {
        _currentState = _currentState.Next();
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            _currentState = _currentState.OnMouseClick(mousePosition.x,
                                                        mousePosition.y);
        }
    }

    public void AddFlagTo(Transform subject)
    {
        GameObject flag = MonoBehaviour.Instantiate(Flag);
        flag.transform.SetParent(subject);
        flag.transform.localPosition = Vector3.zero;
    }

    public int GiveMoney(int amount)
    {
        if(amount < 0 && Cash + amount < 0)
        {
            Cash = 0;
            _presenter.UpdateMoneyView(Cash);
            return amount;
        }
        Cash += amount;
        _presenter.UpdateMoneyView(Cash);
        return 0;
    }

    public bool Pay(int cost)
    {
        if(cost <= Cash)
        {
            Cash -= cost;
            _presenter.UpdateMoneyView(Cash);
            return true;
        }
        return false;
    }

    public bool CanTrain()
    {
        return Pay(_trainCost);
    }

    public bool CanBuild()
    {
        return Pay(_buildCost);
    }

    public (int forTrain, int forBuild) GetCosts()
    {
        return (_trainCost, _buildCost);
    }
}
