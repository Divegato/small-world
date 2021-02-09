using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Structures
{
    public class StorageStructure : Structure
    {
        private Vector3 position;

        public override void Start(BuildNetwork network)
        {
        }

        public override void Update(BuildNetwork network)
        {
            position = Parent.CellToWorld((Vector3Int)GridLocation);
        }

        public override void DrawDebugInfo()
        {
            Handles.Label(position, "Empty");
        }
    }
}
