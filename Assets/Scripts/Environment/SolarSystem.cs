using Assets.Scripts.Helpers;
using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    public float SunRadius = 1000;
    public int PlanetCount = 10;

    void Start()
    {
        GenerateSolarSystem();
    }

    private void Update()
    {
    }

    private void GenerateSolarSystem()
    {
        var orbitDistance = 1000;
        for (int i = 0; i < PlanetCount; i++)
        {
            var radius = Random.Range(10, 500);
            orbitDistance += radius + Random.Range(1000, 10000);
            var center = Geometry.GetRandomPointOnCircle(orbitDistance);

            GenerateCelestialBody.Generate(new CelestialBody
            {
                Name = "Planet-" + i,
                Type = CelestialBodyType.Planet,
                Radius = radius
            }, center);

            // x moons per planet
        }

        // - asteroids
    }
}
