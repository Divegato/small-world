using System.Linq;
using Assets.Scripts.Models;
using Assets.Scripts.Structures;
using UnityEngine;
using UnityEngine.UI;

public class RecipePanel : MonoBehaviour
{
    private RecipeBook Recipes;

    private ForgeStructure Forge;
    private Button BuildButton;
    private Recipe SelectedRecipe;

    void Start()
    {
        Recipes = new RecipeBook();
        BuildButton = GetComponentsInChildren<Button>(true).First(x => x.name == "Build Button");
    }

    void Update()
    {

    }

    public void OnChange(Dropdown change)
    {
        // TODO: Match on ID instead of display name
        var matchingRecipes = Recipes.Recipes.Where(x => x.Value.DisplayName == change.captionText.text).ToList();

        SelectedRecipe = matchingRecipes.Select(x => x.Value).FirstOrDefault();
        BuildButton.enabled = matchingRecipes.Any();
    }

    public void OpenPanel(ForgeStructure forge)
    {
        Forge = forge;
        gameObject.SetActive(true);
    }

    public void SelectRecipe()
    {
        Forge.BuildRecipe(SelectedRecipe);
        gameObject.SetActive(false);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        Forge = null;
    }
}
