using System.Linq;
using UnityEngine;

public class SelfLevelingController : MonoBehaviour
{
    public float MaxRotateAngle = 0.02f;
    public float MaxRotateMagnitude = 0.02f;

    void Start()
    {

    }

    void Update()
    {
        var planets = GameObject.FindGameObjectsWithTag("Planet");

        //For starters we'll just use closest planet
        var closestPlanet = planets
            .Select(x => gameObject.transform.position - x.transform.position)
            .OrderBy(x => x.magnitude)
            .FirstOrDefault();

        var targetVector = Vector3.RotateTowards(gameObject.transform.up, closestPlanet, MaxRotateAngle, MaxRotateMagnitude);
        gameObject.transform.up = new Vector3(targetVector.x, targetVector.y);

        // Find planet with the strongest gravity effect (or maybe the average gravity effect?)

        // Apply a light rotational force so up is opposite to that gravitational force
    }
}
