using Assets.Scripts.Helpers;
using UnityEditor;
using UnityEngine;

public class GravitySource : MonoBehaviour
{
    public float GravityPower = 8;

    public Vector2 GetForce(Rigidbody2D target)
    {
        TryGetComponent<Rigidbody2D>(out var rigidBody);
        var mass = rigidBody?.mass ?? GravityPower;
        var centerOfMass = (rigidBody?.centerOfMass ?? Vector2.zero) + (Vector2)transform.position;

        return Gravity.CalculateGravity(target.centerOfMass + target.position, target.mass, centerOfMass, mass);
    }

    void OnDrawGizmos()
    {
#if DEBUG
        TryGetComponent<Rigidbody2D>(out var rigidBody);
        Handles.Label((rigidBody?.centerOfMass ?? Vector2.zero) + (Vector2)transform.position, (rigidBody?.mass ?? GravityPower).ToString());
#endif
    }
}
