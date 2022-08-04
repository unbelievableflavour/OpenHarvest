using UnityEngine;
using static Definitions;

public class ItemInformation : MonoBehaviour {
    public HarvestDataTypes.Item item;

    public string getItemId()
    {
        return item.itemId;
    }

    public Item getItemInfo()
    {
        return GetItemInformation(item.itemId);
    }

    public HarvestDataTypes.Item getItem()
    {
        return item;
    }
}
