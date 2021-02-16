using UnityEngine;

public static class GenerateCelestialBody
{
    public static GameObject Generate(CelestialBodyModel model, Vector3 position, PlanetarySystem parent)
    {
        var body = new GameObject(model.Name);
        body.transform.position = position;

        var rotation = body.AddComponent<Rotation>();
        rotation.RotationsPerMinute = Random.Range(0f, 10f);

        switch (model.Type)
        {
            case CelestialBodyType.Star:
                break;
            case CelestialBodyType.Planet:
                var planet = body.AddComponent<Planet>();
                planet.Radius = model.Radius;
                planet.RadiusOfInfluence = model.Radius * 10;
                planet.Parent = parent;
                HexTilePlanet.Generate(model.Radius, body.transform);
                break;
            case CelestialBodyType.Moon:
                var moon = body.AddComponent<CelestialBody>();
                moon.Radius = model.Radius;
                moon.Parent = parent;
                moon.RadiusOfInfluence = model.Radius * 10;
                HexTilePlanet.Generate(model.Radius, body.transform);
                break;
            default:
                break;
        }

        return body;
    }
}
