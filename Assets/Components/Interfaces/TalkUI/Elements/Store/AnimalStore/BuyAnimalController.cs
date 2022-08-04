using UnityEngine;
using UnityEngine.UI;
using static Definitions;

public class BuyAnimalController : MonoBehaviour
{
    public Text buttonLabel;
    public Button button;
    public Item item;

    private StoreItemsLister storeItemsLister;

    public void SetItem(Item item)
    {
        button.interactable = true;
        this.item = item;

        if (item.DependsOnBeforeBuying != "" && !GameState.isUnlocked(item.DependsOnBeforeBuying))
        {
            setButtonToDependsOnOtherItem(item.DependsOnBeforeBuying);
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

        return GameState.unlockables[item.itemId] >= item.maximumTimesOwned;
    }

    private bool isAlreadyUnlocked()
    {
        return GameState.isUnlocked(item.itemId);
    }

    private void setButtonToDependsOnOtherItem(string itemId)
    {
        Item itemInfo = GetItemInformation(itemId);

        button.interactable = false;
        var newText = buttonLabel;
        newText.text = "Buy " + itemInfo.name + " first!";
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
        return (GameState.getTotalAmount() - item.buyPrice) >= 0;
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
