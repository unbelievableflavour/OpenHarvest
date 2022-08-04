using BNG;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Definitions;

public class ItemStashController : MonoBehaviour
{
    protected static List<SaveableItem> storedItems = new List<SaveableItem>();

    public Transform inventorySlots;
    public string itemStashName;

    string[] itemsThatShouldNotBeSaved = new string[] { "Wallet", "Backpack", "BackpackBig", "Basket" };

    void Start()
    {
        LoadInventory();
    }

    protected void LoadInventory()
    {
        storedItems = GetFromGameState();

        if (storedItems.Count == 0)
        {
            return;
        }

        var index = 0;

        foreach (Transform inventorySlot in inventorySlots)
        {
            var slot = inventorySlot.GetComponent<SnapZone>();
            if (!slot)
            {
                continue;
            }

            if (storedItems.ElementAtOrDefault(index) == null)
            {
                index++;
                continue;
            }

            var backpackItem = storedItems[index];
            if (backpackItem == null)
            {
                index++;
                continue;
            }

            var itemInfo = GetItemInformation(backpackItem.id);
            string itemId = itemInfo.itemId;

            if (itemsThatShouldNotBeSaved.Contains(itemId))
            {
                index++;
                continue;
            }

            var newItem = InstantiateItem(itemInfo.prefabFileName);
            var grabbableIsNotParent = newItem.GetComponent<GrabbableInDifferentLocation>();
            var newItemGrabbable = newItem.GetComponent<Grabbable>();

            if (grabbableIsNotParent)
            {
                newItemGrabbable = grabbableIsNotParent.grabbable;
            }

            if (newItemGrabbable)
            {
                var itemInformation = newItem.GetComponent<ItemInformation>();
                if (itemInformation && GameState.isUnlockable(itemInformation.getItemId()) && !GameState.isUnlocked(itemInformation.getItemId()))
                {
                    Destroy(newItem);
                    index++;
                    continue;
                }
            }

            var itemStack = newItemGrabbable.GetComponent<ItemStack>();
            if (itemStack)
            {
                itemStack.SetStackSize(backpackItem.currentStackSize);
            }

            var waterAmountStack = newItemGrabbable.GetComponent<WateringCanController>();
            if (waterAmountStack)
            {
                waterAmountStack.waterAmount = backpackItem.currentStackSize;
            }

            slot.GrabGrabbable(newItemGrabbable);

            index++;
        }
    }

    public void UpdateSaveableInventory()
    {
        storedItems = new List<SaveableItem>(inventorySlots.childCount);

        foreach (Transform inventorySlot in inventorySlots)
        {
            var slot = inventorySlot.GetComponent<SnapZone>();
            if (!slot || !slot.HeldItem)
            {
                storedItems.Add(null);
                continue;
            }

            Item itemInfo = slot.HeldItem.GetComponent<ItemInformation>().getItemInfo();
            if (itemInfo == null)
            {
                storedItems.Add(null);
                continue;
            }

            string itemId = itemInfo.itemId;
            if (itemsThatShouldNotBeSaved.Contains(itemId))
            {
                storedItems.Add(null);
                continue;
            }

            var saveableItem = new SaveableItem();
            saveableItem.id = itemId;

            var itemStack = slot.HeldItem.GetComponent<ItemStack>();
            if (itemStack)
            {
                saveableItem.currentStackSize = itemStack.GetStackSize();
            }

            var waterAmountStack = slot.HeldItem.GetComponent<WateringCanController>();
            if (waterAmountStack)
            {
                saveableItem.currentStackSize = waterAmountStack.waterAmount;
            }

            storedItems.Add(saveableItem);
        }

        SetInGameState();
    }

    private List<SaveableItem> GetFromGameState()
    {
        return GameState.itemStashes[itemStashName];
    }

    protected void SetInGameState()
    {
        GameState.itemStashes[itemStashName] = storedItems;
    }
}
