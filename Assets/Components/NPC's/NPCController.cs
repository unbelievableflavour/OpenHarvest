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

        if(snapZone.HeldItem != null)
        {
            snapZone.HeldItem.GetComponent<ItemStack>().IncreaseStack(1);
            return;
        }

        HoldOutHand();
        GameObject spawnedRewardItem = InstantiateItemNew(item.prefab);
        snapZone.GrabGrabbable(spawnedRewardItem.GetComponent<Grabbable>());
    }

    public void GiveUnlockable(HarvestDataTypes.Item unlockableItem)
    {
        var fallbackItem = DatabaseManager.Instance.items.fallbackItem;

        HoldOutHand();
        GameObject rewardItem = InstantiateItemNew(fallbackItem.prefab);
        rewardItem.GetComponent<ShowUnlockMessageOnGrab>().setItem(unlockableItem);
        handSlot.GetComponent<SnapZone>().GrabGrabbable(rewardItem.GetComponent<Grabbable>());
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
