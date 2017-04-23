using System.Linq;
using UnityEngine;

public class SelfLevelingController : MonoBehaviour
{
    public float MaxRotateAngle = 0.25f;

    void Start()
    {

    }

    void Update()
    {
        var closestPlanet = GameObject
            .FindGameObjectsWithTag("Planet")
            .Select(x => new
            {
                Vector = gameObject.transform.position - x.transform.position,
                Planet = x
            })
            .OrderBy(x => x.Vector.magnitude - x.Planet.GetComponent<CircleCollider2D>().radius)
            .FirstOrDefault();

        var targetVector = Vector3.RotateTowards(gameObject.transform.up, closestPlanet.Vector, MaxRotateAngle, Mathf.Infinity);

        gameObject.transform.up = new Vector3(targetVector.x, targetVector.y).normalized;
    }
}
