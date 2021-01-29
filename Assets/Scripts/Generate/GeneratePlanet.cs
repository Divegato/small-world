using System.Linq;
using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.U2D;

public static class GeneratePlanet
{
    public static void GenerateCoreWithSlicesPlanet(float radius, Transform parent)
    {
        var shape = Resources.Load<SpriteShape>("SpriteShapes/SpriteShape");
        GenerateCoreWithSlices(radius, 100, 0.1f, parent, shape);
    }

    public static void GenerateSingleObjectPlanet(float radius, Transform parent, SpriteShape spriteShape)
    {
        GenerateCore(radius, 100, 0.1f, parent, spriteShape);
    }

    public static void GenerateRandomBlocksPlanet(float radius, GameObject[] blocks)
    {
        var blockSize = GetMaxBlockSize(radius);

        for (int i = 0; i < radius * 2; i++)
        {
            var point = Geometry.GetRandomPointOnCircle(radius * UnityEngine.Random.Range(0, 100));
            GenerateBlock(point, UnityEngine.Random.Range(1, blockSize), blocks);
        }
    }

    public static void GenerateFractalCubePlanet(int radius, Vector2 center, GameObject[] blocks)
    {
        var blockSize = GetMaxBlockSize(radius);

        var tiles = new int[radius, radius];

        tiles = CalculateQuadrant(radius, blockSize);

        BuildPlanet(radius, tiles, center, blocks);
    }

    private static int[,] CalculateQuadrant(float radius, float blockSize)
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

    private static bool TileIsWithinRadius(float x, float y, float radius)
    {
        return Mathf.Pow(Mathf.Pow(x, 2) + Mathf.Pow(y, 2), 0.5f) <= radius;
    }

    private static void BuildPlanet(float radius, int[,] tiles, Vector2 center, GameObject[] blocks)
    {
        for (int x = 0; x < radius; x++)
        {
            for (int y = 0; y < radius; y++)
            {
                float s = tiles[x, y];

                if (s > 0)
                {
                    // Add tile
                    var gx = center.x;
                    var gy = center.y;

                    if (x == 0 && y == 0)
                    {
                        // Generate the core
                        GenerateBlock(new Vector3(gx, gy, 0), s * 2, blocks);
                    }
                    else
                    {
                        GenerateBlock(new Vector3(gx + x + s / 2, gy + y + s / 2, 0), s, blocks);
                        GenerateBlock(new Vector3(gx + -1 * (x + s / 2), gy + y + s / 2, 0), s, blocks);
                        GenerateBlock(new Vector3(gx + x + s / 2, gy + -1 * (y + s / 2), 0), s, blocks);
                        GenerateBlock(new Vector3(gx + -1 * (x + s / 2), gy + -1 * (y + s / 2), 0), s, blocks);
                    }
                    // Mark tiles as added
                    for (int bx = x; bx < s + x; bx++)
                    {
                        for (int by = y; by < s + y; by++)
                        {
                            tiles[bx, by] = -1;
                        }
                    }
                }
            }
        }
    }

    private static GameObject GetRandomBlock(GameObject[] blocks)
    {
        return blocks[UnityEngine.Random.Range(0, blocks.Length)];
    }

    private static GameObject GenerateCore(float radius, int granularity, float variationPercent, Transform parent, SpriteShape spriteShape)
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

        var core = GenerateShape("Core", points, parent, spriteShape, 10, false, false);
        core.transform.position = parent.position;

        return core;
    }

    private static GameObject GenerateShape(string name, Vector2[] points, Transform parent, SpriteShape spriteShape, float density = 1, bool withRigidBody = true, bool deconstructable = true)
    {
        //var sum = Vector2.zero;

        //foreach (var point in points)
        //{
        //    sum += point;
        //}

        //var avg = sum / points.Length;

        var center = Geometry.GetCenter(points);
        var shape = new GameObject { name = name, tag = "Planet" };

        shape.transform.parent = parent;
        shape.transform.position = parent.transform.position + (Vector3)center;

        //shape.transform.position = avg;

        var renderer = shape.AddComponent<SpriteShapeRenderer>();
        var shapeController = shape.AddComponent<SpriteShapeController>();
        var shapeCollider = shape.AddComponent<PolygonCollider2D>();
        var gravity = shape.AddComponent<Gravity>();

        if (deconstructable)
        {
            shape.AddComponent<Deconstructable>();
        }

        var area = Geometry.GetArea(points);
        shapeController.spriteShape = spriteShape;
        shapeController.splineDetail = points.Length;
        shapeController.enableTangents = true;
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

            if (parent.gameObject.TryGetComponent<Rigidbody2D>(out var parentBody))
            {
                var joint = shape.AddComponent<FixedJoint2D>();
                joint.autoConfigureConnectedAnchor = true;
                joint.connectedBody = parentBody.GetComponent<Rigidbody2D>();
                joint.breakForce = 400;
                joint.breakTorque = 300;
            }
        }

        //var shadow = shape.AddComponent<ShadowCaster2D>();
        //shadow.useRendererSilhouette = true;
        //shadow.transform.localScale = Vector3.one;

        //shape.gameObject.transform.position = new Vector3(renderer.bounds.center.x, renderer.bounds.center.y, 0);

        return shape;
    }

    public static GameObject GenerateCoreWithSlices(float radius, int granularity, float variationPercent, Transform parent, SpriteShape spriteShape)
    {
        var segments = 10;
        var core = GenerateCore(radius / 2, granularity, variationPercent, parent, spriteShape);

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

                var shape = GenerateShape("Shell", slice, core.transform, spriteShape);
                var gravity = shape.AddComponent<GravityObject>();
                gravity.RefreshRate = 1f;

                segment++;
            }
        }

        var lastSlice = outerPoints
            .Skip(granularity / segments * segment)
            .Take((granularity / segments) + 1)
            .Union(new[] { outerPoints.First(), points.First() })
            .Union(points.Skip(granularity / segments * segment).Take((granularity / segments) + 1).Reverse())
            .ToArray();

        var lastShape = GenerateShape("Shell", lastSlice, core.transform, spriteShape);
        var lastGravity = lastShape.AddComponent<GravityObject>();
        lastGravity.RefreshRate = 1f;


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

    public static GameObject GenerateBlock(Vector3 position, float blockSize, GameObject[] blocks)
    {
        var template = GetRandomBlock(blocks);
        template.transform.localScale = new Vector3(blockSize, blockSize, blockSize);

        var block = Spawner.BuildBlock(template, position, true);
        if (block)
        {
            var body = block.GetComponent<Rigidbody2D>();
            body.mass = Mathf.Pow(blockSize, 3);

            if (blockSize >= 2)
            {
                var gravity = block.AddComponent<Gravity>();
                gravity.GravityPower = blockSize;
            }
        }

        return block;
    }

    public static float GetMaxBlockSize(float radius)
    {
        var blockSize = 1;

        while (blockSize * 2 < radius)
        {
            blockSize *= 2;
        }

        return blockSize;
    }
}
