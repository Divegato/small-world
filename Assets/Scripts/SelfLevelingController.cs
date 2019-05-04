using Assets.Scripts.Helpers;
using UnityEngine;

public class SelfLevelingController : MonoBehaviour
{
    private Rigidbody2D body;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        var force = Environment.GetAverageGravitationalForce(body);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, force * -1);
    }
}
