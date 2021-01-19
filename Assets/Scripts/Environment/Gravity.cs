using UnityEditor;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    private const float GravitationalConstant = 6.67f;

    public float GravityPower = 8;
    public float GravityRange = 8;

    void Start()
    {
    }

    //void Update()
    //{
    //    //var player = GameObject.FindGameObjectWithTag("Player");
    //    //var items = GameObject.FindGameObjectsWithTag("Item");

    //    //if (player != null)
    //    //{
    //    //    AddGravityForce(player.GetComponent<Rigidbody2D>());
    //    //}

    //    //foreach (var item in items)
    //    //{
    //    //    var body = item.GetComponent<Rigidbody2D>();
    //    //    AddGravityForce(body);
    //    //}
    //}

    private Vector2 GetSelfPosition2d()
    {
        var selfPosition = this.gameObject.transform.position;
        return new Vector2(selfPosition.x, selfPosition.y);
    }

    private float GetSelfRadius()
    {
        return gameObject.transform.localScale.x;
        // GetComponent<CircleCollider2D>().radius;
    }

    private void AddGravityForce(Rigidbody2D body)
    {
        var force = GetForce(body);
        body.AddForce(force, ForceMode2D.Force);
    }

    public Vector2 GetForce(Rigidbody2D target)
    {
        TryGetComponent<Rigidbody2D>(out var rigidBody);
        var mass = rigidBody?.mass ?? GravityPower;
        var centerOfMass = (rigidBody?.centerOfMass ?? Vector2.zero) + (Vector2)transform.position;
        return CalculateGravity(target.centerOfMass + target.position, target.mass, centerOfMass, mass);

        var selfPosition = GetSelfPosition2d();
        var selfRadius = GetSelfRadius();

        var differenceFromCore = (selfPosition - target.position);
        //var distanceFromSurface = differenceFromCore.magnitude - selfRadius;
        var percent = Mathf.Min(1, 100f * Mathf.Pow(0.9f, differenceFromCore.magnitude));
        var force = differenceFromCore.normalized * GravityPower * percent;

        return force;
    }

    private static Vector2 CalculateGravity(Vector2 targetPosition, float targetMass, Vector2 centerOfMass, float mass)
    {
        var distance = centerOfMass - targetPosition;

        if (distance.magnitude == 0)
        {
            return Vector2.zero;
        }

        var force = GravitationalConstant * (Mathf.Max(1f, targetMass) * Mathf.Max(1f, mass) / Mathf.Pow(Mathf.Max(1f, distance.magnitude), 2f));

        return distance.normalized * force;
    }

    void OnDrawGizmos()
    {
#if DEBUG
        TryGetComponent<Rigidbody2D>(out var rigidBody);
        Handles.Label((rigidBody?.centerOfMass ?? Vector2.zero) + (Vector2)transform.position, (rigidBody?.mass ?? GravityPower).ToString());
#endif
    }
}
