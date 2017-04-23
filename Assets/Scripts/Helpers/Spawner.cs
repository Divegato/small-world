using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class Spawner
    {
        public static GameObject BuildBlock(GameObject block, Vector3 target)
        {
            var closest = GameObject
                .FindGameObjectsWithTag("Item")
                .Select(x => new
                {
                    Item = x,
                    Distance = (x.transform.position - target).magnitude - (x.transform.localScale.x / 2)
                })
                .Where(x => x.Distance < 1.5);

            if (closest.All(x => x.Distance > 0))
            {
                var newItem = Object.Instantiate(block, target, Quaternion.identity);

                foreach (var otherItem in closest)
                {
                    var joint = newItem.AddComponent<FixedJoint2D>();
                    joint.autoConfigureConnectedAnchor = true;
                    joint.connectedBody = otherItem.Item.GetComponent<Rigidbody2D>();
                    joint.breakForce = 400;
                    joint.breakTorque = 300;
                }

                return newItem;
            }

            return null;
        }
    }
}
