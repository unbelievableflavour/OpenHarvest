using System;
using UnityEngine;
using UnityEngine.UI;
using static Definitions;

public class BuyController : MonoBehaviour
{
    public Text buttonLabel;
    public Button button;
    public Button backButton;
    public StoreItemsLister storeItemsLister;

    private Item item;
    private NPCController npc;
    public void SetNPC(NPCController newNPC)
    {
        npc = newNPC;
        npc.gaveItem += handleNPCGaveItem;
    }

    public void SetItem(Item newItem)
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

        GameState.DecreaseMoneyByAmount(item.buyPrice);
        AudioManager.Instance.PlayClip("buy");
        if (storeItemsLister)
        {
            SpawnInNPCHand(item);
        }

        if (GameState.isUnlockable(item.itemId)) {
            GameState.unlock(item.itemId, 1);
            return;
        }

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

    private void setButtonToDependsOnOtherItem(string itemId)
    {
        Item itemInfo = GetItemInformation(itemId);

        button.interactable = false;
        buttonLabel.text = "Buy " + itemInfo.name + " first!";
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
        return (GameState.getTotalAmount() - item.buyPrice) >= 0;
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

    public void UnlockStore()
    {
        Refresh();
    }

    public void LockStore(Item currentBoughtItem)
    {
        LockStoreItem(currentBoughtItem);
    }

    public void SpawnInNPCHand(Item item)
    {
        backButton.interactable = false;
        var itemId = item.itemId;
        int currentlyOwnedCount = (GameState.isUnlockable(itemId) && GameState.isUnlocked(itemId)) ? GameState.unlockables[itemId] : 0;
        if (item.maximumTimesOwned == currentlyOwnedCount + 1 || String.IsNullOrEmpty(item.prefabFileName))
        {
            LockStore(new Item());
        }
        else
        {
            LockStore(item);
        }

        if (String.IsNullOrEmpty(item.prefabFileName) || itemId == "BackpackBig")
        {
            npc.GiveUnlockable(item);
            return;
        }

        npc.GiveItem(item.prefabFileName);
    }

    private void handleNPCGaveItem(object sender, BNG.Grabbable grabbable)
    {
        backButton.interactable = true;
        UnlockStore();
        npc.BackToIdle();
    }
}
