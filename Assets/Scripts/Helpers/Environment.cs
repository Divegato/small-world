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

        public static Collider2D[] GetNearbyPlanet(Collider2D target, float radius = 5)
        {
            var result = Physics2D
                .OverlapCircleAll(target.transform.position, 5)
                .Where(x => x.tag == "Item" || x.tag == "Planet")
                .ToArray();

            return result;
        }
    }
}
