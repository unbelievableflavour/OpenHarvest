using BNG;
using System;
using UnityEngine;
using static Definitions;

public class NPCController : MonoBehaviour
{
    public GameObject handSlot;
    public Animator NPCAnimator;
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
        if (NPCAnimator.GetCurrentAnimatorStateInfo(0).IsName("GivingIdle")
         || NPCAnimator.GetCurrentAnimatorStateInfo(0).IsName("IdleToGive")) {
            NPCAnimator.Play("GiveToIdle");
        } 

        handSlot.SetActive(false);
    }

    public void HoldOutHand()
    {
        NPCAnimator.Play("IdleToGive");
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
