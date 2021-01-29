using UnityEngine;

public static class GenerateCelestialBody
{
    public static GameObject Generate(CelestialBody model, Vector3 position)
    {
        var body = new GameObject(model.Name);
        body.transform.position = position;

        var planet = body.AddComponent<Planet>();
        planet.Radius = model.Radius;

        var rotation = body.AddComponent<Rotation>();
        rotation.RotationsPerMinute = Random.Range(0f, 10f);

        if (model.Type == CelestialBodyType.Planet)
        {
            //var type = Random.Range(0, 2);

            //switch (type)
            //{
            //    case 0:
            //        GeneratePlanet.GenerateCoreWithSlicesPlanet(model.Radius, body.transform);
            //        break;
            //    case 1:
            HexTilePlanet.Generate(model.Radius, body.transform);
            //        break;
            //}
        }

        return body;
    }
}
