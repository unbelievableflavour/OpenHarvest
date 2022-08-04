using System;
using UnityEngine;
using UnityEngine.UI;
using static Definitions;

public class GrowthState : MonoBehaviour
{
    [Header("Health")]
    public int health = 3;
    public HealthIndicator healthIndicator;

    [Header("States")]
    public Transform growthStates;

    [Header("References")]
    public ObjectSpawner objectSpawner;
    public SpawnManager spawnManager;
    private SoilBehaviour soilBehaviour;

    private int currentState = 0;

    void Awake()
    {
        healthIndicator.gameObject.SetActive(false);

        if (currentState == 0)
        {
            growthStates.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void Water()
    {
        if (spawnManager.getIsFed())
        {
            return;
        }

        if (isAtFinalGrowthState())
        {
            if (!objectSpawner) 
            {
                return;
            }
            if (objectSpawner.GetCurrentNumberOfSpawnedObjects() > 0) 
            { 
                return;
            }
        }

        spawnManager.Spawn();
    }

    public void Grow()
    {
        if (soilBehaviour)
        {
            soilBehaviour.setWatered(false);
            soilBehaviour.setCurrentState(SoilStates.Sowed);
        };

        int newState = isAtFinalGrowthState() ? currentState : currentState + 1;
        setCurrentState(newState);

        if (isAtFinalGrowthState())
        {
            if (objectSpawner)
            {
                objectSpawner.SpawnFruit();
            }
            return;
        }
    }

    private void DeactivateAllStates()
    {
        healthIndicator.gameObject.SetActive(false);

        foreach (Transform growthState in growthStates)
        {
            growthState.gameObject.SetActive(false);
        }
    }

    public int getCurrentState()
    {
        return currentState;
    }

    public void setCurrentState(int newState)
    {
        DeactivateAllStates();

        currentState = newState;
        growthStates.GetChild(currentState).gameObject.SetActive(true);

        if (isAtFinalGrowthState())
        {
            healthIndicator.gameObject.SetActive(true);
            healthIndicator.setNumberOfHearts(health);
            return;
        }
    }

    public void SetSoilBehaviour(SoilBehaviour soilBehaviour) {
        this.soilBehaviour = soilBehaviour;
    }

    public void resetPlantIfLastFruit()
    {
        if (health > 1)
        {
            SetHealth(health-1);
            if (soilBehaviour.getWatered())
            {
                Water();
            }
            
            return;
        }
        soilBehaviour.setCurrentState(SoilStates.Plowed);
        soilBehaviour.plantManager.Reset();
    }

    public void SetHealth(int newHealth)
    {
        if(newHealth < 0)
        {
            newHealth = 0;
        }

        health = newHealth;

        if (!healthIndicator)
        {
            return;
        }

        healthIndicator.setNumberOfHearts(newHealth);
    }

    private bool isAtFinalGrowthState()
    {
        return currentState == (growthStates.childCount - 1);
    }
}
