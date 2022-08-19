using UnityEngine;
using UnityEngine.UI;

public class ItemRow : MonoBehaviour
{
    public Text buttonLabel;
    public Button button;

    private HarvestDataTypes.Item item;
    private StoreItemsLister storeItemsLister;

    public void SetItem(HarvestDataTypes.Item item)
    {
        this.item = item;
        RefreshButton();
    }

    private void RefreshButton()
    {
        button.interactable = true;

        if (item.DependsOnBeforeBuyingItem != null && !GameState.Instance.isUnlocked(item.DependsOnBeforeBuyingItem.itemId))
        {
            setButtonToDependsOnOtherItem(item.DependsOnBeforeBuyingItem);
            return;
        }

        if (GameState.Instance.isUnlocked(item.itemId) && GameState.Instance.ownsMaximumNumber(item))
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

    private void setButtonToDependsOnOtherItem(HarvestDataTypes.Item item)
    {
        buttonLabel.text = "Buy " + item.name + " first!";
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
        storeItemsLister.itemPreviewer.Spawn(null);
    }

    public void Refresh()
    {
        SetItem(item);
    }

    public void SetStoreItemsLister(StoreItemsLister newStoreItemsLister)
    {
        storeItemsLister = newStoreItemsLister;
    }

    public void LockStoreItem(HarvestDataTypes.Item currentBoughtItem)
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
