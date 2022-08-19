using System;
using UnityEngine;
using UnityEngine.UI;

public class BuyController : MonoBehaviour
{
    public Text buttonLabel;
    public Button button;
    public Button backButton;
    public StoreItemsLister storeItemsLister;

    private HarvestDataTypes.Item item;
    private NPCController npc;
    public void SetNPC(NPCController newNPC)
    {
        npc = newNPC;
        npc.gaveItem += handleNPCGaveItem;
    }

    public void SetItem(HarvestDataTypes.Item newItem)
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
        if (storeItemsLister)
        {
            SpawnInNPCHand(item);
        }

        if (item.isUnlockable) {
            GameState.Instance.unlock(item.itemId, 1);
            return;
        }

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

    private void setButtonToDependsOnOtherItem(HarvestDataTypes.Item item)
    {
        button.interactable = false;
        buttonLabel.text = "Buy " + item.name + " first!";
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

    public void UnlockStore()
    {
        Refresh();
    }

    public void LockStore(HarvestDataTypes.Item currentBoughtItem)
    {
        LockStoreItem(currentBoughtItem);
    }

    public void SpawnInNPCHand(HarvestDataTypes.Item item)
    {
        backButton.interactable = false;
        var itemId = item.itemId;
        int currentlyOwnedCount = (item.isUnlockable && GameState.Instance.isUnlocked(itemId)) ? GameState.Instance.unlockables[itemId] : 0;
        if (item.maximumTimesOwned == currentlyOwnedCount + 1 || item.prefab == null)
        {
            LockStore(new HarvestDataTypes.Item());
        }
        else
        {
            LockStore(item);
        }

        if (item.prefab == null || itemId == "BackpackBig")
        {
            npc.GiveUnlockable(item);
            return;
        }

        npc.GiveItem(item);
    }

    private void handleNPCGaveItem(object sender, BNG.Grabbable grabbable)
    {
        backButton.interactable = true;
        UnlockStore();
        npc.BackToIdle();
    }
}
