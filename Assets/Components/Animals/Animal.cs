using System;
using System.Runtime.Serialization;
using static Definitions;

[Serializable]
public class Animal
{
    public string plateauInstanceId;
    public string name;
    public DateTime? bornTimestamp;
    public DateTime? lastTimeFedTimestamp;

    [OptionalField]
    public Items id; // DEPRECATED — kept for save migration only
}
