using BNG;
using UnityEngine;
using static Definitions;
using HarvestDataTypes;

public class MixingBowl : CookingBase
{
    public RecipeDatabase recipeDatabase;
    public RecipeMechanic mechanicWater;
    public RecipeMechanic mechanicStir;
    public CookingTool cookingTool;
    public ParticleSystem spawnParticles;

    private RecipeMechanic activeMechanic;
    private Recipe activeRecipe;
    private bool isSpawningItem = false;

    public void AddWater()
    {
        activeMechanic = mechanicWater;
        recipes = recipeDatabase.getAllForMechanic(activeMechanic);
        StartCooking();
    }

    public void Stir()
    {
        activeMechanic = mechanicStir;
        recipes = recipeDatabase.getAllForMechanic(activeMechanic);
        StartCooking();
    }

    public void StartCooking()
    {
        if (isSpawningItem)
        {
            return;
        }

        var recipe = GetRecipeFromCookingTool(cookingTool);

        if (recipe == null)
        {
            return;
        }

        activeRecipe = recipe;
        spawnParticles.Play();
        SpawnDoneItem();
    }

    public void StopCooking()
    {
        if (isSpawningItem)
        {
            return;
        }

        tooltip.SetActive(false);
        activeRecipe = null;
    }

    private void SpawnDoneItem()
    {
        isSpawningItem = true;
        var spawnedFood = InstantiateItemNew(activeRecipe.item.prefab);
        RemoveOldItems(cookingTool);
        cookingTool.snapZones[0].GrabGrabbable(spawnedFood.GetComponent<Grabbable>());
        isSpawningItem = false;
    }
}
