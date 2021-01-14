using UnityEditor;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public float GravityPower = 8;
    public float GravityRange = 8;

    void Start()
    {

    }

    void Update()
    {
        //var player = GameObject.FindGameObjectWithTag("Player");
        //var items = GameObject.FindGameObjectsWithTag("Item");

        //if (player != null)
        //{
        //    AddGravityForce(player.GetComponent<Rigidbody2D>());
        //}

        //foreach (var item in items)
        //{
        //    var body = item.GetComponent<Rigidbody2D>();
        //    AddGravityForce(body);
        //}
    }

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
        var selfPosition = GetSelfPosition2d();
        var selfRadius = GetSelfRadius();

        var differenceFromCore = (selfPosition - target.position);
        var distanceFromSurface = differenceFromCore.magnitude - selfRadius;
        var percent = Mathf.Min(1, Mathf.Exp(distanceFromSurface * (-1f / GravityRange)));
        var force = differenceFromCore.normalized * (GravityPower / 8) * percent;

        return force;
    }

    void OnDrawGizmos()
    {
        Handles.Label(transform.position, GravityPower.ToString());
    }
}
