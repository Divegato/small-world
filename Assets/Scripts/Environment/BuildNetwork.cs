using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models;
using Assets.Scripts.Structures;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildNetwork : MonoBehaviour
{
    public float MaxCharge = 0;

    public List<Structure> Structures = new List<Structure>();

    void Start()
    {
        Structures = new List<Structure>();
    }

    void Update()
    {
        foreach (var structure in Structures)
        {
            structure.Update(this);
        }
    }

    public void AddStructure(Structure structure)
    {
        Structures.Add(structure);
    }

    public Structure GetStructure(Tilemap parent, Vector2Int location)
    {
        return Structures.FirstOrDefault(x => x.Parent == parent && x.GridLocation == location);
    }

    public void RemoveStructure(Tilemap parent, Vector2Int location)
    {
        var structure = GetStructure(parent, location);

        if (structure != null)
        {
            structure.Stop(this);

            Structures.Remove(structure);
        }
    }

    internal void ActivateStructure(Tilemap parent, Vector2Int location)
    {
        var structure = GetStructure(parent, location);

        if (structure != null)
        {
            structure.Activate(this);
        }
    }

    void OnDrawGizmos()
    {
#if DEBUG
        foreach (var structure in Structures)
        {
            structure.DrawDebugInfo();
        }
#endif
    }

    public bool TryToPower(float electricCharge)
    {
        //TODO: Optimize performance. This is executed each frame * number of power using objects
        var battery = Structures
            .OfType<BatteryStructure>()
            .Where(x => x.ElectricCharge > electricCharge)
            .OrderByDescending(x => x.ElectricCharge)
            .FirstOrDefault();

        if (battery != null)
        {
            battery.ElectricCharge -= electricCharge;
            return true;
        }
        else
        {
            var solarPanel = Structures
                .OfType<SolarStructure>()
                .Where(x => x.ElectricCharge > electricCharge)
                .OrderByDescending(x => x.ElectricCharge)
                .FirstOrDefault();

            if (solarPanel != null)
            {
                solarPanel.ElectricCharge -= electricCharge;
                return true;
            }
        }

        return false;
    }

    public bool TrySpend(Recipe recipe)
    {
        var forges = Structures.OfType<ForgeStructure>().ToArray();

        // TODO: bypass the first forge build another way
        if (!forges.Any() && recipe.Id == "forge")
        {
            return true;
        }

        // TODO: bypass the first solar panel build another way
        if (!Structures.OfType<SolarStructure>().Any() && recipe.Id == "solar")
        {
            return true;
        }

        foreach (var structure in forges.Where(x => x.Storage != null))
        {
            if (structure.Storage.Id == recipe.Id)
            {
                structure.Storage = null;
                return true;
            }
        }

        return false;
    }
}
