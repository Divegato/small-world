using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class HexCoordinates
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
    }
}
