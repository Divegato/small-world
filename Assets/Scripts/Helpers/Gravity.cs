using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class Gravity
    {
        public const float GravitationalConstant = 100;
        public static float MaxTime = 0;

        public static Vector2 GetAverageGravitationalForce(Vector2 centerOfMass, float mass, bool showDebug = false)
        {
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            var gravitySources = Object.FindObjectsOfType<PlanetarySystem>();

            var averageForce = Vector2.zero;
            foreach (var source in gravitySources)
            {
                if ((centerOfMass - source.CenterOfMass).magnitude > source.RadiusOfInfluence)
                {
                    if (showDebug) Debug.DrawLine(centerOfMass, source.CenterOfMass, Color.gray);
                    averageForce += CalculateGravity(centerOfMass, mass, source.CenterOfMass, source.Mass);
                }
                else
                {
                    foreach (var body in source.Bodies)
                    {
                        if ((centerOfMass - body.CenterOfMass).magnitude > body.RadiusOfInfluence)
                        {
                            if (showDebug) Debug.DrawLine(centerOfMass, body.CenterOfMass, Color.gray);
                            averageForce += CalculateGravity(centerOfMass, mass, body.CenterOfMass, body.Mass);
                        }
                        else
                        {
                            foreach (var chunk in body.Chunks)
                            {
                                if ((centerOfMass - chunk.CenterOfMass).magnitude > chunk.RadiusOfInfluence)
                                {
                                    if (showDebug) Debug.DrawLine(centerOfMass, chunk.CenterOfMass, Color.gray);
                                    averageForce += CalculateGravity(centerOfMass, mass, chunk.CenterOfMass, chunk.Mass);
                                }
                                else
                                {
                                    foreach (var tile in chunk.Tiles)
                                    {
                                        var location = chunk.Map.CellToWorld((Vector3Int)tile.GridPosition);
                                        if (showDebug) Debug.DrawLine(centerOfMass, location, Color.gray);
                                        averageForce += CalculateGravity(centerOfMass, mass, location, tile.Mass);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            stopWatch.Stop();
            System.TimeSpan ts = stopWatch.Elapsed;

            if (ts.Milliseconds > MaxTime)
            {
                MaxTime = ts.Milliseconds;
                Debug.LogWarning("Time to process gravity: " + MaxTime);
            }

            return averageForce;
        }

        public static Vector2 CalculateGravity(Vector2 targetPosition, float targetMass, Vector2 centerOfMass, float mass)
        {
            var distance = centerOfMass - targetPosition;

            var multiplier = 1f;
            if (distance.magnitude < 1)
            {
                multiplier = 0;
            }

            var force = GravitationalConstant * (Mathf.Max(1f, targetMass) * Mathf.Max(1f, mass) / Mathf.Pow(Mathf.Max(1f, distance.magnitude), 2f));

            return multiplier * distance.normalized * force;
        }
    }
}
