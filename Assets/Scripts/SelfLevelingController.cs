using System.Linq;
using Assets.Scripts.Helpers;
using UnityEngine;

public class SelfLevelingController : MonoBehaviour
{
    public float MaxRotateAngle = 0.25f;

    void Start()
    {

    }

    void Update()
    {
        var body = GetComponent<Rigidbody2D>();
        var force = Environment.GetAverageGravitationalForce(body);
        gameObject.transform.up = force * -1;


        //var gravitySources = FindObjectsOfType<Gravity>();

        //var averageForce = Vector2.zero;
        //foreach (var source in gravitySources)
        //{
        //    averageForce += source.GetForce(body);
        //}

        //gameObject.transform.up = averageForce.normalized * -1;

        //var closestPlanet = GameObject
        //                .FindGameObjectsWithTag("Planet")
        //    .Select(x => new
        //    {
        //        Vector = gameObject.transform.position - x.transform.position,
        //        Planet = x
        //    })
        //    .OrderBy(x => x.Vector.magnitude - x.Planet.GetComponent<CircleCollider2D>().radius)
        //    .FirstOrDefault();

        //var targetVector = Vector3.RotateTowards(gameObject.transform.up, closestPlanet.Vector, MaxRotateAngle, Mathf.Infinity);

        //gameObject.transform.up = new Vector3(targetVector.x, targetVector.y).normalized;
    }
}
