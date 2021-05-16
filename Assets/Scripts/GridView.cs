using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GridView : MonoBehaviour
{
    public GameObject CellView;
    public float Spacing;
    public GameObject[,] ObjectsMatrix;
    private Dictionary<string, Sprite> _cornerSprites;
    private Dictionary<Structs, string> _landCorners;
    private GridSystem _grid;
    private Sprites _sprites;

    private void Awake()
    {
        _cornerSprites = new Dictionary<string, Sprite>();
        _cornerSprites.Add("desertCorner", Resources.Load<Sprite>("Sprites/DesertCorner"));
        _cornerSprites.Add("seaCorner", Resources.Load<Sprite>("Sprites/SeaCorner"));
        _cornerSprites.Add("fatlandCorner", Resources.Load<Sprite>("Sprites/FatLandCorner"));

        _landCorners = new Dictionary<Structs, string>();
        _landCorners.Add(Structs.Desert, "desertCorner");
        _landCorners.Add(Structs.Sea, "seaCorner");
        _landCorners.Add(Structs.FatLand, "fatlandCorner");
    }

    private void Start()
    {
        _grid = GetComponent<GridSystem>();
        _sprites = GetComponent<Sprites>();

        ObjectsMatrix = new GameObject[_grid.width, _grid.height];
        for(int x = 0; x < _grid.width; x++)
        {
            for (int y = 0; y < _grid.height; y++)
            {
                ObjectsMatrix[x, y] = Instantiate(CellView,
                            new Vector3(Spacing * x, Spacing * y),
                            Quaternion.identity);
                ObjectsMatrix[x, y].GetComponent<SpriteRenderer>()
                                        .sortingOrder = -3;

                if (_grid.grid.Cells[x, y].Items["land"] != Structs.Grass)
                {
                    Structs landType = _grid
                                        .grid
                                        .Cells[x, y]
                                        .Items["land"];

                    bool typicalLandscape = true;
                    foreach(Object cell in _grid.grid.GetNearestCells(x, y))
                    {
                        if(cell.Items["land"] != landType)
                        {
                            typicalLandscape = false;
                        }
                    }
                    if (typicalLandscape)
                    {
                        Destroy(ObjectsMatrix[x, y]);
                    }

                    ObjectsMatrix[x, y] = Instantiate(CellView,
                                            new Vector3(Spacing * x, Spacing * y),
                                            Quaternion.identity);
                    ObjectsMatrix[x, y].GetComponent<SpriteRenderer>().sprite
                                    = _sprites.StructsSprites[(int)landType];
                }
            }
        }

        for(int x = 1; x < _grid.width - 1; x++)
        {
            for(int y = 1; y < _grid.height - 1; y++)
            {
                if (_grid.grid.Cells[x, y].Items["land"] != Structs.Grass)
                {
                    Landscape landscape = _grid.grid.GetNearestLand(x, y);

                    SpriteRenderer renderer = ObjectsMatrix[x, y].GetComponent<SpriteRenderer>();
                    Structs landType = _grid.grid.Cells[x, y].Items["land"];

                    SetupAngle(landscape.Down, landscape.Left,
                                _grid.grid.Cells[x, y],
                                renderer, _cornerSprites[_landCorners[landType]]);

                    SetupAngle(landscape.Down, landscape.Right,
                                _grid.grid.Cells[x, y],
                                renderer, _cornerSprites[_landCorners[landType]]);

                    SetupAngle(landscape.Up, landscape.Left,
                                _grid.grid.Cells[x, y],
                                renderer, _cornerSprites[_landCorners[landType]]);

                    SetupAngle(landscape.Up, landscape.Right,
                                _grid.grid.Cells[x, y],
                                renderer, _cornerSprites[_landCorners[landType]]);
                }
            }
        }

        if (_grid.grid.Cells.Length != ObjectsMatrix.Length)
        {
            throw new System.Exception("Wrong to create Objects Matrix");
        }
    }

    private (bool x, bool y) GetFliping(Object first, Object second,
                                        Object self)
    {
        (bool x, bool y) fliping = (false, false);

        int firstXDirection = self.x - first.x;
        int firstYDirection = self.y - first.y;
        int secondXDirection = self.x - second.x;
        int secondYDirection = self.y - second.y;

        fliping.x = firstXDirection < 0 || secondXDirection < 0;
        fliping.y = firstYDirection < 0 || secondYDirection < 0;

        return fliping;
    }

    private void SetupAngle(Object corner1, Object corner2,
                            Object self, SpriteRenderer renderer,
                            Sprite sprite)
    {
        if(corner1.Items["land"] != self.Items["land"]
            && corner2.Items["land"] != self.Items["land"])
        {
            renderer.sprite = sprite;

            (bool x, bool y) flip = GetFliping(corner1, corner2, self);
            renderer.flipX = flip.x;
            renderer.flipY = flip.y;
        }
    }

    public void UpdateViewIn(int x, int y)
    {
        if (_grid.grid.Cells[x, y].Items["town"] != Structs.None)
        {
            SetObjectSprite((x, y), "town");
        }
        else
        {
            SetObjectSprite((x, y), "land");
        }
    }

    private void SetObjectSprite((int x, int y) position, string spriteType)
    {
        ObjectsMatrix[position.x, position.y].GetComponent<SpriteRenderer>().sprite =
            _sprites.StructsSprites[(int)_grid.grid.Cells[position.x, position.y]
                                                                .Items[spriteType]];
    }
}
