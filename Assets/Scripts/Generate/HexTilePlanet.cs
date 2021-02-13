using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class HexTilePlanet
{
    private const float HexSize = 1.28f;
    private const int chunkRadius = 8;

    private const double Roughness = 0.5;
    public static double min = double.MaxValue;
    public static double max = double.MinValue;

    // TODO: Break into chunks
    public static void Generate(float radius, Transform parent)
    {
        var points = Geometry.GetCircle(radius * 0.9f, 100);
        var core = GeneratePlanet.GenerateShape("Core", "Background", points, parent);
        core.transform.localPosition = Vector3.forward;

        // Determine amount of chunks
        var intRadius = Mathf.FloorToInt(radius);
        var radiusPowerOfTwo = Mathf.NextPowerOfTwo(intRadius * 2);

        // Generate terrain
        var sprites = Resources.LoadAll<Sprite>("Tilemap/terrain-tiles");
        var tiles = sprites.Select(x =>
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = x;
            tile.colliderType = Tile.ColliderType.Grid;

            return tile;
        }).ToArray();

        var heightMap = new DiamondSquare(radiusPowerOfTwo, Roughness, UnityEngine.Random.value).getData();
        var tileIndex = UnityEngine.Random.Range(0, tiles.Length);

        var chunkArea = HexMath.GetHexArea(chunkRadius);
        var shift = HexMath.GetHexShiftForChunks(chunkRadius);

        var chunkDictionary = new Dictionary<string, Tilemap>();

        for (int i = 0; i < heightMap.GetLength(0); i++)
        {
            for (int j = 0; j < heightMap.GetLength(1); j++)
            {
                var height = heightMap[i, j];
                if (height < min)
                {
                    min = height;
                }
                if (height > max)
                {
                    max = height;
                }
            }
        }

        for (int x = 0; x < radiusPowerOfTwo; x++)
        {
            for (int y = 0; y < radiusPowerOfTwo; y++)
            {
                var xCoord = x - intRadius;
                var yCoord = y - intRadius;

                var row = (xCoord * -1) - yCoord;
                var col = xCoord + (row - (row & 1)) / 2;

                var cubeCoordinate = HexMath.ConvertOffsetToCube(row, col);

                var actualPosition = new Vector2(HexSize * Mathf.Sqrt(3) * (col + 0.5f * (row & 1)), HexSize * 3 / 2 * row);

                if (actualPosition.magnitude < radius)
                {
                    var index = (int)Math.Floor((heightMap[row + intRadius, col + intRadius] + Roughness) * Roughness * (tiles.Length));
                    if (index >= 0 && index < tiles.Length) // Index out of range is caves
                    {
                        try
                        {
                            var chunkCoord = HexMath.HexToChunk(cubeCoordinate, chunkArea, shift);
                            var id = chunkCoord.ToString();

                            if (!chunkDictionary.ContainsKey(id))
                            {
                                var body = new GameObject { name = "Chunk-" + id, tag = "Planet" };
                                body.transform.parent = core.transform;
                                body.transform.localPosition = Vector3.back; // TODO: Set chunk offset

                                body.AddComponent<Deconstructable>();

                                var grid = body.AddComponent<Grid>();
                                grid.cellLayout = GridLayout.CellLayout.Hexagon;
                                grid.cellSize = new Vector3(Mathf.Sqrt(3) * HexSize, 2 * HexSize, 1);

                                var tileMap = body.AddComponent<Tilemap>();
                                tileMap.tileAnchor = Vector3.zero;

                                var renderer = body.AddComponent<TilemapRenderer>();

                                chunkDictionary[id] = tileMap;
                            }

                            chunkDictionary[id].SetTile(new Vector3Int(col, row, 0), tiles[index]);

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                    else
                    {
                        Debug.LogError("Index: " + index + " is out of range.");
                    }
                }
            }
        }

        foreach (var chunk in chunkDictionary.Values.Select(x => x.gameObject))
        {
            var gravity = chunk.AddComponent<GravitySource>();
            // TODO: Set gravity based on number (and mass?) of tiles
            gravity.GravityPower = chunkArea;

            var rigidBody = chunk.AddComponent<Rigidbody2D>();
            rigidBody.bodyType = RigidbodyType2D.Kinematic;
            rigidBody.mass = chunkArea;
            rigidBody.gravityScale = 0;
            rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var tileCollider = chunk.AddComponent<TilemapCollider2D>();
            tileCollider.usedByComposite = true;

            var collider = chunk.AddComponent<CompositeCollider2D>();
            collider.geometryType = CompositeCollider2D.GeometryType.Polygons;
        }
    }
}
