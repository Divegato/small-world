using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class Environment
    {
        public static bool IsGrounded(GameObject target)
        {
            var result = Physics2D.OverlapCircleAll(target.transform.position, 1);

            return result.Any(x => x.tag != "Player");
        }
    }
}
