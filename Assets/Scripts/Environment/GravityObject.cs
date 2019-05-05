using Assets.Scripts.Helpers;
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

        if (progress >= RefreshRate)
        {
            progress = 0;
            force = Environment.GetAverageGravitationalForce(body);
        }

        body.AddForce(force, ForceMode2D.Force);
    }
}
