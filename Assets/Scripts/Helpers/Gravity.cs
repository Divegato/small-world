using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class Gravity
    {
        public const float GravitationalConstant = 100;

        public static Vector2 GetAverageGravitationalForce(Rigidbody2D target)
        {
            var gravitySources = Object.FindObjectsOfType<GravitySource>();
            // TODO: Probably don't want to use Rigidbody2D to get these values.
            var targetGravity = new GravitySource { CenterOfMass = target.centerOfMass + target.position, GravityPower = target.mass };

            var averageForce = Vector2.zero;
            foreach (var source in gravitySources)
            {
                averageForce += source.GetForce(targetGravity);
            }

            return averageForce;
        }

        public static Vector2 CalculateGravity(Vector2 targetPosition, float targetMass, Vector2 centerOfMass, float mass)
        {
            var distance = centerOfMass - targetPosition;

            // TODO: This isn't granular enough at close distances.
            var multiplier = 1f;
            if (distance.magnitude < 50)
            {
                multiplier = Mathf.Exp(-1 * (50f - distance.magnitude) / 2);
            }

            var force = GravitationalConstant * (Mathf.Max(1f, targetMass) * Mathf.Max(1f, mass) / Mathf.Pow(Mathf.Max(1f, distance.magnitude), 2f));

            return multiplier * distance.normalized * force;
        }
    }
}
