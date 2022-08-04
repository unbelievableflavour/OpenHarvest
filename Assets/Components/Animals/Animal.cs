using System;
using static Definitions;

[Serializable]
public class Animal
{
    public string name; 
    public DateTime? bornTimestamp;
    public DateTime? lastTimeFedTimestamp;

    public Items id; // DEPRECATED
}