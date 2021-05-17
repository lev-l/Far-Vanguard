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
    private Camera _camera;
    private ChoosedSquad _choosed;
    private ChoosedArmyPresenter _presenter;

    private void Start()
    {
        _camera = Camera.main;
        _presenter = GetComponent<ChoosedArmyPresenter>();
        SetNullChoosed();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            (int x, int y) gridMousePosition = Grid.VectorToGridPosition
                                                        (
                                                        _camera.ScreenToWorldPoint(Input.mousePosition)
                                                        );
            if (_choosed.Subject != null)
            {
                _choosed.Subject.GoTo(gridMousePosition.x,
                                        gridMousePosition.y);
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
            if(_choosed.Subject != null)
            {
                _choosed.Subject.GoTo();
            }
            _choosed = choose;
            _presenter.IsSelected(choose.Subject.gameObject);
            return true; 
        }
        return false;
    }

    private void SetNullChoosed()
    {
        _choosed = new ChoosedSquad(null, -1);
        _presenter.IsUnselected();
    }
}
