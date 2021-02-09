using System;
using UnityEngine.Tilemaps;

[Obsolete]
public class StructureDefinition
{
    public Type StructureType { get; set; }

    public string Name { get; set; }

    public Tile MapTile { get; set; }
}
