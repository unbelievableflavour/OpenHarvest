using UnityEngine;
using UnityEngine.UI;

public class BuyAnimalController : MonoBehaviour
{
    public Text buttonLabel;
    public Button button;
    public HarvestDataTypes.StoreProduct item;

    private StoreItemsLister storeItemsLister;

    public void SetItem(HarvestDataTypes.StoreProduct item)
    {
        button.interactable = true;
        this.item = item;

        if (!string.IsNullOrWhiteSpace(item.dependsOnId) && !GameState.Instance.isUnlocked(item.dependsOnId))
        {
            setButtonToDependsOnOtherItem(item.dependsOnId);
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
        string ownedCountKey = GetOwnedCountKey(item);
        return GameState.Instance.ownsMaximumNumber(ownedCountKey, item.maximumTimesOwned);
    }

    private bool isAlreadyUnlocked()
    {
        string ownedCountKey = GetOwnedCountKey(item);
        return GameState.Instance.isUnlocked(ownedCountKey);
    }

    private void setButtonToDependsOnOtherItem(string dependsOnId)
    {
        button.interactable = false;
        var newText = buttonLabel;
        newText.text = "Buy " + dependsOnId + " first!";
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

    private static string GetOwnedCountKey(HarvestDataTypes.StoreProduct product)
    {
        if (product == null)
        {
            return string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(product.ownedCountId))
        {
            return product.ownedCountId;
        }

        return string.IsNullOrWhiteSpace(product.unlockableId) ? product.id : product.unlockableId;
    }
}
