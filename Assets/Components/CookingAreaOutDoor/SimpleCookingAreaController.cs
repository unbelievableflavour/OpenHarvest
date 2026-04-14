using BNG;
using System.Collections;
using UnityEngine;
using static Definitions;
using HarvestDataTypes;

public class SimpleCookingAreaController : CookingBase
{
    public RecipeDatabase recipeDatabase;
    public RecipeMechanic mechanic;
    public CookingTool cookingTool;
    public AudioSource cookingSound;
    public GameObject smokeParticles;

    private float cookingTimeFrom = 5.0f;
    private float cookingTimeTill = 10.0f;
    private float timeBeforeBurned = 2.0f;
    private int loaderState = 0;

    public bool doorShouldBeClosed = false;
    private bool doorIsClosed = false;

    private CookingStates m_currentState = CookingStates.notCookingHere;
    private Recipe activeRecipe;
    private bool isSpawningRecipeItem = false;

    public void Start()
    {
        recipes = recipeDatabase.getAllForMechanic(mechanic);
    }

    void Update()
    {
        if (m_currentState == CookingStates.notCookingHere)
        {
            return;
        }

        if (m_currentState == CookingStates.cookingShouldBeInitiated)
        {
            m_currentState = CookingStates.cooking;

            tooltip.SetActive(true);
            cookingSound.Play();
            InvokeRepeating("SetLoader", 0f, 1.0f);
            StartCoroutine(InitiateCooking());
            return;
        }
    }

    private IEnumerator InitiateCooking()
    {
        yield return new WaitForSeconds(Random.Range(cookingTimeFrom, cookingTimeTill));
        if (m_currentState == CookingStates.cooking)
        {
            m_currentState = CookingStates.done;
            tooltipText.text = "!";
            SpawnDoneRecipeItem();
            if (activeRecipe.onFail == OnFail.spawnFailItem)
            {
                StartCoroutine(InitiateTooLate());
            }
        }
    }


    private IEnumerator InitiateTooLate()
    {
        yield return new WaitForSeconds(timeBeforeBurned);
        if (m_currentState == CookingStates.done)
        {
            m_currentState = CookingStates.burned;
            smokeParticles.SetActive(true);
            tooltipText.text = "Too late";
            SpawnFailedRecipeItem();
        }
    }

    void SetLoader()
    {
        if (m_currentState == CookingStates.done)
        {
            CancelInvoke("SetLoader");
            return;
        }

        if (loaderState == 0)
        {
            tooltipText.text = ".";
            loaderState = 1;
            return;
        }

        if (loaderState == 1)
        {
            tooltipText.text = "..";
            loaderState = 2;
            return;
        }

        if (loaderState == 2)
        {
            tooltipText.text = "...";
            loaderState = 0;
            return;
        }
    }

    public void StartCooking()
    {
        if (isSpawningRecipeItem)
        {
            return;
        }

        if (doorShouldBeClosed && !doorIsClosed)
        {
            tooltip.SetActive(true);
            tooltipText.text = "Door should be closed";
            return;
        }

        var recipe = GetRecipeFromCookingTool(cookingTool);

        if (recipe == null)
        {
            return;
        }

        activeRecipe = recipe;
        m_currentState = CookingStates.cookingShouldBeInitiated;
    }

    public void StopCooking()
    {
        if (isSpawningRecipeItem)
        {
            return;
        }

        m_currentState = CookingStates.notCookingHere;
        tooltip.SetActive(false);

        activeRecipe = null;

        smokeParticles.SetActive(false);
        cookingSound.Stop();
        CancelInvoke("SetLoader");
        return;
    }

    public void OpenDoor()
    {
        doorIsClosed = false; 
        StopCooking();
    }

    public void CloseDoor()
    {
        doorIsClosed = true;
        StartCooking();
    }

    private void SpawnFailedRecipeItem()
    {
        isSpawningRecipeItem = true;

        if (!activeRecipe.failedItem)
        {
            RemoveOldItems(cookingTool);
            isSpawningRecipeItem = false;
            StopCooking();
            return;
        }

        var spawnedFood = InstantiateItemNew(activeRecipe.failedItem.prefab);
        RemoveOldItems(cookingTool);
        cookingTool.snapZones[0].GrabGrabbable(spawnedFood.GetComponent<Grabbable>());
        isSpawningRecipeItem = false;
    }

    private void SpawnDoneRecipeItem()
    {
        isSpawningRecipeItem = true;
        var spawnedFood = InstantiateItemNew(activeRecipe.item.prefab);
        RemoveOldItems(cookingTool);
        cookingTool.snapZones[0].GrabGrabbable(spawnedFood.GetComponent<Grabbable>());
        isSpawningRecipeItem = false;
    }
}
