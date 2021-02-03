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

        public static float GetArea(Vector2[] points)
        {
            float temp = 0;
            int i = 0;
            for (; i < points.Length; i++)
            {
                if (i != points.Length - 1)
                {
                    float mulA = points[i].x * points[i + 1].y;
                    float mulB = points[i + 1].x * points[i].y;
                    temp = temp + (mulA - mulB);
                }
                else
                {
                    float mulA = points[i].x * points[0].y;
                    float mulB = points[0].x * points[i].y;
                    temp = temp + (mulA - mulB);
                }
            }
            temp *= 0.5f;
            return Mathf.Abs(temp);
        }

        public static Vector2 GetCenter(Vector2[] points)
        {
            var total = Vector2.zero;

            foreach (var point in points)
            {
                total += point;
            }

            return total / points.Length;
        }

        public static Vector2[] GetCircle(float radius, int granularity, float variation = 0)
        {
            var points = new Vector2[granularity];

            for (int i = 0; i < granularity; i++)
            {
                var angle = 2 * Mathf.PI / granularity * i;
                var length = (radius + Random.Range(variation * -1, variation));
                var x = Mathf.Cos(angle) * length;
                var y = Mathf.Sin(angle) * length;

                points[i] = new Vector2(x, y);
            }

            return points;
        }
    }
}
