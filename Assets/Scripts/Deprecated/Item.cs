using static Definitions;

//Dont save files in this format. Use saveableItem.

[System.Serializable]
public class Item
{
    public string itemId;
    public string name;
    public string prefabFileName;
    public int buyPrice = 0;
    public int sellPrice = 0;
    public string description = "";
    public string DependsOnBeforeBuying;
    public int? maximumTimesOwned;

    // Deprecated
    public Items id;
}
