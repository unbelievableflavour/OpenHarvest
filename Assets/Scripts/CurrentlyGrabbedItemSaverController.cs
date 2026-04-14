using BNG;
using System;
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
        SceneSwitcher.Instance.beforeSceneSwitch += beforeSceneSwitch;
        Invoke("SpawnGrabbedItemsFromLastScene", 1);
    }

    private void SpawnGrabbedItemsFromLastScene()
    {
        if (storedItems.Count == 0)
        {
            return;
        }

        foreach (SaveableItem saveableItem in storedItems)
        {
            var itemInfo = DatabaseManager.Instance.items.FindById(saveableItem.id);
            if(!itemInfo){
                continue;
            }
            var newItem = InstantiateItemNew(itemInfo.prefab);

            var grabbableIsNotParent = newItem.GetComponent<GrabbableInDifferentLocation>();
            var newItemGrabbable = newItem.GetComponent<Grabbable>();

            if (grabbableIsNotParent)
            {
                newItemGrabbable = grabbableIsNotParent.grabbable;
            }

            if (newItemGrabbable)
            {
                var item = Definitions.GetItemFromObject(newItem);
                if (item && item.isUnlockable && !GameState.Instance.isUnlocked(item.itemId))
                {
                    Destroy(newItem);
                    continue;
                }
            }

            var itemStack = newItemGrabbable.GetComponent<ItemStack>();
            if (itemStack)
            {
                itemStack.SetStackSize(saveableItem.currentStackSize);
            }

            var waterAmountStack = newItemGrabbable.GetComponent<WateringCanController>();
            if (waterAmountStack)
            {
                waterAmountStack.waterAmount = saveableItem.currentStackSize;
            }

            newItem.transform.position = GameState.Instance.currentPlayerPosition.position;
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

        var item = Definitions.GetItemFromObject(grabber.HeldGrabbable);
        if (item == null)
        {
            return;
        }

        string itemId = item.itemId;
        if (itemId == "Wallet" || itemId == "Backpack" || itemId == "BackpackBig")
        {
            return;
        }

        var saveableItem = new SaveableItem();
        saveableItem.name = item.name;
        saveableItem.prefabFileName = item.prefab.transform.name;

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

    protected void beforeSceneSwitch(object sender, EventArgs e)
    {
        SceneSwitcher.Instance.beforeSceneSwitch -= beforeSceneSwitch;
        SaveGrabbedItems();
    }
}
