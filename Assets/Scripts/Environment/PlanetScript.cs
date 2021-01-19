using System.Linq;
using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.U2D;

public class PlanetScript : MonoBehaviour
{
    public SpriteShape PlanetSpriteShape;

    public float RotationsPerMinute = 1;

    public GameObject[] Blocks;

    private GameObject core;

    private int MinSizeForGravity = 2;

    private int[,] Tiles;

    void Start()
    {
        GenerateCoreWithSlicesPlanet();
    }

    void GenerateCoreWithSlicesPlanet()
    {
        var collider = GetComponent<CircleCollider2D>();
        var radius = Mathf.FloorToInt(collider.radius);
        GenerateCoreWithSlices(radius, 100, 0.1f);
    }

    void GenerateSingleObjectPlanet()
    {
        var collider = GetComponent<CircleCollider2D>();
        var radius = Mathf.FloorToInt(collider.radius);
        GenerateCore(radius, 100, 0.1f);
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
        if (core)
        {
            var rotationsPerFrame = (RotationsPerMinute / 60f) * Time.deltaTime;
            var angle = (2f * Mathf.PI) * rotationsPerFrame;
            core.transform.Rotate(Vector3.forward, angle);
        }
    }

    private GameObject GetRandomBlock()
    {
        return Blocks[UnityEngine.Random.Range(0, this.Blocks.Length)];
    }

    private GameObject GenerateCore(float radius, int granularity, float variationPercent)
    {
        var area = Mathf.PI * Mathf.Pow(radius, 2);
        var permimeter = 2 * Mathf.PI * radius;

        var points = new Vector2[granularity];
        for (int i = 0; i < granularity; i++)
        {
            var angle = 2 * Mathf.PI / granularity * i;
            var variation = variationPercent * radius / 2;
            var length = (radius + UnityEngine.Random.Range(variation * -1, variation));

            var x = Mathf.Cos(angle) * length;
            var y = Mathf.Sin(angle) * length;

            points[i] = new Vector2(x, y);
        }

        core = GenerateShape("Core", points, gameObject.transform, 10, false);
        core.transform.position = gameObject.transform.position;

        return core;
    }

    private GameObject GenerateShape(string name, Vector2[] points, Transform parent, float density = 1, bool withRigidBody = true)
    {
        //var sum = Vector2.zero;

        //foreach (var point in points)
        //{
        //    sum += point;
        //}

        //var avg = sum / points.Length;

        var center = GetCenter(points);
        var shape = new GameObject { name = name, tag = "Planet" };

        shape.transform.parent = parent;
        shape.transform.position = parent.transform.position + (Vector3)center;

        //shape.transform.position = avg;

        var renderer = shape.AddComponent<SpriteShapeRenderer>();
        var shapeController = shape.AddComponent<SpriteShapeController>();
        var shapeCollider = shape.AddComponent<PolygonCollider2D>();
        var gravity = shape.AddComponent<Gravity>();

        var area = GetArea(points);
        shapeController.spriteShape = PlanetSpriteShape;
        shapeController.splineDetail = points.Length;
        shapeController.enableTangents = true;
        gravity.GravityRange = area;
        gravity.GravityPower = area;

        for (int i = 0; i < points.Length; i++)
        {
            shapeController.spline.InsertPointAt(i, new Vector2(points[i].x, points[i].y) - center);
        }
        shapeCollider.points = points.Select(x => x - center).ToArray();

        if (withRigidBody)
        {
            var rigidBody = shape.AddComponent<Rigidbody2D>();
            rigidBody.mass = area;
            rigidBody.gravityScale = 0;
            rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        //var shadow = shape.AddComponent<ShadowCaster2D>();
        //shadow.useRendererSilhouette = true;
        //shadow.transform.localScale = Vector3.one;

        //shape.gameObject.transform.position = new Vector3(renderer.bounds.center.x, renderer.bounds.center.y, 0);

        return shape;
    }

    float GetArea(Vector2[] points)
    {
        float temp = 0;
        int i = 0;
        for (; i < points.Length; i++)
        {
            if (i != points.Length - 1)
            {
                float mulA = points[i].x * points[i + 1].y;
                float mulB = points[i + 1].x * points[i].y;
                temp = temp + (mulA - mulB);
            }
            else
            {
                float mulA = points[i].x * points[0].y;
                float mulB = points[0].x * points[i].y;
                temp = temp + (mulA - mulB);
            }
        }
        temp *= 0.5f;
        return Mathf.Abs(temp);
    }

    Vector2 GetCenter(Vector2[] points)
    {
        var total = Vector2.zero;

        foreach (var point in points)
        {
            total += point;
        }

        return total / points.Length;
    }

    private GameObject GenerateCoreWithSlices(float radius, int granularity, float variationPercent)
    {
        var segments = 10;
        var core = GenerateCore(radius / 2, granularity, variationPercent);

        var points = core.GetComponent<PolygonCollider2D>().points;

        var variation = variationPercent * radius / 2;
        var firstLength = (radius + UnityEngine.Random.Range(variation * -1, variation));
        var lastAngle = 2 * Mathf.PI / granularity * (points.Length - 1);
        var previousX = Mathf.Cos(lastAngle) * firstLength;
        var previousY = Mathf.Sin(lastAngle) * firstLength;

        var outerPoints = new Vector2[points.Length];
        var segment = 0;
        for (int i = 0; i < points.Length; i++)
        {
            var angle = 2 * Mathf.PI / granularity * i;

            variation = variationPercent * radius / 2;
            var length = (radius + UnityEngine.Random.Range(variation * -1, variation));
            var x = Mathf.Cos(angle) * length;
            var y = Mathf.Sin(angle) * length;

            outerPoints[i] = new Vector2(x, y);

            if (i > granularity / segments * (segment + 1))
            {
                var slice = outerPoints
                    .Skip(granularity / segments * segment)
                    .Take((granularity / segments) + 1)
                    .Union(points.Skip(granularity / segments * segment).Take((granularity / segments) + 1).Reverse())
                    .ToArray();

                var shape = GenerateShape("Shell", slice, core.transform);
                var gravity = shape.AddComponent<GravityObject>();
                gravity.RefreshRate = 0;

                segment++;
            }
        }

        var lastSlice = outerPoints
            .Skip(granularity / segments * segment)
            .Take((granularity / segments) + 1)
            .Union(new[] { outerPoints.First(), points.First() })
            .Union(points.Skip(granularity / segments * segment).Take((granularity / segments) + 1).Reverse())
            .ToArray();

        var lastShape = GenerateShape("Shell", lastSlice, core.transform);
        var lastGravity = lastShape.AddComponent<GravityObject>();
        lastGravity.RefreshRate = 0;


        //var singleSlice = outerPoints.Union(points.Reverse()).ToArray();
        //var shape = GenerateShape("Shell", singleSlice, 64);
        //shape.AddComponent<GravityObject>();

        //var chunks = 3;
        //for (int i = 0; i < chunks; i++)
        //{
        //    var slicePoints = new Vector2[granularity / chunks];

        //    for (int j = 0; j < slicePoints.Length; j++)
        //    {
        //        slicePoints[j] = outerPoints[

        //    }


        //    var shape = GenerateShape(slicePoints, 64);
        //    shape.transform.position = core.transform.position;
        //    shape.AddComponent<GravityObject>();
        //}


        // Todo center game object on average location of points and adjust points accordingly
        //var nextPoint = points[(i + 1) % points.Length];

        //var slicePoints = new[]
        //{
        //        new Vector2(points[i].x, points[i].y),
        //        new Vector2(nextPoint.x, nextPoint.y),
        //        new Vector2(x, y),
        //        new Vector2(previousX, previousY)
        //    };


        //previousX = x;
        //previousY = y;


        return core;
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
