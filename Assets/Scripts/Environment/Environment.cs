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

        public static Collider2D GetClosestBlock(Collider2D target)
        {
            var result = Physics2D.OverlapCircleAll(target.transform.position, 5);

            var closest = result
                .Where(x => x.tag == "Item")
                .OrderBy(t => (t.Distance(target).distance)).FirstOrDefault();

            return closest;
        }

        public static Collider2D[] GetNearbyBlocks(Collider2D target, float radius = 5)
        {
            var result = Physics2D
                .OverlapCircleAll(target.transform.position, 5)
                .Where(x => x.tag == "Item")
                .ToArray();

            return result;
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
