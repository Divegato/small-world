using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class BuildControls : MonoBehaviour
{
    public float BuildDistance = 5; // TODO: This will be part of a player stat model

    public GameObject Cursor;
    private Light2D CursorLight;
    private TileCursor CursorTarget;

    private RecipeBook Recipes;
    private Recipe SelectedRecipe;
    private List<Tuple<KeyCode, Recipe>> RecipeMap;

    public SpriteAtlas Sprites;
    private Dictionary<string, Tile> Tiles;

    void Start()
    {
        Recipes = new RecipeBook();
        Tiles = Recipes.Recipes.ToDictionary(x => x.Key, x => CreateTile(x.Value.SpriteName));

        Cursor = Instantiate(Cursor);
        Cursor.SetActive(false);
        CursorLight = Cursor.GetComponent<Light2D>();

        // TODO: Make a UI for seeing and changing this mapping
        RecipeMap = new List<Tuple<KeyCode, Recipe>>
        {
            new Tuple<KeyCode, Recipe>(KeyCode.Alpha1, Recipes.Recipes["wall"]),
            new Tuple<KeyCode, Recipe>(KeyCode.Alpha2, Recipes.Recipes["forge"]),
            new Tuple<KeyCode, Recipe>(KeyCode.Alpha3, Recipes.Recipes["storage"]),
            new Tuple<KeyCode, Recipe>(KeyCode.Alpha4, Recipes.Recipes["solar"]),
            new Tuple<KeyCode, Recipe>(KeyCode.Alpha5, Recipes.Recipes["battery"])
        };
    }

    private Tile CreateTile(string spriteName)
    {
        var tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = Sprites.GetSprite(spriteName);
        tile.colliderType = Tile.ColliderType.Grid;

        return tile;
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            SelectBuildStructure();
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        var cursor = MoveCursor();

        if (cursor != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                TryActivateStructure(cursor);
            }
            else if (Input.GetMouseButton(0))
            {
                TryBuildStructureAtCursor(cursor);
            }
            else if (Input.GetMouseButton(1))
            {
                TryDeconstructAtCursor(cursor);
            }
        }
    }

    private TileCursor MoveCursor()
    {
        CursorTarget = null;

        var mouseWorldPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hit = Physics2D.Raycast(ray.origin, ray.direction, 10);
        var distance = (Vector2)transform.position - hit.point;

        if (hit && distance.magnitude < BuildDistance && hit.transform.gameObject.TryGetComponent<Deconstructable>(out var deconstructable))
        {
            var target = hit.transform.gameObject;
            var color = Color.cyan;

            if (target.TryGetComponent<Tilemap>(out var tileMap))
            {
                var center = tileMap.GetCellCenterWorld(Vector3Int.zero) - tileMap.transform.position;
                var tileCoordinate = tileMap.WorldToCell((Vector3)mouseWorldPos - center);
                var tileLocation = tileMap.GetCellCenterWorld(tileCoordinate);
                var localPosition = tileMap.CellToLocalInterpolated(tileCoordinate);

                var existingSprite = tileMap.GetSprite(tileCoordinate);
                CursorTarget = new TileCursor
                {
                    Map = tileMap,
                    Position = tileCoordinate,
                    CurrentSprite = existingSprite
                };

                Cursor.transform.parent = hit.transform;
                Cursor.transform.position = tileLocation;
                Cursor.transform.localRotation = Quaternion.identity;

                if (Input.GetMouseButton(1))
                {
                    color = Color.red;
                }

                CursorLight.color = color;
                Cursor.SetActive(true);

                Debug.DrawLine(transform.position, (Vector3)mouseWorldPos - center, color);
                Debug.DrawLine((Vector3)mouseWorldPos - center, tileLocation, color);
            }

            Debug.DrawLine(transform.position, mouseWorldPos, color);
            Debug.DrawLine(transform.position, hit.point, color);
            Debug.DrawLine(hit.point, hit.transform.position, color);
        }
        else
        {
            Cursor.SetActive(false);

            // TODO: consolodate a bit with the above block
            // TODO: render a ghost tile where we will be building
            var mousePoint = Input.mousePosition;
            mousePoint.z = 10f;
            var target = Camera.main.ScreenToWorldPoint(mousePoint);
            var buildDistance = (Vector2)transform.position - (Vector2)target;

            if (buildDistance.magnitude < BuildDistance)
            {
                var targets = Physics2D.OverlapCircleAll(transform.position, BuildDistance);

                Tilemap tileMap = null;
                var found = targets.Any(x => x.gameObject.TryGetComponent(out tileMap));
                if (found)
                {
                    Vector3Int tileCoordinate = tileMap.WorldToCell(target);
                    CursorTarget = new TileCursor
                    {
                        Map = tileMap,
                        Position = tileCoordinate
                    };
                }
            }
        }

        return CursorTarget;
    }

    private void SelectBuildStructure()
    {
        if (RecipeMap != null)
        {
            foreach (var map in RecipeMap)
            {
                if (Input.GetKeyDown(map.Item1))
                {
                    SelectedRecipe = map.Item2;
                }
            }
        }
    }

    private void TryActivateStructure(TileCursor cursor)
    {
        if (cursor.CurrentSprite != null)
        {
            if (TryGetComponent<BuildNetwork>(out var network))
            {
                network.ActivateStructure(cursor.Map, (Vector2Int)cursor.Position);
            }
        }
    }

    private void TryDeconstructAtCursor(TileCursor cursor)
    {
        if (cursor.CurrentSprite != null)
        {
            if (TryGetComponent<BuildNetwork>(out var network))
            {
                var structure = network.GetStructure(cursor.Map, (Vector2Int)cursor.Position);
                if (structure == null)
                {
                    // TODO: Determine material by tile
                    if (network.TryAddMaterial(new MaterialQuantity { Id = "stone", Quantity = 1 }))
                    {
                        cursor.Map.SetTile(cursor.Position, null);
                    }
                }
                else
                {
                    if (network.TryRemoveStructure(cursor.Map, (Vector2Int)cursor.Position))
                    {
                        cursor.Map.SetTile(cursor.Position, null);
                    }
                }
            }

            // Reduce target mass
        }
    }

    private void TryBuildStructureAtCursor(TileCursor cursor)
    {
        if (cursor.CurrentSprite == null)
        {
            if (SelectedRecipe != null)
            {
                // TODO: Make sure to not build on top of self buildDistance.magnitude > 2.1
                BuildStructure(cursor.Map, cursor.Position, SelectedRecipe);
            }
        }
    }

    private void BuildStructure(Tilemap tileMap, Vector3Int tileCoordinate, Recipe recipe)
    {
        if (TryGetComponent<BuildNetwork>(out var network))
        {
            if (network.TrySpend(recipe))
            {
                if (recipe.StructureClass != null)
                {
                    Structure structure = (Structure)Activator.CreateInstance(recipe.StructureClass);
                    structure.Parent = tileMap;
                    structure.GridLocation = (Vector2Int)tileCoordinate;
                    structure.Definition = recipe;
                    network.AddStructure(structure);
                }

                tileMap.SetTile(tileCoordinate, Tiles[recipe.Id]);

                // Increase target mass
            }
        }
    }
}
