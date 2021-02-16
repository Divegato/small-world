using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public PlanetarySystem Parent;

    public float Radius;

    public float RadiusOfInfluence;

    public Vector2 CenterOfMass;

    public float Mass;

    public HexChunk[] Chunks;

    void Start()
    {
        Chunks = GetComponentsInChildren<HexChunk>();
        CenterOfMass = transform.position;
    }

    public void Update()
    {
        // TODO: Update/Average chunks.CenterOfMass based on chunks.Mass
    }

    public void UpdateMass(float mass)
    {
        Mass += mass;
        Parent.UpdateMass(mass);
    }

    void OnDrawGizmos()
    {
#if DEBUG
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(CenterOfMass, Vector3.forward, RadiusOfInfluence);
        UnityEditor.Handles.Label((Vector3)CenterOfMass + (Vector3.left * RadiusOfInfluence), Mass.ToString());
#endif
    }
}
