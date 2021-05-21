using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ChoosedSquad
{
    public Movement Subject { get; private set; }
    public int Priority { get; private set; }

    public ChoosedSquad(Movement subject, int priority)
    {
        Subject = subject;
        Priority = priority;
    }
}

public class ChooseSquad : MonoBehaviour
{
    [SerializeField] private Texture2D SetTargetCursor,
                                        StandartCursor;
    
    private Camera _camera;
    private ChoosedSquad _choosed;
    private ChoosedArmyPresenter _presenter;
    private bool moving; // do not use
    private bool _moving
    {
        set
        {
            moving = value;
            if (value)
            {
                Cursor.SetCursor(SetTargetCursor, Vector2.zero, CursorMode.ForceSoftware);
            }
            else
            {
                Cursor.SetCursor(StandartCursor, Vector2.zero, CursorMode.ForceSoftware);
            }
        }
        get
        {
            return moving;
        }
    }

    private void Start()
    {
        _moving = false;
        _camera = Camera.main;
        _presenter = GetComponent<ChoosedArmyPresenter>();
        SetNullChoosed();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)
            && _moving)
        {
            _moving = false;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            _moving = true;
        }

        if (Input.GetMouseButtonDown(0) && _moving)
        {
            (int x, int y) gridMousePosition = Grid.VectorToGridPosition
                                                        (
                                                        _camera.ScreenToWorldPoint(Input.mousePosition)
                                                        );
            if (_choosed.Subject != null)
            {
                _choosed.Subject.GoTo(gridMousePosition.x,
                                        gridMousePosition.y);
                _moving = false;
            }
            else
            {
                SetNullChoosed();
            }
        }
    }

    public bool Choose(ChoosedSquad choose)
    {
        if (choose.Subject == _choosed.Subject)
        {
            SetNullChoosed();
            return false;
        }
        if(choose.Priority >= _choosed.Priority)
        {
            if(_moving
                && _choosed.Subject != null)
            {
                _choosed.Subject.GoTo();
            }
            _choosed = choose;
            _presenter.IsSelected(choose.Subject.gameObject);
            return true; 
        }
        return false;
    }

    public void Unchoose(Movement subject)
    {
        if(subject == _choosed.Subject)
        {
            SetNullChoosed();
        }
    }

    private void SetNullChoosed()
    {
        _moving = false;
        _choosed = new ChoosedSquad(null, -1);
        _presenter.IsUnselected();
    }
}
