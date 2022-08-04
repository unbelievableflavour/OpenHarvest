using UnityEngine;

public class StoreDetailPage : MonoBehaviour
{
    public AutoType heading;
    public AutoType description;
    public ItemPreviewer itemPreviewer;
    public BuyController buyController;

    private void UpdateInformation(Item item)
    {
        heading.ResetText(item.name);
        description.ResetText(item.description);
        itemPreviewer.Spawn(item);
    }

    public void SetItem(Item item)
    {
        buyController.SetItem(item);
        UpdateInformation(item);
    }
}
