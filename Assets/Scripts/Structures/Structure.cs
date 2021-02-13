using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Structure
{
    public abstract Recipe GetRecipe();

    public Tilemap Parent { get; set; }

    public Vector2Int GridLocation { get; set; }

    public Recipe Definition { get; set; }

    public virtual void Start(BuildNetwork network) { }

    public virtual void Update(BuildNetwork network) { }

    public virtual void Stop(BuildNetwork network) { }

    public virtual void Activate(BuildNetwork network) { }

    public virtual void DrawDebugInfo() { }
}
