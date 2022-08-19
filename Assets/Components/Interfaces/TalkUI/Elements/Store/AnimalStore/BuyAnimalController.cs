using UnityEngine;
using UnityEngine.UI;

public class BuyAnimalController : MonoBehaviour
{
    public Text buttonLabel;
    public Button button;
    public HarvestDataTypes.Item item;

    private StoreItemsLister storeItemsLister;

    public void SetItem(HarvestDataTypes.Item item)
    {
        button.interactable = true;
        this.item = item;

        if (item.DependsOnBeforeBuyingItem != null && !GameState.Instance.isUnlocked(item.DependsOnBeforeBuyingItem.itemId))
        {
            setButtonToDependsOnOtherItem(item.DependsOnBeforeBuyingItem);
            return;
        }

        if (isAlreadyUnlocked() && hasBoughtMaximum())
        {
            setButtonToAlreadyBought();
            return;
        }

        if (!hasEnoughMoney())
        {
            setButtonToNotEnoughMoney();
            return;
        }

        setButtonPrice();
    }

    public void BuyItem()
    {
        if (isAlreadyUnlocked() && hasBoughtMaximum())
        {
            setButtonToAlreadyBought();
            return;
        }

        if (!hasEnoughMoney())
        {
            setButtonToNotEnoughMoney();
            return;
        }

        storeItemsLister.InitialisePet(item);
    }

    public bool hasBoughtMaximum()
    {
        if (item.maximumTimesOwned == null)
        {
            return false;
        }

        return GameState.Instance.unlockables[item.itemId] >= item.maximumTimesOwned;
    }

    private bool isAlreadyUnlocked()
    {
        return GameState.Instance.isUnlocked(item.itemId);
    }

    private void setButtonToDependsOnOtherItem(HarvestDataTypes.Item item)
    {
        button.interactable = false;
        var newText = buttonLabel;
        newText.text = "Buy " + item.name + " first!";
    }

    private void setButtonToAlreadyBought()
    {
        button.interactable = false;
        var newText = buttonLabel;
        newText.text = "Maximum owned amount of item reached";
    }

    private void setButtonToNotEnoughMoney()
    {
        button.interactable = false;
        var newText = buttonLabel;
        newText.text = "Not enough money (" + item.buyPrice + ")";
    }

    private bool hasEnoughMoney()
    {
        return (GameState.Instance.getTotalAmount() - item.buyPrice) >= 0;
    }

    private void setButtonPrice()
    {
        var newText = buttonLabel;
        newText.text = "Buy (" + item.buyPrice + ")";
    }

    public void UpdateItemDetailsLabel()
    {
        StoreItemInformation.SetItem(item);
    }

    public void ResetItemDetailsLabel()
    {
        StoreItemInformation.Reset();
    }

    public void Refresh()
    {
        SetItem(item);
    }

    public void SetItemLister(StoreItemsLister newStoreItemsLister)
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
