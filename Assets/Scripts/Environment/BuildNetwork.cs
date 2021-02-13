using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models;
using Assets.Scripts.Structures;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildNetwork : MonoBehaviour
{
    public float MaxCharge = 0;
    public int MaxQuantity = 100;

    public List<Structure> Structures = new List<Structure>();

    // TODO: Handle by player build network
    public List<MaterialQuantity> Storage = new List<MaterialQuantity> { new MaterialQuantity { Id = "stone", Quantity = 0 } };

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

    public bool TryAddMaterial(MaterialQuantity material)
    {
        bool success = false;

        var storage = Structures.OfType<StorageStructure>().ToArray();

        // TODO: Stone storage should be part of player build network
        if (material.Id == "stone")
        {
            if (Storage.Single().Quantity + material.Quantity <= MaxQuantity)
            {
                Storage.Single().Quantity += material.Quantity;
                success = true;
            }
        }

        if (!success)
        {
            foreach (var chest in storage)
            {
                if (chest.TryToStore(material))
                {
                    success = true;
                    break;
                }
            }
        }

        if (success)
        {
            UpdateHud();
        }

        return success;
    }

    private void UpdateHud()
    {
        var hud = FindObjectOfType<InventoryDisplay>();
        var storage = Structures.OfType<StorageStructure>();

        var summary = Storage
            .Union(storage.SelectMany(x => x.Storage))
            .GroupBy(x => x.Id)
            .Select(x => new MaterialQuantity
            {
                Id = x.Key,
                Quantity = x.Sum(y => y.Quantity)
            })
            .ToList();

        hud.UpdateText(summary);
    }

    public bool TrySpendMaterial(List<MaterialQuantity> materials)
    {
        bool haveMaterials = true;

        var storage = Storage.Union(Structures.OfType<StorageStructure>().SelectMany(x => x.Storage));

        foreach (var material in materials)
        {
            bool found = false;
            foreach (var storageMaterial in storage)
            {
                if (storageMaterial.Id == material.Id && storageMaterial.Quantity >= material.Quantity)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                haveMaterials = false;
                break;
            }
        }

        if (haveMaterials)
        {
            foreach (var material in materials)
            {
                foreach (var storageMaterial in storage)
                {
                    if (storageMaterial.Id == material.Id && storageMaterial.Quantity >= material.Quantity)
                    {
                        storageMaterial.Quantity -= material.Quantity;
                        break;
                    }
                }
            }

            UpdateHud();
        }

        return haveMaterials;
    }

    public void AddStructure(Structure structure)
    {
        Structures.Add(structure);
    }

    public Structure GetStructure(Tilemap parent, Vector2Int location)
    {
        return Structures.FirstOrDefault(x => x.Parent == parent && x.GridLocation == location);
    }

    public bool TryRemoveStructure(Tilemap parent, Vector2Int location)
    {
        var structure = GetStructure(parent, location);

        if (structure != null)
        {
            // TODO: Special handling for removing a storage chest

            if (TryAddMaterial(new MaterialQuantity { Id = structure.GetRecipe().Id, Quantity = 1 }))
            {
                structure.Stop(this);

                Structures.Remove(structure);

                return true;
            }
        }

        return false;
    }

    public void ActivateStructure(Tilemap parent, Vector2Int location)
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
            return TrySpendMaterial(ForgeStructure.Recipe.Cost);
        }

        // TODO: bypass the first solar panel build another way
        if (!Structures.OfType<SolarStructure>().Any() && recipe.Id == "solar")
        {
            return TrySpendMaterial(SolarStructure.Recipe.Cost);
        }

        if (TrySpendMaterial(new List<MaterialQuantity> { new MaterialQuantity { Id = recipe.Id, Quantity = 1 } }))
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
