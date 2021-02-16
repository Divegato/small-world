using UnityEngine;

namespace Assets.Scripts.Models
{
    public class TileCursor
    {
        public HexChunk Chunk { get; set; }

        public Vector3Int Position { get; set; }

        public Sprite CurrentSprite { get; set; }
    }
}
