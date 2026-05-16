using BNG;
using System;
using UnityEngine;
using static Definitions;

public class NPCController : MonoBehaviour
{
    public GameObject handSlot;
    public Animator NPCAnimator; // if no animator, it will just toggle the handSlot game object.
    public event EventHandler<Grabbable> gaveItem;
    public event EventHandler<Grabbable> grabbedItem;
    
    private bool dontInvokeEvent = false;

    public void SpawnQuestReward(HarvestDataTypes.Item rewardItem)
    {
        HoldOutHand();
        dontInvokeEvent = true;
        GameObject spawnedRewardItem = InstantiateItemNew(rewardItem.prefab);
        handSlot.GetComponent<SnapZone>().GrabGrabbable(spawnedRewardItem.GetComponent<Grabbable>());
    }

    public void BackToIdle()
    {
        if (handSlot == null)
        {
            return;
        }

        // New animation system.
        if (NPCAnimator != null && (
            NPCAnimator.GetCurrentAnimatorStateInfo(0).IsName("GivingIdle") // old animation system
         || NPCAnimator.GetCurrentAnimatorStateInfo(1).IsName("GivingIdle") // new animation system
         || NPCAnimator.GetCurrentAnimatorStateInfo(0).IsName("IdleToGive") // old animation system
         || NPCAnimator.GetCurrentAnimatorStateInfo(1).IsName("IdleToGive") // new animation system
        )) {
            NPCAnimator.Play("GiveToIdle"); // new animation system
        }

        handSlot.SetActive(false);
    }

    public void HoldOutHand()
    {
        if (handSlot == null)
        {
            return;
        }

        if (NPCAnimator != null)
        {
            NPCAnimator.Play("IdleToGive");
        }

        handSlot.SetActive(true);
    }

    public void GiveItem(HarvestDataTypes.Item item)
    {
        var snapZone = handSlot.GetComponent<SnapZone>();
        bool isUnlockable = item.prefab == null;

        if(snapZone.HeldItem != null)
        {
            snapZone.HeldItem.GetComponent<ItemStack>().IncreaseStack(1);
            return;
        }

        HoldOutHand();
        GameObject spawnedRewardItem = Definitions.InstantiateItemNew(isUnlockable 
            ? DatabaseManager.Instance.items.fallbackItem.prefab 
            : item.prefab
        );

        snapZone.GrabGrabbable(spawnedRewardItem.GetComponent<Grabbable>());

        if(isUnlockable) {
            spawnedRewardItem.GetComponent<ShowUnlockMessageOnGrab>().setItem(item);
        }
    }

    public void GiveStoreProduct(HarvestDataTypes.StoreProduct product)
    {
        var snapZone = handSlot.GetComponent<SnapZone>();
        bool hasPrefab = product != null && product.prefab != null;

        if (snapZone.HeldItem != null)
        {
            snapZone.HeldItem.GetComponent<ItemStack>().IncreaseStack(1);
            return;
        }

        HoldOutHand();
        GameObject spawnedRewardItem = Definitions.InstantiateItemNew(
            hasPrefab
                ? product.prefab
                : DatabaseManager.Instance.items.fallbackItem.prefab
        );

        snapZone.GrabGrabbable(spawnedRewardItem.GetComponent<Grabbable>());
    }

    public void ReleaseItemWithoutCallingEvent()
    {
        dontInvokeEvent = true;
        handSlot.GetComponent<SnapZone>().ReleaseAll();
    }

    public void RemoveCurrentlyHoldingItem()
    {
        if (!handSlot.GetComponent<SnapZone>().HeldItem) {
            return;
        }

        var currentlyHeldItem = handSlot.GetComponent<SnapZone>().HeldItem.gameObject;
        ReleaseItemWithoutCallingEvent();
        Destroy(currentlyHeldItem);
    }

    public void NPCGaveItem(Grabbable grabbable)
    {
        if (dontInvokeEvent) {
            // we just spawned a reward item.
            dontInvokeEvent = false;
            return;
        }
        gaveItem?.Invoke(this, grabbable);
    }

    public void NPCGrabbedItem(Grabbable grabbable)
    {   
        if (dontInvokeEvent) {
            // we just spawned a reward item.
            dontInvokeEvent = false;
            Debug.Log("we just spawned a reward item.");
            return;
        }
        grabbedItem?.Invoke(this, grabbable);
    }
}
