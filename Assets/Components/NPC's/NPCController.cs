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

    public void SpawnQuestReward(string rewardPrefabName)
    {
        HoldOutHand();
        GameObject rewardItem = InstantiateItem(rewardPrefabName);
        handSlot.GetComponent<SnapZone>().GrabGrabbable(rewardItem.GetComponent<Grabbable>());
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

    public void GiveItem(string rewardPrefabName)
    {
        var snapZone = handSlot.GetComponent<SnapZone>();

        if(snapZone.HeldItem != null)
        {
            snapZone.HeldItem.GetComponent<ItemStack>().IncreaseStack(1);
            return;
        }

        HoldOutHand();
        GameObject rewardItem = InstantiateItem(rewardPrefabName);
        snapZone.GrabGrabbable(rewardItem.GetComponent<Grabbable>());
    }

    public void GiveUnlockable(Item unlockableItem)
    {
        HoldOutHand();
        Item itemInfo = GetItemInformation(fallbackItemId);
        GameObject rewardItem = InstantiateItem(itemInfo.prefabFileName);
        rewardItem.GetComponent<ShowUnlockMessageOnGrab>().setItemId(unlockableItem.itemId);
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
