using BNG;
using System.Collections;
using UnityEngine;
using static Definitions;
using HarvestDataTypes;
using UnityEngine.UI;
using System.Collections.Generic;

public class CraftAreaController : CookingBase
{
    public RecipeMechanic mechanic;
    public CookingTool cookingTool;
    public ParticleSystem spawnEffect;
    public AnvilRecipeSelector anvilRecipeSelector;
    public GameObject recipesList;

    private float cookingTimeFrom = 5.0f;
    private float cookingTimeTill = 10.0f;
    private float timeBeforeBurned = 2.0f;
    private int loaderState = 0;

    private CookingStates m_currentState = CookingStates.notCookingHere;
    private Recipe activeRecipe;
    private bool isSpawningRecipeItem = false;

    public List<Grabbable> slottedGrabbables = new List<Grabbable>();

    public void Start() {
        Reset();
    }

    public void StartCooking()
    {
        foreach (Grabbable grabbable in slottedGrabbables)
        {
            if (grabbable.GetComponent<ItemStack>() && grabbable.GetComponent<ItemStack>().GetStackSize() > 1)
            {
                tooltipText.text = "Dont use item stacks here.";
                return;
            }
        }

        if (activeRecipe == null)
        {
            recipesList.SetActive(true);
            List<string> ingredientIds = new List<string>();
            foreach (Grabbable slottedGrabbable in slottedGrabbables)
            {
                ingredientIds.Add(GetItemFromObject(slottedGrabbable).itemId);
            }

            anvilRecipeSelector.RefreshListWithIngredients(ingredientIds); 

            tooltipText.text = "Select a recipe.";
            return;
        }
        
        recipesList.SetActive(false);
        SetState(CookingStates.cooking);
    }

    public void PlaceIngredient(Grabbable grabbable) {
        if (isSpawningRecipeItem)
        {
            return;
        }

        if(!slottedGrabbables.Contains(grabbable)) {
            slottedGrabbables.Add(grabbable);
        }

        Reset();
        StartCooking();
    }

    public void RemoveIngredient(Grabbable grabbable) {
        if (isSpawningRecipeItem)
        {
            return;
        }

        if(slottedGrabbables.Contains(grabbable)) {
            slottedGrabbables.Remove(grabbable);
        }

        Reset();

        if(slottedGrabbables.Count > 0) {
            StartCooking();
        }
    }

    void SetState(CookingStates newState)
    { 
        m_currentState = newState;

        if(m_currentState == CookingStates.notCookingHere) {
            tooltipText.text = "Place ingredients to start.";
            return;
        }

        if(m_currentState == CookingStates.cooking) {
            if (loaderState == 0)
            {
                tooltipText.text = "Ready! Hit it with the hammer.";
                loaderState = 1;
                return;
            }

            if (loaderState == 1)
            {
                tooltipText.text = ".";
                loaderState = 2;
                return;
            }

            if (loaderState == 2)
            {
                tooltipText.text = "..";
                loaderState = 3;
                return;
            }

            if (loaderState == 3)
            {
                tooltipText.text = "...";
                loaderState = 4;
                return;
            }

            if (loaderState == 4)
            {
                tooltipText.text = "!";
                SetState(CookingStates.done);
                return;
            }

            return;
        }

        if (m_currentState == CookingStates.done)
        {
            SpawnDoneRecipeItem();
            return;
        }
    }

    public void ProgressCooking()
    {
        if(m_currentState != CookingStates.cooking) {
            return;
        }
        SetState(CookingStates.cooking);
    }


    private void SpawnDoneRecipeItem()
    {
        spawnEffect.Play();
        isSpawningRecipeItem = true;
        var spawnedFood = InstantiateItemNew(activeRecipe.item.prefab);
        var spawnedGrabbable = spawnedFood.GetComponent<Grabbable>();
        RemoveOldItems(cookingTool);
        slottedGrabbables = new List<Grabbable>();
        slottedGrabbables.Add(spawnedGrabbable);
        cookingTool.snapZones[0].GrabGrabbable(spawnedGrabbable);
        isSpawningRecipeItem = false;
    }

    public void SetActiveRecipe(Recipe newActiveRecipe) {
        activeRecipe = newActiveRecipe;
        StartCooking();
    }

    private void Reset() {
        loaderState = 0;
        recipesList.SetActive(false);
        activeRecipe = null;
        SetState(CookingStates.notCookingHere);
    }
}
