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
            var radius = Random.Range(10, 150);
            orbitDistance += radius + Random.Range(1000, 2000);
            var center = Geometry.GetRandomPointOnCircle(orbitDistance);

            var planetarySystem = new GameObject("System-" + (i + 1));
            planetarySystem.transform.position = center;
            planetarySystem.transform.parent = transform;

            var system = planetarySystem.AddComponent<PlanetarySystem>();

            var planet = GenerateCelestialBody.Generate(new CelestialBodyModel
            {
                Name = "Planet-" + (i + 1),
                Type = CelestialBodyType.Planet,
                Radius = radius
            }, center, system);

            planet.transform.parent = planetarySystem.transform;

            var moons = Random.Range(0, 5);

            var moonOrbitDistance = radius * 4;
            for (int j = 0; j < moons; j++)
            {
                var moonRadius = Random.Range(5, radius / 4);
                moonOrbitDistance += radius + Random.Range(100, 200);
                orbitDistance += moonOrbitDistance;
                var moonCenter = Geometry.GetRandomPointOnCircle(moonOrbitDistance);

                var moon = GenerateCelestialBody.Generate(new CelestialBodyModel
                {
                    Name = "Moon-" + (i + 1) + "-" + (j + 1),
                    Type = CelestialBodyType.Moon,
                    Radius = moonRadius
                }, moonCenter + center, system);

                moon.transform.parent = planetarySystem.transform;
            }

            system.RadiusOfInfluence = moonOrbitDistance + 1000;
        }
    }
}
