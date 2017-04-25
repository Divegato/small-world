using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class Environment
    {
        public static bool IsGrounded(GameObject target)
        {
            var result = Physics2D.OverlapCircleAll(target.transform.position, 5);

            return result.Any(x => x.tag != "Player");
        }

        public static Vector2 GetAverageGravitationalForce(Rigidbody2D target)
        {
            var gravitySources = Object.FindObjectsOfType<Gravity>();

            var averageForce = Vector2.zero;
            foreach (var source in gravitySources)
            {
                averageForce += source.GetForce(target);
            }

            return averageForce;
        }
    }
}
