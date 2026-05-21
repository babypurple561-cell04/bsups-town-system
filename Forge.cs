using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Forge building - crafts items and tools.
/// </summary>
public class Forge : Building
{
    [SerializeField] private Transform recipePanel;
    [SerializeField] private Text recipeListText;
    [SerializeField] private Text selectedRecipeText;
    [SerializeField] private Button craftButton;

    private List<Recipe> availableRecipes = new List<Recipe>();
    private Recipe selectedRecipe;

    protected override void Start()
    {
        buildingName = "Forge";
        buildingDescription = "A place to craft tools and items.";
        InitializeRecipes();
        base.Start();
    }

    /// <summary>
    /// Initialize available recipes
    /// </summary>
    private void InitializeRecipes()
    {
        availableRecipes.Add(new Recipe
        {
            recipeName = "Wooden Axe",
            requiredMaterials = new List<string> { "Wood", "Wood" },
            craftTime = 2f,
            resultItem = "Wooden Axe"
        });

        availableRecipes.Add(new Recipe
        {
            recipeName = "Copper Pickaxe",
            requiredMaterials = new List<string> { "Copper Ore", "Wood" },
            craftTime = 3f,
            resultItem = "Copper Pickaxe"
        });

        availableRecipes.Add(new Recipe
        {
            recipeName = "Bronze Sword",
            requiredMaterials = new List<string> { "Copper Ore", "Tin Ore" },
            craftTime = 5f,
            resultItem = "Bronze Sword"
        });
    }

    /// <summary>
    /// Display available recipes
    /// </summary>
    public void DisplayRecipes()
    {
        if (recipeListText == null) return;

        string recipeList = "Available Recipes:\n";
        for (int i = 0; i < availableRecipes.Count; i++)
        {
            recipeList += $"{i + 1}. {availableRecipes[i].recipeName}\n";
        }
        recipeListText.text = recipeList;
    }

    /// <summary>
    /// Select a recipe to view details
    /// </summary>
    public void SelectRecipe(int recipeIndex)
    {
        if (recipeIndex < 0 || recipeIndex >= availableRecipes.Count)
            return;

        selectedRecipe = availableRecipes[recipeIndex];
        
        if (selectedRecipeText != null)
        {
            string materials = "";
            foreach (string material in selectedRecipe.requiredMaterials)
            {
                materials += $"{material}, ";
            }
            materials = materials.TrimEnd(',', ' ');

            selectedRecipeText.text = $"<b>{selectedRecipe.recipeName}</b>\n\nMaterials: {materials}\nCraft Time: {selectedRecipe.craftTime}s\nResult: {selectedRecipe.resultItem}";
        }
    }

    /// <summary>
    /// Craft the selected recipe
    /// </summary>
    public void CraftItem()
    {
        if (selectedRecipe == null)
        {
            Debug.LogWarning("No recipe selected!");
            return;
        }

        // Check if player has required materials
        if (HasRequiredMaterials(selectedRecipe))
        {
            StartCoroutine(CraftingProcess(selectedRecipe));
        }
        else
        {
            Debug.LogWarning("Not enough materials to craft!");
        }
    }

    /// <summary>
    /// Check if player has required materials
    /// </summary>
    private bool HasRequiredMaterials(Recipe recipe)
    {
        // This would check the player's inventory
        // For now, we'll assume they have materials
        return true;
    }

    /// <summary>
    /// Simulate crafting process
    /// </summary>
    private System.Collections.IEnumerator CraftingProcess(Recipe recipe)
    {
        Debug.Log($"Crafting {recipe.recipeName}...");
        yield return new WaitForSeconds(recipe.craftTime);
        Debug.Log($"Crafting complete! Created {recipe.resultItem}");
        // Add item to player inventory
    }

    public override void Open()
    {
        base.Open();
        DisplayRecipes();
    }
}