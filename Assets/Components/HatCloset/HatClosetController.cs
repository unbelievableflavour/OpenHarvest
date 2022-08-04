using BNG;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Definitions;

public class HatClosetController : MonoBehaviour
{
    public Transform inventorySlots;

    List<string> hatIds = new List<string>()
    {
        "HatStraw",
        "HatFarmer",
        "HatBaret",
        "HatCrown",
        "HatChicken",
        "HatCloud",
        "HatFish",
        "HatFishing",
        "HatFlatCap",
        "HatFlying",
        "HatSanta",
        //"HatHelmet",
        "HatSombrero",
        //"HatTop",
        //"HatViking",
        "HatWinter",
        "HatPlain",
        "HatMiner",
        //"HatHeadphone",
        "HatSea",
        "HatRecycle"
    };

    List<Item> unlockableHats = new List<Item>();

    void Start()
    {
        foreach(string hatId in hatIds)
        {
            unlockableHats.Add(GetItemInformation(hatId));
        }

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

            var hat = unlockableHats[index];
            if (hat == null)
            {
                index++;
                continue;
            }

            if (!GameState.isUnlocked(hat.itemId)){
                index++;
                continue;
            }

            var instantiatedItem = InstantiateItem(hat.prefabFileName);
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

    public List<string> GetHatIds()
    {
        return hatIds;
    }
}
