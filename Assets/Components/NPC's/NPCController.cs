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
    public event EventHandler<GameObject> talks;

    public void SpawnQuestReward(HarvestDataTypes.Item rewardItem)
    {
        HoldOutHand();
        GameObject spawnedRewardItem = InstantiateItemNew(rewardItem.prefab);
        handSlot.GetComponent<SnapZone>().GrabGrabbable(spawnedRewardItem.GetComponent<Grabbable>());
    }

    public void BackToIdle()
    {
        NPCAnimator.Play("GiveToIdle");
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

    public void NPCGaveItem(Grabbable grabbable)
    {
        gaveItem?.Invoke(this, grabbable);
    }

    public void NPCGrabbedItem(Grabbable grabbable)
    {
        grabbedItem?.Invoke(this, grabbable);
    }

    public void NPCTalks(GameObject talkObject)
    {
        talks?.Invoke(this, talkObject);
    }
}
