using System;
using System.Linq;
using UnityEngine;

public class FractalPlanetScript : MonoBehaviour
{
    public GameObject[] Blocks;

    private Direction[] directions;

    void Start()
    {
        var collider = GetComponent<CircleCollider2D>();
        directions = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToArray();

        var blockSize = 1;

        do
        {
            blockSize *= 2;
        }
        while (blockSize < collider.radius);

        GenerateCore(blockSize);
    }

    void Update()
    {

    }

    private void GenerateCore(float blockSize)
    {
        var core = GenerateBlock(gameObject.transform.position, blockSize);

        blockSize /= 2;

        if (blockSize >= 1)
        {
            foreach (var direction in directions)
            {
                GenerateNodes(core, direction, blockSize);
            }
        }
    }

    private void GenerateNodes(GameObject parent, Direction direction, float blockSize)
    {
        var newLocation = GetNodeOffset(parent.transform.position, direction, blockSize);
        var block = GenerateBlock(newLocation, blockSize);

        var joint = block.AddComponent<FixedJoint2D>();
        joint.connectedBody = parent.GetComponent<Rigidbody2D>();
        // Add weld location

        blockSize /= 2;

        if (blockSize >= 1)
        {
            foreach (var option in directions)
            {
                if (!IsOppositeDirection(option, direction))
                {
                    GenerateNodes(block, option, blockSize);
                }
            }
        }
    }

    private Vector3 GetNodeOffset(Vector3 parentLocation, Direction direction, float blockSize)
    {
        var x = parentLocation.x;
        var y = parentLocation.y;

        switch (direction)
        {
            case Direction.N:
                y += blockSize * 1.5f;
                break;
            case Direction.S:
                y -= blockSize * 1.5f;
                break;
            case Direction.E:
                x += blockSize * 1.5f;
                break;
            case Direction.W:
                x -= blockSize * 1.5f;
                break;
        }

        return new Vector3(x, y);
    }

    private GameObject GetRandomBlock()
    {
        return Blocks[UnityEngine.Random.Range(0, this.Blocks.Length)];
    }

    private GameObject GenerateBlock(Vector3 position, float blockSize)
    {
        var block = Instantiate(GetRandomBlock(), position, Quaternion.identity);
        block.transform.localScale = new Vector3(blockSize, blockSize, blockSize);
        //Debug.Log(position);
        //Debug.Log(blockSize);
        // Set Mass
        // Attach Rigidbody

        return block;
    }

    private bool IsOppositeDirection(Direction d1, Direction d2)
    {
        switch (d1)
        {
            case Direction.N:
                return d2 == Direction.S;
            case Direction.S:
                return d2 == Direction.N;
            case Direction.E:
                return d2 == Direction.W;
            case Direction.W:
                return d2 == Direction.E;
        }

        throw new Exception("THIS IS NOT EVEN POSSIBLE! WHAT HAVE YOU DONE!?");
    }

    private enum Direction
    {
        N,
        S,
        E,
        W
    }
}
