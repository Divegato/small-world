using System;
using System.Linq;
using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class HexTilePlanet
{
    public static void Generate(float radius, Transform parent)
    {
        var body = new GameObject { name = "HexPlanet", tag = "Planet" };
        body.transform.parent = parent;
        body.transform.localPosition = Vector3.zero;

        var tileMap = body.AddComponent<Tilemap>();
        var renderer = body.AddComponent<TilemapRenderer>();

        var grid = body.AddComponent<Grid>();
        grid.cellLayout = GridLayout.CellLayout.Hexagon;
        grid.cellSize = new Vector3(2.28f, 2.66f, 1);

        var gravity = body.AddComponent<Gravity>();
        gravity.GravityPower = Mathf.PI * Mathf.Pow(radius, 2);

        var sprites = Resources.LoadAll<Sprite>("Tilemap/hex-tile-samples");
        var tiles = sprites.Select(x =>
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = x;
            tile.colliderType = Tile.ColliderType.Grid;

            return tile;
        }).ToArray();

        var intRadius = Mathf.FloorToInt(radius);

        var radiusPowerOfTwo = 2;
        while (intRadius * 2 > radiusPowerOfTwo)
        {
            radiusPowerOfTwo *= 2;
        }

        var heightMap = new DiamondSquare(radiusPowerOfTwo, tiles.Length, UnityEngine.Random.value).getData();
        var tileIndex = UnityEngine.Random.Range(0, tiles.Length);

        for (int x = 0; x < radiusPowerOfTwo; x++)
        {
            for (int y = 0; y < radiusPowerOfTwo; y++)
            {
                var xCoord = x - intRadius;
                var yCoord = y - intRadius;

                var row = (xCoord * -1) - yCoord;
                var col = xCoord + (row - (row & 1)) / 2;

                var cubeCoordinate = HexCoordinates.ConvertOffsetToCube(row, col);
                if ((Vector3Int.zero - cubeCoordinate).magnitude * 2.5f < radius)
                {
                    var index = Math.Max(0, Math.Min((int)Math.Floor(heightMap[row + intRadius, col + intRadius]), tiles.Length - 1));

                    if (index == 5)
                    {
                        // cave
                    }
                    else
                    {
                        tileMap.SetTile(new Vector3Int(col, row, 0), tiles[index]);
                    }
                }
            }
        }

        var rigidBody = body.AddComponent<Rigidbody2D>();
        rigidBody.bodyType = RigidbodyType2D.Kinematic;
        rigidBody.mass = Mathf.PI * Mathf.Pow(radius, 2);
        rigidBody.gravityScale = 0;
        rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var tileCollider = body.AddComponent<TilemapCollider2D>();
        tileCollider.usedByComposite = true;

        var collider = body.AddComponent<CompositeCollider2D>();
        collider.geometryType = CompositeCollider2D.GeometryType.Polygons;
        collider.generationType = CompositeCollider2D.GenerationType.Manual;
        collider.GenerateGeometry();
    }
}
