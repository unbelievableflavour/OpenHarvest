using BNG;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarvestDataTypes;

public class HatClosetController : MonoBehaviour
{
    public Transform inventorySlots;

    List<HarvestDataTypes.Item> unlockableHats = new List<HarvestDataTypes.Item>();

    void Start()
    {
        var itemDatabase = DatabaseManager.Instance.items;
        unlockableHats = itemDatabase.FindAllByTag("hatCloset");

        LoadUnlockedHats();
    }

    private void LoadUnlockedHats()
    {
        var index = 0;
        foreach (Transform inventorySlot in inventorySlots)
        {
            var slot = inventorySlot.GetComponent<SnapZone>();
            if (!slot)
            {
                continue;
            }

            if (unlockableHats.ElementAtOrDefault(index) == null)
            {
                index++;
                continue;
            }

            var item = unlockableHats[index];
            if (item == null)
            {
                index++;
                continue;
            }

            if (!GameState.Instance.isUnlocked(item.itemId)){
                index++;
                continue;
            }

            var instantiatedItem = Definitions.InstantiateItemNew(item.prefab);
            var instantiatedItemGrabbable = instantiatedItem.GetComponent<Grabbable>();
            if (!instantiatedItemGrabbable)
            {
                index++;
                continue;
            }

            slot.GrabGrabbable(instantiatedItemGrabbable);
            index++;
        }
    }
}
