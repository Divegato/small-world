using System.Linq;
using Assets.Scripts.Helpers;
using UnityEngine;

public class StartOnPlanet : MonoBehaviour
{
    private bool initialized = false;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            initialized = false;
        }

        if (!initialized)
        {
            var planets = FindObjectsOfType<Planet>();

            if (planets.Any())
            {
                var planet = planets[Random.Range(0, planets.Length)];
                gameObject.transform.position = planet.transform.position + Geometry.GetRandomPointOnCircle(planet.Radius * 1.1f);

                initialized = true;
            }
        }
    }
}
