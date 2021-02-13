using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
    private Text reference;
    private RecipeBook recipeBook = new RecipeBook();

    void Start()
    {
        reference = GetComponent<Text>();
    }

    void Update()
    {
    }

    public void UpdateText(List<MaterialQuantity> materials)
    {
        reference.text = string.Join(Environment.NewLine, materials.Select(x => recipeBook.Recipes[x.Id].DisplayName + ": " + x.Quantity));
    }
}
