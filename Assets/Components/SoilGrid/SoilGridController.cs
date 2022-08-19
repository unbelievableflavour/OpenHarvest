using System.Collections.Generic;
using UnityEngine;
using System;

public class SoilGridController : MonoBehaviour
{
    private List<SaveablePatch> patches = new List<SaveablePatch>();
    private Transform farmSoilGrid;

    public string patchLocation;

    void Start()
    {
        SceneSwitcher.Instance.beforeSceneSwitch += beforeSceneSwitch;
        farmSoilGrid = GetComponent<Transform>();
        if(patchLocation == "greenhouse")
        {
            // It's a dirty hack I know.
            patchLocation = GameState.Instance.enteredSceneThrough;
        }

        LoadGrid();
    }

    public void LoadGrid()
    {
        patches = GameState.Instance.soilGrids[patchLocation];

        for (int i = 0; i < patches.Count; i++)
        {
            if (farmSoilGrid.childCount <= i)
            {
                continue;
            }

            SaveablePatch saveablePatch = patches[i];

            var child = farmSoilGrid.GetChild(i).gameObject.GetComponent<SoilBehaviour>();
            if (!child)
            {
                continue;
            }
            child.setWatered(saveablePatch.isWatered);
            child.setCurrentState(saveablePatch.patchState);

            if (string.IsNullOrEmpty(saveablePatch.selectedPlant) || saveablePatch.selectedPlant == "None") // second line might not be necessary anymore
            {
                continue;
            }
    
            var plantManager = child.plantManager;
            plantManager.SpawnPlant(saveablePatch.selectedPlant);

            var growthState = plantManager.getSelectedPlantGrowthState();
            var spawnManager = growthState.spawnManager;
            
            growthState.setCurrentState(saveablePatch.growthState2);
            spawnManager.setStartingTimestamp(saveablePatch.wateredTimestamp);
            spawnManager.setIsFed(saveablePatch.isWatered);
            growthState.SetHealth(saveablePatch.health);

            if (saveablePatch.numberOfCurrentlySpawnedFruits != 0)
            {
                plantManager.getSelectedPlantObjectSpawner().SpawnSpecificNumberOfFruits(saveablePatch.numberOfCurrentlySpawnedFruits);
            }
        }
    }

    public void UpdateSaveableGrid()
    {
        patches.Clear();

        int i = 0;
        foreach (Transform targetGameObject in farmSoilGrid)
        {
            SoilBehaviour target = targetGameObject.GetComponent<SoilBehaviour>();
            if (!target)
            {
                continue;
            }

            var growthState = target.plantManager.getSelectedPlantGrowthState();
            var objectSpawner = target.plantManager.getSelectedPlantObjectSpawner();

            SaveablePatch saveablePatch = new SaveablePatch();
            saveablePatch.patchState = target.getCurrentState();
            saveablePatch.isWatered = target.getWatered();
            saveablePatch.selectedPlant = target.plantManager.getSelectedPlant();
            saveablePatch.growthState2 = growthState ? growthState.getCurrentState() : 0;
            saveablePatch.wateredTimestamp = growthState ? growthState.spawnManager.getStartingTimestamp() : null;
            saveablePatch.numberOfCurrentlySpawnedFruits = objectSpawner ? objectSpawner.GetCurrentNumberOfSpawnedObjects() : 0;
            saveablePatch.health = growthState ? growthState.health : 3;
            patches.Add(saveablePatch);

            i++;
        }

        GameState.Instance.soilGrids[patchLocation] = patches;
    }

    protected void beforeSceneSwitch(object sender, EventArgs e)
    {
        SceneSwitcher.Instance.beforeSceneSwitch -= beforeSceneSwitch;
        UpdateSaveableGrid();
    }
}
