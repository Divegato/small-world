using Assets.Scripts.Helpers;
using UnityEditor;
using UnityEngine;

public class GravityObject : MonoBehaviour
{
    public float RefreshRate = 1;

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

        Debug.DrawLine(body.centerOfMass + body.position, body.centerOfMass + body.position + new Vector2(force.x, force.y) / 60, color);

        body.AddForce(force * Time.deltaTime, ForceMode2D.Force);
    }

    void OnDrawGizmos()
    {
#if DEBUG
        if (gameObject.tag == "Player")
        {
            Handles.Label(transform.position, force.magnitude.ToString("n1"));
        }
#endif
    }
}
