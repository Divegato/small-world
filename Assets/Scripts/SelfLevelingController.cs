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
        var force = Gravity.GetAverageGravitationalForce(body);
        // TODO: Limit the max speed of rotation
        transform.rotation = Quaternion.LookRotation(Vector3.forward, force * -1);
    }
}
