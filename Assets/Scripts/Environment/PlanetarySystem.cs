using UnityEditor;
using UnityEngine;

public class PlanetarySystem : MonoBehaviour
{
    public float Mass = 0;

    public float RadiusOfInfluence;

    public Vector2 CenterOfMass;

    public CelestialBody[] Bodies;

    public void Start()
    {
        Bodies = GetComponentsInChildren<CelestialBody>();
        CenterOfMass = transform.position;
    }

    public void Update()
    {
        // TODO: Update/Average celestialBodies.CenterOfMass based on celestialBodies.Mass
    }

    public void UpdateMass(float mass)
    {
        Mass += mass;
    }

    void OnDrawGizmos()
    {
#if DEBUG
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(CenterOfMass, Vector3.forward, RadiusOfInfluence);
        Handles.Label((Vector3)CenterOfMass + (Vector3.left * RadiusOfInfluence), Mass.ToString());
#endif
    }
}
