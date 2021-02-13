using System;
using Assets.Scripts.Helpers;
using UnityEditor;
using UnityEngine;

public class GravitySource : MonoBehaviour
{
    public float GravityPower = 8;
    public Vector2 CenterOfMass;

    public Vector2 GetForce(GravitySource target)
    {
        TryGetComponent<Rigidbody2D>(out var rigidBody);
        if (rigidBody != null && rigidBody?.mass != GravityPower)
        {
            throw new Exception("Mass doesn't equal gravity!");
        }

        // TODO: Improve how we find the center of the object
        TryGetComponent<CompositeCollider2D>(out var collider);
        if (collider != null)
        {
            CenterOfMass = collider.bounds.center;
        }
        else
        {
            CenterOfMass = (rigidBody?.centerOfMass ?? Vector2.zero) + (Vector2)transform.position;
        }

        return Gravity.CalculateGravity(target.CenterOfMass, target.GravityPower, CenterOfMass, GravityPower);
    }

    void OnDrawGizmos()
    {
#if DEBUG
        TryGetComponent<Rigidbody2D>(out var rigidBody);
        Handles.Label(CenterOfMass, GravityPower.ToString());
#endif
    }
}
