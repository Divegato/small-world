using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class Geometry
    {
        public static Vector3 GetRandomPointOnCircle(float radius)
        {
            return GetRandomPointOnCircle(Vector3.zero, radius);
        }

        public static Vector3 GetRandomPointOnCircle(Vector3 center, float radius)
        {
            var angle = Random.value * Mathf.PI * 2;

            var x = (Mathf.Cos(angle) * radius) + center.x;
            var y = (Mathf.Sin(angle) * radius) + center.y;

            return new Vector3(x, y, 0);
        }
    }
}
