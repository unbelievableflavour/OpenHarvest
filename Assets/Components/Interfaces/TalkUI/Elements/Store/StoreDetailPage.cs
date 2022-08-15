using UnityEngine;

public class StoreDetailPage : MonoBehaviour
{
    public AutoType heading;
    public AutoType description;
    public ItemPreviewer itemPreviewer;
    public BuyController buyController;

    private void UpdateInformation(HarvestDataTypes.Item item)
    {
        heading.ResetText(item.name);
        description.ResetText(item.description);
        itemPreviewer.Spawn(item);
    }

    public void SetItem(HarvestDataTypes.Item item)
    {
        buyController.SetItem(item);
        UpdateInformation(item);
    }
}
