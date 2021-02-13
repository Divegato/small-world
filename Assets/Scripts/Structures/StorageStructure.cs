using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Structures
{
    public class StorageStructure : Structure
    {
        public static Recipe Recipe = new Recipe
        {
            Id = "storage",
            DisplayName = "Storage",
            SecondsToBuild = 30,
            SpriteName = "hex-tiles_2",
            StructureClass = typeof(StorageStructure),
            Cost = new List<MaterialQuantity>
            {
                new MaterialQuantity { Id = "stone", Quantity = 10 }
            }
        };

        public override Recipe GetRecipe()
        {
            return Recipe;
        }

        public List<MaterialQuantity> Storage = new List<MaterialQuantity>();

        private Vector3 position;

        public override void Start(BuildNetwork network)
        {
        }

        public override void Update(BuildNetwork network)
        {
            position = Parent.CellToWorld((Vector3Int)GridLocation);
        }

        public override void DrawDebugInfo()
        {
            Handles.Label(position, string.Join(Environment.NewLine, Storage.Select(x => x.Id + ": " + x.Quantity)));
        }

        public bool TryToStore(MaterialQuantity material)
        {
            // TODO: Set Storage Limits
            bool stacked = false;
            foreach (var item in Storage)
            {
                if (item.Id == material.Id)
                {
                    item.Quantity += material.Quantity;
                    stacked = true;
                    break;
                }
            }

            if (!stacked)
            {
                Storage.Add(material);
                stacked = true;
            }

            return stacked;
        }
    }
}
