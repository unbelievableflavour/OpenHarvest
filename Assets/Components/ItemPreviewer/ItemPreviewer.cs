using BNG;
using System;
using UnityEngine;

public class ItemPreviewer : MonoBehaviour
{
    public SnapZone snapZone;

    private HarvestDataTypes.Item fallbackItem;
    
    public void Start(){
        fallbackItem = DatabaseManager.Instance.items.fallbackItem;
    }

    public void Spawn(HarvestDataTypes.Item item)
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

    private void InstantiateAndGrab(SnapZone snapZone, HarvestDataTypes.Item item)
    {
        var newItem = Definitions.InstantiateItemNew(item.prefab == null ? fallbackItem.prefab : item.prefab);
        var grabbableIsNotParent = newItem.GetComponent<GrabbableInDifferentLocation>();
        var newItemGrabbable = newItem.GetComponent<Grabbable>();

        if (grabbableIsNotParent)
        {
            newItemGrabbable = grabbableIsNotParent.grabbable;
        }

        snapZone.GrabGrabbable(newItemGrabbable);
    }
}
