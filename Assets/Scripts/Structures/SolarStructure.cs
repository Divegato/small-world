using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Structures
{
    public class SolarStructure : Structure
    {
        public float MaxCharge = 10;
        public float ElectricCharge = 0;
        public float ChargeRatePerSecond = 1;

        private Vector3 solarPosition;
        private float chargeRate = 0;

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
            solarPosition = Parent.CellToWorld((Vector3Int)GridLocation);

            if (ElectricCharge < MaxCharge)
            {
                var sun = Object.FindObjectOfType<Sun>();
                var sunDirection = (sun.transform.position - solarPosition).normalized;
                var startPosition = solarPosition + (sunDirection * 1.33f);
                var hit = Physics2D.Raycast(startPosition, sunDirection);
                Debug.DrawRay(startPosition, sunDirection, Color.blue);

                if (hit.collider.gameObject.tag == "Sun")
                {
                    chargeRate = Time.deltaTime * ChargeRatePerSecond;
                    ElectricCharge += chargeRate;

                    if (ElectricCharge > MaxCharge)
                    {
                        ElectricCharge = MaxCharge;
                    }
                }
            }

            if (ElectricCharge > 0)
            {
                var batteries = network
                    .Structures
                    .OfType<BatteryStructure>()
                    .Where(x => x.ElectricCharge < x.MaxCharge)
                    .OrderByDescending(x => x.MaxCharge - x.ElectricCharge);

                foreach (var battery in batteries)
                {
                    var charge = Mathf.Min(ElectricCharge, battery.MaxCharge - battery.ElectricCharge);
                    battery.ElectricCharge += charge;
                    ElectricCharge -= charge;

                    if (Mathf.Abs(ElectricCharge) < float.Epsilon)
                    {
                        break;
                    }
                }
            }
        }

        public override void DrawDebugInfo()
        {
            Handles.Label(solarPosition, ElectricCharge.ToString("n1"));
        }
    }
}
