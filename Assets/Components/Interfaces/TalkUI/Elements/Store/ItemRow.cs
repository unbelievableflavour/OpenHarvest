using UnityEngine;
using UnityEngine.UI;

public class ItemRow : MonoBehaviour
{
    public Text buttonLabel;
    public Button button;

    private HarvestDataTypes.StoreProduct item;
    private StoreItemsLister storeItemsLister;

    public void SetItem(HarvestDataTypes.StoreProduct item)
    {
        this.item = item;
        RefreshButton();
    }

    private void RefreshButton()
    {
        button.interactable = true;

        if (!string.IsNullOrWhiteSpace(item.dependsOnId) && !GameState.Instance.isUnlocked(item.dependsOnId))
        {
            setButtonToDependsOnOtherItem(item.dependsOnId);
            return;
        }

        if (GameState.Instance.isUnlocked(item.id) && GameState.Instance.ownsMaximumNumber(item.id, item.maximumTimesOwned))
        {
            buttonLabel.text = "Maximum owned amount of item reached";
            return;
        }

        if (!hasEnoughMoney())
        {
            buttonLabel.text = "Not enough money (" + item.buyPrice + ")";
            return;
        }

        buttonLabel.text = "Buy (" + item.buyPrice + ")";
    }

    private void setButtonToDependsOnOtherItem(string dependsOnId)
    {
        buttonLabel.text = "Buy " + dependsOnId + " first!";
    }

    private bool hasEnoughMoney()
    {
        return (GameState.Instance.getTotalAmount() - item.buyPrice) >= 0;
    }

    public void GoToDetailPage()
    {
        storeItemsLister.storeDetailPage.SetItem(item);
        storeItemsLister.viewSwitcher.setActiveView("detail");
    }

    public void UpdateItemDetailsLabel()
    {
        storeItemsLister.itemPreviewer.Spawn(item);
    }

    public void ResetItemDetailsLabel()
    {
        storeItemsLister.itemPreviewer.Spawn((HarvestDataTypes.StoreProduct)null);
    }

    public void Refresh()
    {
        SetItem(item);
    }

    public void SetStoreItemsLister(StoreItemsLister newStoreItemsLister)
    {
        storeItemsLister = newStoreItemsLister;
    }

    public void LockStoreItem(HarvestDataTypes.StoreProduct currentBoughtItem)
    {
        if (item == currentBoughtItem)
        {
            return;
        }
        button.interactable = false;
        var newText = buttonLabel;
        newText.text = "Pickup bought item first";
    }
}
