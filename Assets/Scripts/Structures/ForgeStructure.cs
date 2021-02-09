using Assets.Scripts.Models;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Structures
{
    public class ForgeStructure : Structure
    {
        public Recipe Recipe;

        private Vector3 position;

        public Recipe Storage;

        private float Progress;

        private bool Paid;

        private float PowerPerSecond = 1;

        public override void Update(BuildNetwork network)
        {
            position = Parent.CellToWorld((Vector3Int)GridLocation);

            if (Storage == null && Recipe != null)
            {
                if (!Paid)
                {
                    Pay(Recipe, network);
                }

                if (network.TryToPower(PowerPerSecond * Time.deltaTime))
                {
                    Progress += Time.deltaTime;

                    if (Progress > Recipe.SecondsToBuild)
                    {
                        Storage = Recipe;
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

        public void BuildRecipe(Recipe recipe)
        {
            if (Progress > 0 && Recipe != null)
            {
                Refund(Recipe);
            }

            Progress = 0;
            Paid = false;
            Recipe = recipe;
        }

        private void Pay(Recipe recipe, BuildNetwork network)
        {
            // TODO: Pay for Recipe
        }

        private void Refund(Recipe recipe)
        {
            // TODO: Refund Recipe, probably need access to the build network
            Paid = false;
        }

        public override void DrawDebugInfo()
        {
            if (Recipe != null)
            {
                var percent = Progress / Recipe.SecondsToBuild;
                Handles.Label(position, string.Format("{0} {1} {2}", Recipe.DisplayName, percent.ToString("P"), Storage == null ? "" : "Complete"));
            }
        }
    }
}
