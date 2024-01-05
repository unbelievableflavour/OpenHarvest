using BNG;
using System.Collections;
using UnityEngine;
using static Definitions;
using HarvestDataTypes;
using UnityEngine.UI;

public class SmithingAreaController : CookingBase
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

    public void Start(){
        Reset();
    }

    public void StartCooking()
    {
        if (activeRecipe == null)
        {
            tooltipText.text = "Select a recipe.";
            return;
        }

        SetState(CookingStates.cooking);
    }

    public void PlaceIngredient(Grabbable grabbable) {
        if (isSpawningRecipeItem)
        {
            return;
        }

        if (grabbable.GetComponent<ItemStack>() && grabbable.GetComponent<ItemStack>().GetStackSize() > 1)
        {
            tooltipText.text = "Dont use item stacks on an anvil.";
            return;
        }

        recipesList.SetActive(true);
        var item = GetItemFromObject(grabbable);
        anvilRecipeSelector.RefreshListWithIngredient(item);
        StartCooking();
    }

    public void StopCooking()
    {
        if (isSpawningRecipeItem)
        {
            return;
        }

        Reset();
        return;
    }

    void SetState(CookingStates newState)
    { 
        m_currentState = newState;

        if(m_currentState == CookingStates.notCookingHere) {
            activeRecipe = null;
            tooltipText.text = "Place an ingredient to smith.";
            return;
        }

        if(m_currentState == CookingStates.cooking) {
            recipesList.SetActive(false);

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
        RemoveOldItems(cookingTool);
        cookingTool.snapZones[0].GrabGrabbable(spawnedFood.GetComponent<Grabbable>());
        isSpawningRecipeItem = false;
    }

    public void SetActiveRecipe(Recipe newActiveRecipe) {
        activeRecipe = newActiveRecipe;
        StartCooking();
    }

    private void Reset() {
        loaderState = 0;
        recipesList.SetActive(false);
        SetState(CookingStates.notCookingHere);
    }
}
