using BNG;
using System.Collections.Generic;
using UnityEngine;
using static Definitions;

public class CurrentlyGrabbedItemSaverController : MonoBehaviour
{
    public Grabber leftGrabber;
    public Grabber rightGrabber;

    protected static List<SaveableItem> storedItems = new List<SaveableItem>();

    void Start()
    {
        Invoke("SpawnGrabbedItemsFromLastScene", 1);
    }

    private void SpawnGrabbedItemsFromLastScene()
    {
        if (storedItems.Count == 0)
        {
            return;
        }

        foreach (SaveableItem item in storedItems)
        {
            var newItem = InstantiateItem(item.prefabFileName);

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
                    continue;
                }
            }

            var itemStack = newItemGrabbable.GetComponent<ItemStack>();
            if (itemStack)
            {
                itemStack.SetStackSize(item.currentStackSize);
            }

            var waterAmountStack = newItemGrabbable.GetComponent<WateringCanController>();
            if (waterAmountStack)
            {
                waterAmountStack.waterAmount = item.currentStackSize;
            }

            newItem.transform.position = GameState.currentPlayerPosition.position;
        }
    }

    public void SaveGrabbedItems()
    {
        storedItems = new List<SaveableItem>();

        SaveGrabbedItem(leftGrabber);
        SaveGrabbedItem(rightGrabber);
    }

    private void SaveGrabbedItem(Grabber grabber)
    {
        if (!grabber || !grabber.HeldGrabbable || !grabber.HeldGrabbable.CanBeSnappedToSnapZone)
        {
            return;
        }

        Item itemInfo = grabber.HeldGrabbable.GetComponent<ItemInformation>().getItemInfo();
        if (itemInfo == null)
        {
            return;
        }

        string itemId = grabber.HeldGrabbable.GetComponent<ItemInformation>().getItemId();
        if (itemId == "Wallet" || itemId == "Backpack" || itemId == "BackpackBig")
        {
            return;
        }

        var saveableItem = new SaveableItem();
        saveableItem.name = itemInfo.name;
        saveableItem.prefabFileName = itemInfo.prefabFileName;

        var itemStack = grabber.HeldGrabbable.GetComponent<ItemStack>();
        if (itemStack)
        {
            saveableItem.currentStackSize = itemStack.GetStackSize();
        }

        var waterAmountStack = grabber.HeldGrabbable.GetComponent<WateringCanController>();
        if (waterAmountStack)
        {
            saveableItem.currentStackSize = waterAmountStack.waterAmount;
        }

        storedItems.Add(saveableItem);
    }
}
