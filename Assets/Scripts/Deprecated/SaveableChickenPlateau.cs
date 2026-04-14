using System;
using static Definitions;

[System.Serializable]
public class SaveableChickenPlateau
{
    //public ChickenPlateauStates state;
    public int numberOfCurrentlySpawnedObjects;
    public bool isFed;
    public DateTime? startingTimestamp;
}
