using System.Collections.Generic;
using Assets.Scripts.Models;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Structures
{
    public class ForgeStructure : Structure
    {
        public static Recipe Recipe = new Recipe
        {
            Id = "forge",
            DisplayName = "Forge",
            SecondsToBuild = 5,
            SpriteName = "hex-tiles_0",
            StructureClass = typeof(ForgeStructure),
            Cost = new List<MaterialQuantity>
            {
                new MaterialQuantity { Id = "stone", Quantity = 5 }
            }
        };

        public override Recipe GetRecipe()
        {
            return Recipe;
        }

        public Recipe SelectedRecipe;

        private Vector3 position;

        public Recipe Storage;

        private float Progress;

        private bool Paid;

        private float PowerPerSecond = 1;

        public override void Update(BuildNetwork network)
        {
            position = Parent.CellToWorld((Vector3Int)GridLocation);

            if (Storage == null && SelectedRecipe != null)
            {
                if (!Paid)
                {
                    if (network.TrySpendMaterial(SelectedRecipe.Cost))
                    {
                        Paid = true;
                    }
                }

                if (Paid && network.TryToPower(PowerPerSecond * Time.deltaTime))
                {
                    Progress += Time.deltaTime;

                    if (Progress > SelectedRecipe.SecondsToBuild)
                    {
                        Storage = SelectedRecipe;
                        Progress = 0;
                        Paid = false;
                    }
                }
            }
        }

        public override void Activate(BuildNetwork network)
        {
            var recipe = Object.FindObjectOfType<RecipePanel>(true);
            if (recipe != null)
            {
                recipe.OpenPanel(this);
            }
        }

        public void SelectRecipe(Recipe recipe)
        {
            if (Progress > 0 && SelectedRecipe != null)
            {
                Refund(SelectedRecipe);
            }

            Progress = 0;
            Paid = false;
            SelectedRecipe = recipe;
        }

        private void Refund(Recipe recipe)
        {
            // TODO: Refund Recipe, probably need access to the build network
            Paid = false;
        }

        public override void DrawDebugInfo()
        {
            if (SelectedRecipe != null)
            {
                var percent = Progress / SelectedRecipe.SecondsToBuild;
                Handles.Label(position, string.Format("{0} {1} {2}", SelectedRecipe.DisplayName, percent.ToString("P"), Storage == null ? "" : "Complete"));
            }
        }
    }
}
