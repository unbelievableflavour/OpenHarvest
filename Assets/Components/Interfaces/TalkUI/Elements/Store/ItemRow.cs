using UnityEngine;
using UnityEngine.UI;
using static Definitions;

public class ItemRow : MonoBehaviour
{
    public Text buttonLabel;
    public Button button;

    private Item item;
    private StoreItemsLister storeItemsLister;

    public void SetItem(Item item)
    {
        this.item = item;
        RefreshButton();
    }

    private void RefreshButton()
    {
        button.interactable = true;

        if (item.DependsOnBeforeBuying != "" && !GameState.isUnlocked(item.DependsOnBeforeBuying))
        {
            setButtonToDependsOnOtherItem(item.DependsOnBeforeBuying);
            return;
        }

        if (GameState.isUnlocked(item.itemId) && GameState.ownsMaximumNumber(item))
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

    private void setButtonToDependsOnOtherItem(string itemId)
    {
        Item itemInfo = GetItemInformation(itemId);
        buttonLabel.text = "Buy " + itemInfo.name + " first!";
    }

    private bool hasEnoughMoney()
    {
        return (GameState.getTotalAmount() - item.buyPrice) >= 0;
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

    public void LockStoreItem(Item currentBoughtItem)
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
