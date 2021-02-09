using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class Gravity
    {
        private const float GravitationalConstant = 100;

        public static Vector2 GetAverageGravitationalForce(Rigidbody2D target)
        {
            var gravitySources = Object.FindObjectsOfType<GravitySource>();

            var averageForce = Vector2.zero;
            foreach (var source in gravitySources)
            {
                averageForce += source.GetForce(target);
            }

            return averageForce;
        }

        public static Vector2 CalculateGravity(Vector2 targetPosition, float targetMass, Vector2 centerOfMass, float mass)
        {
            var distance = centerOfMass - targetPosition;

            if (distance.magnitude < 1)
            {
                return Vector2.zero;
            }

            var force = GravitationalConstant * (Mathf.Max(1f, targetMass) * Mathf.Max(1f, mass) / Mathf.Pow(Mathf.Max(1f, distance.magnitude), 2f));

            return distance.normalized * force;
        }
    }
}
