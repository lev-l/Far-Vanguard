using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#region States

public abstract class TownState : IState
{
    protected GameObject _selfMenu;
    protected GameObject _town;
    protected Vector3 _selfPosition;
    protected Text _nameText,
        _numOfSquadsText;
    protected Button _exitSquadButton;

    protected TownState(GameObject town,
                        GameObject menu)
    {
        _selfMenu = menu;

        _town = town;
        _selfPosition = town.transform.position;

        Text[] menuTexts = _selfMenu.GetComponentsInChildren<Text>();
        _nameText = menuTexts[1];
        _numOfSquadsText = menuTexts[0];

        _exitSquadButton = _selfMenu.GetComponentInChildren<Button>();
    }

    public virtual IState Next()
    {
        return this;
    }

    public virtual IState OnMouseClick(float x, float y)
    {
        throw new System.NotImplementedException();
    }

    protected bool IsClickedOn(float x, float y)
    {
        return (x + 0.11f > _selfPosition.x
                    && x - 0.11f < _selfPosition.x)
                && (y + 0.11f > _selfPosition.y
                    && y - 0.11f < _selfPosition.y);
    }
}

public class CalmTownState : TownState
{
    private Castle _creator;

    public CalmTownState(GameObject town,
                        GameObject menu)
                        : base(town, menu)
    {
        _creator = town.GetComponent<TownMoney>().Creator;
    }

    public override IState OnMouseClick(float x, float y)
    {
        if(IsClickedOn(x, y)
            && _creator.Creator is CentralCastle)
        {
            return new TownMenuState(_town, _selfMenu);
        }
        return this;
    }
}

public class TownMenuState : TownState
{
    private SquadsRoom _room;

    public TownMenuState(GameObject town,
                        GameObject menu)
                        : base(town, menu)
    {
        _nameText.text = _town.GetComponent<TownMenu>().Name;
        _room = _town.GetComponent<SquadsRoom>();
        OpenMenu();
    }

    public override IState Next()
    {
        _numOfSquadsText.text = _room.NumOfSquads.ToString();
        return this;
    }

    public override IState OnMouseClick(float x, float y)
    {
        CloseMenu();
        return new CalmTownState(_town, _selfMenu);
    }

    private void OpenMenu()
    {
        _selfMenu.SetActive(true);

        _exitSquadButton.onClick.RemoveAllListeners();
        _exitSquadButton.onClick.AddListener(delegate
        {
            _room.ExitSquad();
        });
    }

    private void CloseMenu()
    {
        _selfMenu.SetActive(false);
    }
}

#endregion

public static class TownsMenu
{
    public static GameObject Menu;
}

public class TownMenu : MonoBehaviour
{
    public GameObject NameField;
    public readonly string Name = NameCreator.CreateName();
    private GameObject _selfMenu;
    private Camera _camera;
    private IState _state;

    void Start()
    {
        Text text = Instantiate(NameField,
                            transform.position + new Vector3(0.2f, 0.2f),
                            Quaternion.identity)
                    .GetComponentInChildren<Text>();
        text.transform.parent.SetParent(transform);
        text.text = Name;
        _camera = Camera.main;
        text.transform.parent.GetComponent<Canvas>().worldCamera = _camera;

        _selfMenu = TownsMenu.Menu;

        _state = new CalmTownState(gameObject, _selfMenu);
    }

    void Update()
    {
        _state = _state.Next();

        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

            _state = _state.OnMouseClick(mousePosition.x, mousePosition.y);
        }
    }
}
