using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexChunk : MonoBehaviour
{
    public float RadiusOfInfluence = 40;

    public Vector2 CenterOfMass;

    public float Mass;

    public List<HexTile> Tiles = new List<HexTile>();

    public Tilemap Map;

    public CelestialBody Parent;

    public Collider2D Collider;

    void Start()
    {
        CenterOfMass = transform.position;
        Collider = GetComponent<CompositeCollider2D>();
    }

    void Update()
    {
        CenterOfMass = Collider.bounds.center;
    }

    public void AddTile(Vector2Int position, TileBase tile, float mass = 1)
    {
        Tiles.Add(new HexTile { GridPosition = position, Mass = mass });
        Map.SetTile((Vector3Int)position, tile);
        Mass += mass;

        // TODO: Adjust Center of Mass

        Parent.UpdateMass(mass);
    }

    public void RemoveTile(Vector2Int position)
    {
        var tile = Tiles.Find(x => x.GridPosition == position);
        Map.SetTile((Vector3Int)position, null);
        Mass -= tile.Mass;
        Tiles.Remove(tile);

        // TODO: Adjust Center of Mass

        Parent.UpdateMass(-1 * tile.Mass);
    }

    void OnDrawGizmos()
    {
#if DEBUG
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.Label(CenterOfMass, Mass.ToString());
#endif
    }
}
