using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Models
{
    public class TileCursor
    {
        public Tilemap Map { get; set; }

        public Vector3Int Position { get; set; }

        public Sprite CurrentSprite { get; set; }
    }
}
