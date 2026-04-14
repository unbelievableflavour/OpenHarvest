using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Definitions;

public class ForcePlantState : MonoBehaviour
{
    public SoilStates soilState;
    public int plantState;
    public string plantName = "Tomato";
    public bool spawnFruits = false;
    public bool runOnStart = false;

    void Start()
    {
        if(runOnStart){
            SetState();
        }
    }

    public void SetState()
    {
        var soilBehaviour = GetComponent<SoilBehaviour>();
        soilBehaviour.setCurrentState(soilState, false);
        soilBehaviour.plantManager.Reset();
        soilBehaviour.setWatered(false);

        if (soilState == SoilStates.Sowed) {
            soilBehaviour.plantManager.SpawnPlant(plantName);
            soilBehaviour.plantManager.getSelectedPlantGrowthState().setCurrentState(plantState);
        }

        if(!spawnFruits)
        {
            return;
           
        }
        if (soilBehaviour.plantManager.getSelectedPlantObjectSpawner())
        {
            soilBehaviour.plantManager.getSelectedPlantObjectSpawner().SpawnFruit();
        }
    }
}
