using UnityEngine;

public class StoreDetailPage : MonoBehaviour
{
    public AutoType heading;
    public AutoType description;
    public ItemPreviewer itemPreviewer;
    public BuyController buyController;

    private void UpdateInformation(HarvestDataTypes.StoreProduct item)
    {
        heading.ResetText(item.displayName);
        description.ResetText(item.description);
        itemPreviewer.Spawn(item);
    }

    public void SetItem(HarvestDataTypes.StoreProduct item)
    {
        buyController.SetItem(item);
        UpdateInformation(item);
    }
}
