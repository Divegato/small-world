using System.Linq;
using Assets.Scripts.Helpers;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public KeyCode UpKey = KeyCode.W;
    public KeyCode DownKey = KeyCode.S;
    public KeyCode LeftKey = KeyCode.A;
    public KeyCode RightKey = KeyCode.D;

    public KeyCode JumpKey = KeyCode.Space;

    public float JumpPower = 10f;
    public float MaxEnergy = 10f;

    private float energy;

    void Start()
    {
        energy = MaxEnergy;
    }

    void Update()
    {
        var body = GetComponent<Rigidbody2D>();
        var collider = GetComponent<Collider2D>();
        var direction = Vector2.zero;

        if (Input.GetKey(this.UpKey))
        {
            direction += new Vector2(body.transform.up.x, body.transform.up.y);
        }
        if (Input.GetKey(this.DownKey))
        {
            direction += new Vector2(body.transform.up.x, body.transform.up.y) * -1f;
        }
        if (Input.GetKey(this.LeftKey))
        {
            direction += Rotate(body.transform.up, 90);
        }
        if (Input.GetKey(this.RightKey))
        {
            direction += Rotate(body.transform.up, -90);
        }

        Physics2D.OverlapArea(body.transform.up, body.transform.up);

        if (direction.magnitude > 0)
        {
            var nearby = Environment.GetNearbyBlocks(collider);
            if (nearby.Any())
            {
                body.velocity = (body.velocity + (direction.normalized * 10)) / 2;
                foreach (var block in nearby)
                {
                    if (block.attachedRigidbody)
                    {
                        block.attachedRigidbody.AddForce(direction.normalized * (-100f / nearby.Length));
                    }
                }
            }
        }
        else
        {
            energy += 0.1f;
        }

        if (Input.GetKeyDown(this.JumpKey))
        {
            if (Environment.IsGrounded(gameObject))
            {
                body.AddForce(body.transform.up * JumpPower, ForceMode2D.Impulse);
            }
        }
    }

    private Vector2 Rotate(Vector2 source, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        var x = (cos * source.x) - (sin * source.y);
        var y = (sin * source.x) + (cos * source.y);

        return new Vector2(x, y);
    }
}
