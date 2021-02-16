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

    public static void Generate(float radius, Transform parent)
    {
        // Generate core background
        var points = Geometry.GetCircle(radius * 0.9f, 100);
        var core = GeneratePlanet.GenerateShape("Core", "Background", points, parent);
        core.transform.localPosition = Vector3.forward;

        // Determine terrain size
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

        var chunkDictionary = new Dictionary<string, HexChunk>();

        // Place tiles
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
                    // TODO: Make more caves
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

                                var chunk = body.AddComponent<HexChunk>();
                                chunk.Parent = parent.GetComponent<CelestialBody>();
                                chunk.Map = tileMap;

                                chunkDictionary[id] = chunk;
                            }

                            chunkDictionary[id].AddTile(new Vector2Int(col, row), tiles[index]);

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

        foreach (var chunk in chunkDictionary.Values)
        {
            var rigidBody = chunk.gameObject.AddComponent<Rigidbody2D>();
            rigidBody.bodyType = RigidbodyType2D.Static;
            rigidBody.mass = chunk.Mass;
            rigidBody.gravityScale = 0;
            rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Discrete;

            var tileCollider = chunk.gameObject.AddComponent<TilemapCollider2D>();
            tileCollider.usedByComposite = true;

            var collider = chunk.gameObject.AddComponent<CompositeCollider2D>();
            collider.geometryType = CompositeCollider2D.GeometryType.Polygons;
        }
    }
}
