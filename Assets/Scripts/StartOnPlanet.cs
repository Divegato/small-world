using System.Linq;
using Assets.Scripts.Helpers;
using UnityEngine;

public class StartOnPlanet : MonoBehaviour
{
    private bool playerPlaced = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            playerPlaced = false;
        }

        if (!playerPlaced)
        {
            PlacePlayer();
        }
    }

    private void PlacePlayer()
    {
        var planets = FindObjectsOfType<Planet>().Where(x => x.name.StartsWith("Planet")).ToArray();

        if (planets.Any())
        {
            var planet = planets[Random.Range(0, planets.Length)];
            gameObject.transform.position = planet.transform.position + Geometry.GetRandomPointOnCircle(planet.Radius * 1.1f);
            playerPlaced = true;
        }
    }
}
