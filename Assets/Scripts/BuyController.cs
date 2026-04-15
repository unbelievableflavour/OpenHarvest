using System;
using UnityEngine;
using UnityEngine.UI;

public class BuyController : MonoBehaviour
{
    public Text buttonLabel;
    public Button button;
    public Button backButton;
    public StoreItemsLister storeItemsLister;

    private HarvestDataTypes.StoreProduct item;
    private NPCController npc;
    public void SetNPC(NPCController newNPC)
    {
        npc = newNPC;
        npc.gaveItem += handleNPCGaveItem;
    }

    public void SetItem(HarvestDataTypes.StoreProduct newItem)
    {
        item = newItem;
        RefreshButton();
    }

    public void BuyItem()
    {
        RefreshButton();

        if (!button.interactable){
            return;
        }

        GameState.Instance.DecreaseMoneyByAmount(item.buyPrice);
        AudioManager.Instance.PlayClip("buy");
        if (storeItemsLister && ShouldSpawnBoughtProduct(item))
        {
            SpawnInNPCHand(item);
        }

        if (item.isUnlockable) {
            string unlockableKey = GetUnlockableKey(item);
            if (!string.IsNullOrWhiteSpace(unlockableKey))
            {
                GameState.Instance.unlock(unlockableKey, 1);
            }
        }

        string ownedCountKey = GetOwnedCountKey(item);
        if (!string.IsNullOrWhiteSpace(ownedCountKey) && (!item.isUnlockable || ownedCountKey != GetUnlockableKey(item)))
        {
            GameState.Instance.unlock(ownedCountKey, 1);
        }

        RefreshButton();
    }

    private void RefreshButton()
    {
        button.interactable = true;
        string ownedCountKey = GetOwnedCountKey(item);

        if (!string.IsNullOrWhiteSpace(item.dependsOnId) && !GameState.Instance.isUnlocked(item.dependsOnId))
        {
            setButtonToDependsOnOtherItem(item.dependsOnId);
            return;
        }

        if (!string.IsNullOrWhiteSpace(ownedCountKey) &&
            GameState.Instance.isUnlocked(ownedCountKey) &&
            GameState.Instance.ownsMaximumNumber(ownedCountKey, item.maximumTimesOwned))
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

    private void setButtonToDependsOnOtherItem(string dependsOnId)
    {
        button.interactable = false;
        buttonLabel.text = "Buy " + dependsOnId + " first!";
    }

    private void setButtonToAlreadyBought()
    {
        button.interactable = false;
        buttonLabel.text = "Maximum owned amount of item reached";
    }

    private void setButtonToNotEnoughMoney()
    {
        button.interactable = false;
        buttonLabel.text = "Not enough money (" + item.buyPrice + ")";
    }

    private void setButtonPrice()
    {
        buttonLabel.text = "Buy (" + item.buyPrice + ")";
    }

    private bool hasEnoughMoney()
    {
        return (GameState.Instance.getTotalAmount() - item.buyPrice) >= 0;
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

    public void UnlockStore()
    {
        Refresh();
    }

    public void LockStore(HarvestDataTypes.StoreProduct currentBoughtItem)
    {
        LockStoreItem(currentBoughtItem);
    }

    public void SpawnInNPCHand(HarvestDataTypes.StoreProduct item)
    {
        backButton.interactable = false;
        string ownedCountKey = GetOwnedCountKey(item);
        int currentlyOwnedCount = (!string.IsNullOrWhiteSpace(ownedCountKey) && GameState.Instance.isUnlocked(ownedCountKey))
            ? GameState.Instance.unlockables[ownedCountKey]
            : 0;
        if (item.maximumTimesOwned == currentlyOwnedCount + 1 || item.prefab == null)
        {
            LockStore(new HarvestDataTypes.StoreProduct());
        }
        else
        {
            LockStore(item);
        }

        if (item.sourceItem != null)
        {
            npc.GiveItem(item.sourceItem);
        }
        else
        {
            npc.GiveStoreProduct(item);
        }
    }

    private static string GetUnlockableKey(HarvestDataTypes.StoreProduct product)
    {
        if (product == null)
        {
            return string.Empty;
        }

        return string.IsNullOrWhiteSpace(product.unlockableId) ? product.id : product.unlockableId;
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

        return GetUnlockableKey(product);
    }

    private static bool ShouldSpawnBoughtProduct(HarvestDataTypes.StoreProduct product)
    {
        return product != null && product.sourceUnlockableDefinition == null;
    }

    private void handleNPCGaveItem(object sender, BNG.Grabbable grabbable)
    {
        backButton.interactable = true;
        UnlockStore();
        npc.BackToIdle();
    }
}
