using System;
using static Definitions;

[System.Serializable]
public class SaveableWindmill
{
    public WindmillStates state;
    public int currentWheatCount;
    public int currentFlourCount;
    public DateTime? startingTimestamp;
}
