using BNG;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarvestDataTypes;

public class ItemStashController : MonoBehaviour
{
    protected static List<SaveableItem> storedItems = new List<SaveableItem>();

    public Transform inventorySlots;
    public string itemStashName;

    string[] itemsThatShouldNotBeSaved = new string[] { "Wallet", "Backpack", "BackpackBig", "Basket" };
    ItemDatabase itemDatabase;

    void Start()
    {
        LoadItemDatabase();
        LoadInventory();
    }

    private void LoadItemDatabase() {
        if (itemDatabase == null){
            itemDatabase = DatabaseManager.Instance.items;
        }
    }

    protected void LoadInventory()
    {
        LoadItemDatabase();
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

            var stashedItem = storedItems[index];
            if (stashedItem == null)
            {
                index++;
                continue;
            }

            Debug.Log(itemDatabase);
            var item = itemDatabase.FindById(stashedItem.id);

            if (itemsThatShouldNotBeSaved.Contains(item.itemId))
            {
                index++;
                continue;
            }

            var spawnedItem = Definitions.InstantiateItemNew(item.prefab);
            var grabbableIsNotParent = spawnedItem.GetComponent<GrabbableInDifferentLocation>();
            var newItemGrabbable = spawnedItem.GetComponent<Grabbable>();

            if (grabbableIsNotParent)
            {
                newItemGrabbable = grabbableIsNotParent.grabbable;
            }

            if (newItemGrabbable)
            {
                if (item.isUnlockable && !GameState.isUnlocked(item.itemId))
                {
                    Destroy(spawnedItem);
                    index++;
                    continue;
                }
            }

            var itemStack = newItemGrabbable.GetComponent<ItemStack>();
            if (itemStack)
            {
                itemStack.SetStackSize(stashedItem.currentStackSize);
            }

            var waterAmountStack = newItemGrabbable.GetComponent<WateringCanController>();
            if (waterAmountStack)
            {
                waterAmountStack.waterAmount = stashedItem.currentStackSize;
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

            var item = Definitions.GetItemFromObject(slot.HeldItem);
            if (item == null)
            {
                storedItems.Add(null);
                continue;
            }

            string itemId = item.itemId;
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
