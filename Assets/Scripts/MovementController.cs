using UnityEngine;

public class MovementController : MonoBehaviour
{
    public KeyCode UpKey = KeyCode.UpArrow;
    public KeyCode DownKey = KeyCode.DownArrow;
    public KeyCode LeftKey = KeyCode.LeftArrow;
    public KeyCode RightKey = KeyCode.RightArrow;

    void Start()
    {

    }

    void Update()
    {
        var body = GetComponent<Rigidbody2D>();
        var direction = Vector2.zero;

        if (Input.GetKey(this.UpKey))
        {
            direction = body.transform.up;
        }
        if (Input.GetKey(this.DownKey))
        {
            direction = body.transform.up * -1f;
        }
        if (Input.GetKey(this.LeftKey))
        {
            direction = Rotate(body.transform.up, 90);
        }
        if (Input.GetKey(this.RightKey))
        {
            direction = Rotate(body.transform.up, -90);
        }

        body.AddForce(direction / 5, ForceMode2D.Impulse);
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
