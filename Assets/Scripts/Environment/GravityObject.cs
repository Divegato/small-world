using Assets.Scripts.Helpers;
using UnityEngine;

public class GravityObject : MonoBehaviour
{
    public float RefreshRate = 10;

    private float progress = 0;
    private Vector2 force;

    void Start()
    {
        progress = Random.value * RefreshRate;

        var body = GetComponent<Rigidbody2D>();
        force = Environment.GetAverageGravitationalForce(body);
    }

    void Update()
    {
        progress += Time.deltaTime;

        var body = GetComponent<Rigidbody2D>();

        var color = Color.blue;

        if (progress >= RefreshRate)
        {
            progress = 0;
            force = Environment.GetAverageGravitationalForce(body);
            color = Color.red;
        }

        Debug.DrawLine(this.gameObject.transform.position, this.gameObject.transform.position + new Vector3(force.x, force.y, 0), color);

        body.AddForce(force, ForceMode2D.Force);
    }
}
