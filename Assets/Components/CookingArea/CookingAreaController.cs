using BNG;
using System.Collections;
using UnityEngine;
using static Definitions;
using HarvestDataTypes;

public class CookingAreaController : CookingBase
{
    public RecipeDatabase recipeDatabase;
    public RecipeMechanic mechanic;
    public AudioSource cookingSound;
    public GameObject fireParticles;
    public GameObject smokeParticles;

    private float cookingTimeFrom = 5.0f;
    private float cookingTimeTill = 10.0f;
    private float timeBeforeBurned = 2.0f;

    private int loaderState = 0;

    public SnapZone cookingSnapZone;

    public bool doorShouldBeClosed = false;
    private bool doorIsClosed = false;

    private CookingStates m_currentState = CookingStates.notCookingHere;

    private Recipe activeRecipe;
    private CookingTool activeCookingTool;

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
            fireParticles.SetActive(true);
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

    public void SnapPan()
    {
        if (!cookingSnapZone.HeldItem)
        {
            return;
        }

        if (doorShouldBeClosed && !doorIsClosed)
        {
            tooltip.SetActive(true);
            tooltipText.text = "Door should be closed";
            return;
        }

        activeCookingTool = cookingSnapZone.HeldItem.GetComponent<CookingTool>();

        if(activeCookingTool == null)
        {
            return;
        }

        var recipe = GetRecipeFromCookingTool(activeCookingTool);

        if (recipe == null)
        {
            return;
        }

        activeRecipe = recipe;
        m_currentState = CookingStates.cookingShouldBeInitiated;
    }

    public void UnsnapPan()
    {
        m_currentState = CookingStates.notCookingHere;
        tooltip.SetActive(false);

        activeRecipe = null;
        activeCookingTool = null;

        fireParticles.SetActive(false);
        smokeParticles.SetActive(false);
        cookingSound.Stop();
        CancelInvoke("SetLoader");
        return;
    }

    public void OpenDoor()
    {
        doorIsClosed = false;
        UnsnapPan();
    }

    public void CloseDoor()
    {
        doorIsClosed = true;
        SnapPan();
    }

    private void SpawnFailedRecipeItem()
    {
        if (activeRecipe.failedItem)
        {
            var spawnedFood = InstantiateItemNew(activeRecipe.failedItem.prefab);
            RemoveOldItems(activeCookingTool);
            activeCookingTool.snapZones[0].GrabGrabbable(spawnedFood.GetComponent<Grabbable>());
            return;
        }

        RemoveOldItems(activeCookingTool);
    }

    private void SpawnDoneRecipeItem()
    {
        var spawnedFood = InstantiateItemNew(activeRecipe.item.prefab);
        RemoveOldItems(activeCookingTool);
        activeCookingTool.snapZones[0].GrabGrabbable(spawnedFood.GetComponent<Grabbable>());
    }
}
