using Assets.Scripts.Helpers;
using UnityEngine;

public class PlanetScript : MonoBehaviour
{
    public GameObject[] Blocks;

    private int MinSizeForGravity = 2;

    private int[,] Tiles;

    void Start()
    {
        GenerateFractalCubePlanet();
    }

    void GenerateSingleObjectPlanet()
    {
        // TODO
        GenerateShape();
    }

    void GenerateRandomBlocksPlanet()
    {
        var collider = GetComponent<CircleCollider2D>();

        var radius = Mathf.FloorToInt(collider.radius);

        var blockSize = GetMaxBlockSize(radius);

        for (int i = 0; i < radius * 2; i++)
        {
            var point = Geometry.GetRandomPointOnCircle(radius * UnityEngine.Random.Range(0, 100));
            GenerateBlock(point, UnityEngine.Random.Range(1, blockSize));
        }
    }

    void GenerateFractalCubePlanet()
    {
        var collider = GetComponent<CircleCollider2D>();

        var radius = Mathf.FloorToInt(collider.radius);

        var blockSize = GetMaxBlockSize(radius);

        Tiles = new int[radius, radius];

        Tiles = CalculateQuadrant(radius, blockSize);

        BuildPlanet(radius);
    }

    private int[,] CalculateQuadrant(float radius, float blockSize)
    {
        var tiles = new int[(int)radius, (int)radius];

        for (float s = blockSize; s >= 1; s /= 2)
        {
            for (float x = 0; x < radius; x++)
            {
                for (float y = 0; y < radius; y++)
                {
                    if (tiles[(int)x, (int)y] == 0)
                    {
                        if (TileIsWithinRadius(x, y, radius))
                        {
                            var isThereSpace = true;

                            for (int bx = (int)x; bx < s + (int)x; bx++)
                            {
                                for (int by = (int)y; by < s + (int)y; by++)
                                {
                                    if (bx >= radius || by >= radius || tiles[bx, by] != 0 || !TileIsWithinRadius(bx, by, radius))
                                    {
                                        isThereSpace = false;
                                    }
                                }
                            }

                            if (isThereSpace)
                            {
                                for (int bx = (int)x; bx < s + (int)x; bx++)
                                {
                                    for (int by = (int)y; by < s + (int)y; by++)
                                    {
                                        tiles[bx, by] = (int)s;
                                    }
                                }
                            }
                        }
                        else
                        {
                            tiles[(int)x, (int)y] = -1;
                        }
                    }
                }
            }
        }

        return tiles;
    }

    private bool TileIsWithinRadius(float x, float y, float radius)
    {
        return Mathf.Pow(Mathf.Pow(x, 2) + Mathf.Pow(y, 2), 0.5f) <= radius;
    }

    private void BuildPlanet(float radius)
    {
        for (int x = 0; x < radius; x++)
        {
            for (int y = 0; y < radius; y++)
            {
                float s = Tiles[x, y];

                if (s > 0)
                {
                    // Add tile
                    var gx = gameObject.transform.position.x;
                    var gy = gameObject.transform.position.y;

                    if (x == 0 && y == 0)
                    {
                        // Generate the core
                        GenerateBlock(new Vector3(gx, gy, 0), s * 2);
                    }
                    else
                    {
                        GenerateBlock(new Vector3(gx + x + s / 2, gy + y + s / 2, 0), s);
                        GenerateBlock(new Vector3(gx + -1 * (x + s / 2), gy + y + s / 2, 0), s);
                        GenerateBlock(new Vector3(gx + x + s / 2, gy + -1 * (y + s / 2), 0), s);
                        GenerateBlock(new Vector3(gx + -1 * (x + s / 2), gy + -1 * (y + s / 2), 0), s);
                    }
                    // Mark tiles as added
                    for (int bx = x; bx < s + x; bx++)
                    {
                        for (int by = y; by < s + y; by++)
                        {
                            Tiles[bx, by] = -1;
                        }
                    }
                }
            }
        }
    }

    void Update()
    {

    }

    private GameObject GetRandomBlock()
    {
        return Blocks[UnityEngine.Random.Range(0, this.Blocks.Length)];
    }

    private GameObject GenerateShape()
    {
        var shape = new GameObject();
        var collider = shape.AddComponent<PolygonCollider2D>();

        // TODO

        return shape;
    }

    private GameObject GenerateBlock(Vector3 position, float blockSize)
    {
        var template = GetRandomBlock();
        template.transform.localScale = new Vector3(blockSize, blockSize, blockSize);

        var block = Spawner.BuildBlock(template, position, true);
        if (block)
        {
            var body = block.GetComponent<Rigidbody2D>();
            body.mass = Mathf.Pow(blockSize, 3);

            if (blockSize >= MinSizeForGravity)
            {
                var gravity = block.AddComponent<Gravity>();
                gravity.GravityRange = blockSize;
                gravity.GravityPower = blockSize;
            }
        }

        return block;
    }

    private float GetMaxBlockSize(float radius)
    {
        var blockSize = 1;

        while (blockSize * 2 < radius)
        {
            blockSize *= 2;
        }

        return blockSize;
    }
}
