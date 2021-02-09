using System;
using System.Linq;
using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class HexTilePlanet
{
    public static void Generate(float radius, Transform parent)
    {
        var points = Geometry.GetCircle(radius * 0.9f, 100);
        var core = GeneratePlanet.GenerateShape("Core", "Background", points, parent);
        core.transform.localPosition = Vector3.forward;

        var body = new GameObject { name = "HexPlanet", tag = "Planet" };
        body.transform.parent = core.transform;
        body.transform.localPosition = Vector3.back;

        body.AddComponent<Deconstructable>();

        var hexSize = 1.33f;
        var grid = body.AddComponent<Grid>();
        grid.cellLayout = GridLayout.CellLayout.Hexagon;
        grid.cellSize = new Vector3(Mathf.Sqrt(3) * hexSize, 2 * hexSize, 1);

        var tileMap = body.AddComponent<Tilemap>();
        tileMap.tileAnchor = Vector3.zero;

        var renderer = body.AddComponent<TilemapRenderer>();

        var gravity = body.AddComponent<GravitySource>();
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

                var actualPosition = new Vector2(hexSize * Mathf.Sqrt(3) * (col + 0.5f * (row & 1)), hexSize * 3 / 2 * row);

                if (actualPosition.magnitude < radius)
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
    }
}
