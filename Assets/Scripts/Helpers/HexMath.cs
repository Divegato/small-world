using System;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class HexMath
    {
        public static Vector3Int ConvertOffsetToCube(int row, int col)
        {
            var x = col - (row - (row & 1)) / 2;
            var z = row;
            var y = -x - z;

            return new Vector3Int(x, y, z);
        }

        public static float CubeDistance(Vector3Int a, Vector3Int b)
        {
            return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2;
        }

        public static int GetHexArea(int radius)
        {
            return 3 * (int)Math.Pow(radius, 2) + (3 * radius) + 1;
        }

        public static int GetHexShiftForChunks(int radius)
        {
            return (3 * radius) + 2;
        }

        public static Vector3Int HexToChunk(Vector3Int coordinate, int area, int shift)
        {
            var xh = Divide(coordinate.y + shift * coordinate.x, area);
            var yh = Divide(coordinate.z + shift * coordinate.y, area);
            var zh = Divide(coordinate.x + shift * coordinate.z, area);
            var i = Divide(1 + xh - yh, 3);
            var j = Divide(1 + yh - zh, 3);
            var k = Divide(1 + zh - xh, 3);
            return new Vector3Int(i, j, k);
        }

        private static int Divide(int a, int b)
        {
            return (a / b - Convert.ToInt32(((a < 0) ^ (b < 0)) && (a % b != 0)));
        }
    }
}
