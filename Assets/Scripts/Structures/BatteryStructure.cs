using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Structures
{
    public class BatteryStructure : Structure
    {
        public float MaxCharge = 1000;
        public float ElectricCharge = 0;

        private Vector3 position;

        public override void Start(BuildNetwork network)
        {
            network.MaxCharge += MaxCharge;
        }

        public override void Stop(BuildNetwork network)
        {
            network.MaxCharge -= MaxCharge;
        }

        public override void Update(BuildNetwork network)
        {
            position = Parent.CellToWorld((Vector3Int)GridLocation);
        }

        public override void DrawDebugInfo()
        {
            Handles.Label(position, ElectricCharge.ToString("n1"));
        }
    }
}
