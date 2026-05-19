using System;
using static Definitions;

[System.Serializable]
public class SaveablePatch
{
    public SoilStates patchState;
    public string selectedPlant;
    public int growthState2;
    public int numberOfCurrentlySpawnedFruits;
    public bool isWatered;
    public DateTime? wateredTimestamp;
    public int health;

    //DEPRECATED
    public PlantStates growthState;
}