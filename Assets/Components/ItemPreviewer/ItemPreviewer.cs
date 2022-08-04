using BNG;
using System;
using UnityEngine;

public class ItemPreviewer : MonoBehaviour
{
    public SnapZone snapZone;

    public void Spawn(Item item)
    {
        if (!snapZone)
        {
            return;
        }

        if (snapZone.HeldItem)
        {
            var currentItem = snapZone.HeldItem;
            currentItem.DropItem(null, true, true);
            Destroy(currentItem.gameObject);
        }

        if(item == null)
        {
            return;
        }

        InstantiateAndGrab(snapZone, item);
    }

    private void InstantiateAndGrab(SnapZone snapZone, Item item)
    {
        var newItem = Definitions.InstantiateItem(String.IsNullOrEmpty(item.prefabFileName) ? "BuyItemFallBack" : item.prefabFileName);
        var grabbableIsNotParent = newItem.GetComponent<GrabbableInDifferentLocation>();
        var newItemGrabbable = newItem.GetComponent<Grabbable>();

        if (grabbableIsNotParent)
        {
            newItemGrabbable = grabbableIsNotParent.grabbable;
        }

        snapZone.GrabGrabbable(newItemGrabbable);
    }
}
