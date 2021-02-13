using System;
using System.Collections.Generic;
using Assets.Scripts.Structures;

namespace Assets.Scripts.Models
{
    public class RecipeBook
    {
        public RecipeBook()
        {
            Recipes = new Dictionary<string, Recipe>
            {
                { "stone", BuildRecipe("stone", "Stone", 0, "hex-tiles_8") },
                { "wall", BuildRecipe("wall", "Wall", 1, "hex-tiles_1") },
                { "forge", ForgeStructure.Recipe },
                { "storage", StorageStructure.Recipe },
                { "solar", SolarStructure.Recipe },
                { "battery", BatteryStructure.Recipe }
            };
        }

        private static Recipe BuildRecipe(string id, string displayName, float secondToBuild, string spriteName, Type structureType = null)
        {
            return new Recipe
            {
                Id = id,
                DisplayName = displayName,
                SecondsToBuild = secondToBuild,
                SpriteName = spriteName,
                StructureClass = structureType
            };
        }

        public Dictionary<string, Recipe> Recipes { get; set; }
    }
}
