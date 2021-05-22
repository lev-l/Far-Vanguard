using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region States

public enum Direction
{
    Direct,
    Angle
}

public interface IState
{
    IState Next();

    IState OnMouseClick(float x, float y);
}

public abstract class MovementState : IState
{
    protected ChooseSquad _chooser;
    protected Transform _self;
    protected TargetsStackContainer _targets;
    protected Castle _creator;
    protected Movement _parent;

    protected void MessageSquad()
    {
        Squad selfSquad = _self.gameObject
                            .GetComponent<Squad>();
        if (selfSquad)
        {
            selfSquad.TargetAchived();
        }
    }

    protected MovementState(Transform self,
                            TargetsStackContainer targets,
                            Castle creator)
    {
        _chooser = GameObject.FindGameObjectWithTag("Chooser")
                                            .GetComponent<ChooseSquad>();
        _self = self;
        _targets = targets;
        _creator = creator;
        _parent = _self.GetComponent<Movement>();
    }

    public virtual IState Next()
    {
        throw new System.NotImplementedException();
    }

    public virtual IState OnMouseClick(float x, float y)
    {
        return this;
    }
}

public class CalmState : MovementState
{
    public CalmState(Transform self,
                    TargetsStackContainer targets,
                    Castle creator)
                        : base(self, targets, creator)
    {
        MessageSquad();
    }

    public override IState Next()
    {
        return this;
    }

    public override IState OnMouseClick(float x, float y)
    {
        if (_creator.Creator is CentralCastle)
        {
            if ((x + 0.11f > _self.position.x && x - 0.11f < _self.position.x)
            && (y + 0.11f > _self.position.y && y - 0.11f < _self.position.y))
            {
                _chooser.Choose(new ChoosedSquad(_parent, 1));
            }
            else
            {
                return this;
            }
        }
        return this;
    }
}

public class MoveState : MovementState
{
    private GridSystem _gridSystem;
    private Vector2 _currentTarget;

    public Vector2 GetTarget()
    {
        return _currentTarget;
    }

    public MoveState(Transform self,
                    TargetsStackContainer targets,
                    Castle creator)
                        : base(self, targets, creator)
    {
        _gridSystem = MonoBehaviour.FindObjectOfType<GridSystem>();
        PickUpTarget();

        (int x, int y)[] wayTurns = GetTurns(
                                            FindPath(
                                                    Grid.VectorToGridPosition(_currentTarget)
                                                    )
                                            );
        _targets.targets.AddRange(wayTurns);

        PickUpTarget();
    }

    public override IState Next()
    {
        _self.position = Vector2.MoveTowards(_self.position,
                                            _currentTarget,
                                            Time.deltaTime);

        if(Vector3To2(_self.position) == _currentTarget)
        {
            if (_targets.targets.Count > 0)
            {
                PickUpTarget();
                return this;
            }
            else
            {
                return new CalmState(_self, _targets, _creator);
            }
        }
        else
        {
            return this;
        }
    }

    private void PickUpTarget()
    {
        _currentTarget = Grid.GridPositionToVector(_targets.targets[0]);
        _targets.targets.Remove(Grid.VectorToGridPosition(_currentTarget));
    }

    public (int x, int y)[] GetTurns((int x, int y)[] way)
    {
        List<(int x, int y)> turns = new List<(int x, int y)>();

        (int x, int y)[] previous2Cells = new (int x, int y)[2];
        previous2Cells[0] = way[0];
        previous2Cells[1] = way[0];
        foreach((int x, int y) cell in way)
        {
            (int x, int y) distance1 = GetDistance(previous2Cells[0],
                                                                cell);
            (int x, int y) distance2 = GetDistance(previous2Cells[1],
                                                    previous2Cells[0]);

            if(GetTypeOfDirection(distance1)
                == GetTypeOfDirection(distance2))
            {
                previous2Cells[1] = previous2Cells[0];
                previous2Cells[0] = cell;
                continue;
            }
            else
            {
                turns.Add(previous2Cells[0]);
                previous2Cells[1] = previous2Cells[0];
                previous2Cells[0] = cell;
                continue;
            }

            throw new System.Exception("there is no update to next cell strings");
        }
        // add last cell in targets
        turns.Add(way[way.Length - 1]);

        return turns.ToArray();
    }

    private Direction GetTypeOfDirection((int x, int y) direction)
    {
        if(direction.x != 0 && direction.y != 0)
        {
            return Direction.Angle;
        }
        else
        {
            return Direction.Direct;
        }
    }

    private (int x, int y) GetDistance((int x, int y) cell1, (int x, int y) cell)
    {
        (int x, int y) distance = (0, 0);
        distance.x = Mathf.Abs(cell1.x - cell.x);
        distance.y = Mathf.Abs(cell1.y - cell.y);

        return distance;
    }

    public (int x, int y)[] FindPath((int x, int y) target)
    {
        List<(int x, int y)> closedCells = new List<(int x, int y)>();
        List<(int x, int y)> way = new List<(int x, int y)>();
        (int x, int y) currentPosition = Grid.VectorToGridPosition(_self.position);
        (int x, int y) startPosition = currentPosition;
        Dictionary<(int x, int y), (int x, int y)> directions =
                            new Dictionary<(int x, int y), (int x, int y)>();

        // odd variable
        (int x, int y) currentCell;
        // while we are not achived target
        while (!directions.TryGetValue(target, out currentCell))
        {
            currentPosition = NextCell(target, currentPosition, directions, closedCells);
        }

        currentCell = target;
        // add the target to way
        way.Add(currentCell);
        while (currentCell != startPosition)
        {
            // go by directions from target to start and remember way
            way.Add(directions[currentCell]);
            currentCell = directions[currentCell];
        }

        return Roll(way.ToArray());
    }

    private (int x, int y)[] Roll((int x, int y)[] array)
    {
        (int x, int y)[] returnArray = new (int x, int y)[array.Length];

        int rollIndex = returnArray.Length - 1;
        for (int i = 0; i < returnArray.Length; i++)
        {
            returnArray[i] = array[rollIndex];
            rollIndex--;
        }
        return returnArray;
    }

    public (int x, int y) NextCell((int x, int y) target,
                                       (int x, int y) currentPosition,
                                       Dictionary<(int x, int y), (int x, int y)> directions,
                                       List<(int x, int y)> closed)
    {
        Object[] rawNeighbors = _gridSystem.grid.GetNearestCells(currentPosition.x,
                                                            currentPosition.y);
        List<Object> neighbors = new List<Object>();
        foreach(Object neighbor in rawNeighbors)
        {
            if(!closed.Contains((neighbor.x, neighbor.y))
                && neighbor.Items["land"] != Structs.Sea)
            {
                neighbors.Add(neighbor);
            }
        }

        int[] values = new int[neighbors.Count];

        for (int i = 0; i < neighbors.Count; i++)//get distance from target for each neighbor
        {
            values[i] = Mathf.Abs(target.x - neighbors[i].x)
                        + Mathf.Abs(target.y - neighbors[i].y);

            // odd variable
            (int x, int y) newWay;
            if (!directions.TryGetValue((neighbors[i].x, neighbors[i].y),
                                                    out newWay))
            {
                directions.Add((neighbors[i].x, neighbors[i].y), currentPosition);
            }
        }

        // find min distance to target
        int min = Mathf.Min(values);
        int indexOfMin = 0;
        for (int i = 0; i < values.Length; i++)
        {
            if (min == values[i])
            {
                indexOfMin = i;
            }
        }

        if(neighbors.Count == 0)
        {
            Debug.Log("Can`t go to this place, sir!");
            return Grid.VectorToGridPosition(_self.position);
        }
        currentPosition = (neighbors[indexOfMin].x, neighbors[indexOfMin].y);
        // don`t consider cells, in what we has ever been
        closed.Add(currentPosition);
        return currentPosition;
    }

    public override IState OnMouseClick(float x, float y)
    {
        return base.OnMouseClick(x, y);
    }

    public static Vector2 Vector3To2(Vector3 vector3)
    {
        Vector2 vector2 = new Vector2(vector3.x,
                                    vector3.y);
        return vector2;
    }
}
#endregion

public class TargetsStackContainer
{
    public List<(int x, int y)> targets;

    public TargetsStackContainer(List<(int x, int y)> targets)
    {
        this.targets = targets;
    }
    public TargetsStackContainer(Vector2 target)
    {
        List<(int x, int y)> targets = new List<(int x, int y)>();
        targets.Add(Grid.VectorToGridPosition(target));

        this.targets = targets;
    }
}

public class TargetContainer
{
    public Vector2 target;

    public TargetContainer(Vector2 target)
    {
        this.target = target;
    }
}


public class Movement : MonoBehaviour
{
    protected Transform _self;
    protected TargetsStackContainer _target;
    private Camera _camera;
    private ChooseSquad _chooser;
    private Grid _grid;
    private Castle _creator;
    private IState _state;

    private void Start()
    {
        _camera = Camera.main;
        _chooser = FindObjectOfType<ChooseSquad>();
        _self = GetComponent<Transform>();
        _target = new TargetsStackContainer(_self.position);
        _grid = FindObjectOfType<GridSystem>().grid;

        _creator = GetComponent<Squad>().Creator;
        _state = new CalmState(_self, _target, _creator);
    }

    private void Update()
    {
        _state = _state.Next();

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 cursorPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            _state = _state.OnMouseClick(cursorPosition.x, cursorPosition.y);
        }
        if (Input.GetMouseButtonDown(1))
        {
            _chooser.Unchoose(this);
        }
    }

    public bool GoTo(int x, int y)
    {
        if (_grid.Cells[x, y].Items["land"] != Structs.Sea)
        {
            _state = new MoveState(_self, new TargetsStackContainer(
                                                                Grid.GridPositionToVector((x, y))
                                                                    ),
                                                                _creator
                );
            return true;
        }
        return false;
    }

    public void GoTo()
    {
        _state = new CalmState(_self, new TargetsStackContainer(_self.position), _creator);
    }
}
