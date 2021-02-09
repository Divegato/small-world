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
                { "wall", BuildRecipe("wall", "Wall", 1, "hex-tiles_1") },
                { "forge", BuildRecipe("forge", "Forge", 30, "hex-tiles_0", typeof(ForgeStructure)) },
                { "storage", BuildRecipe("storage", "Storage", 30, "hex-tiles_2", typeof(StorageStructure)) },
                { "solar", BuildRecipe("solar", "Solar Panel", 30, "hex-tiles_3", typeof(SolarStructure)) },
                { "battery", BuildRecipe("battery", "Battery", 30, "hex-tiles_4", typeof(BatteryStructure)) },
                { "portal", BuildRecipe("portal", "Portal", 600, "hex--") }
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
