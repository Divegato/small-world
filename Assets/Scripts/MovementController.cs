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

        if (Input.GetKeyDown(this.UpKey))
        {
            direction = body.transform.up;
        }
        if (Input.GetKeyDown(this.DownKey))
        {
            direction = body.transform.up * -1;
        }
        if (Input.GetKeyDown(this.LeftKey))
        {
            direction = Vector2.left;
        }
        if (Input.GetKeyDown(this.RightKey))
        {
            direction = Vector2.right;
        }

        body.AddForce(direction, ForceMode2D.Impulse);
    }
}
