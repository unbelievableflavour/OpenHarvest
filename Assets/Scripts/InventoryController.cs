using BNG;
using System;
using UnityEngine;

[Serializable]
public class SaveableItem
{
    public string id;
    public string name; // DEPRECATED, DONT USE THIS
    public string prefabFileName; // DEPRECATED, DONT USE THIS
    public int currentStackSize;
}

public class InventoryController : ItemStashController
{
    public GameObject backpack;
    public GameObject backpackBig;
    public Transform backpackInventorySlots;
    public Transform backpackBiginventorySlots;
    public Transform spawnIfDoesntFitSlot;

    private string backpackBigId = "BackpackBig";


    void Start()
    {
        SceneSwitcher.Instance.beforeSceneSwitch += beforeSceneSwitch;
        inventorySlots = backpackInventorySlots;

        //Enable big backpack if unlocked
        if (GameState.Instance.isUnlocked(backpackBigId))
        {
            inventorySlots = backpackBiginventorySlots;
            backpack.SetActive(false);
            backpackBig.SetActive(true);
            GetComponent<SnapZone>().HeldItem = backpackBig.GetComponent<Grabbable>();
        }

        LoadInventory();
    }

    public void AddToFirstOpenSlot(Grabbable grabbable)
    {
        if (grabbable.gameObject.name == "Backpack" || grabbable.gameObject.name == "BackpackBig")
        {
            return;
        }
        var item = Definitions.GetItemFromObject(grabbable.gameObject);
        if (item == null)
        {
            return;
        }

        var grabbableStack = grabbable.GetComponent<ItemStack>();
        var waterAmountStack = grabbable.GetComponent<WateringCanController>();

        //Check if already in backpack if it is stackable
        if (grabbableStack)
        {
            foreach (Transform inventorySlot in inventorySlots)
            {
                var slot = inventorySlot.GetComponent<SnapZone>();
                if (!slot || !slot.HeldItem)
                {
                    continue;
                }

                var itemInSlot = Definitions.GetItemFromObject(slot.HeldItem);
                if (!itemInSlot)
                {
                    continue;
                }

                if (itemInSlot.itemId != item.itemId)
                {
                    continue;
                }

                slot.HeldItem.GetComponent<ItemStack>().IncreaseStack(grabbableStack.GetStackSize());

                grabbable.DropItem(false, false);
                Destroy(grabbable.gameObject);

                var stackableSnapZone = slot.GetComponent<StackableSnapZone>();
                if (stackableSnapZone)
                {
                    stackableSnapZone.updateStackText(slot.HeldItem.GetComponent<ItemStack>().GetStackSize());
                    slot.HeldItem.GetComponent<ItemStack>().stackindicator.gameObject.SetActive(false);
                }
                return;
            }
        }

        //If not, just add it to the first open slot
        foreach (Transform inventorySlot in inventorySlots)
        {
            var slot = inventorySlot.GetComponent<SnapZone>();
            if (!slot || slot.HeldItem)
            {
                continue;
            }

            int stackSize = grabbableStack ? grabbableStack.GetStackSize() : 1;
            int waterAmount = waterAmountStack ? waterAmountStack.waterAmount : 1;

            grabbable.DropItem(null, true, true);
            Destroy(grabbable.gameObject);

            InstantiateAndGrab(slot, item, stackSize, waterAmount);
            return;
        }

        grabbable.DropItem(null, true, true);
        Destroy(grabbable.gameObject);

        Definitions.InstantiateItemNew(item.prefab, spawnIfDoesntFitSlot);
    }

    private void InstantiateAndGrab(SnapZone snapZone, HarvestDataTypes.Item item, int stackSize, int waterAmount)
    {
        var newItem = Definitions.InstantiateItemNew(item.prefab);
        var grabbableIsNotParent = newItem.GetComponent<GrabbableInDifferentLocation>();
        var newItemGrabbable = newItem.GetComponent<Grabbable>();

        if (grabbableIsNotParent)
        {
            newItemGrabbable = grabbableIsNotParent.grabbable;
        }

        var itemStack = newItemGrabbable.GetComponent<ItemStack>();
        if (itemStack)
        {
            itemStack.SetStackSize(stackSize);
        }

        var waterAmountStack = newItemGrabbable.GetComponent<WateringCanController>();
        if (waterAmountStack)
        {
            waterAmountStack.waterAmount = waterAmount;
        }

        snapZone.GrabGrabbable(newItemGrabbable);
    }
}
