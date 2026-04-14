using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

public class QuestShellFinder : QuestBase
{
    private int minimumNumberOfSeaShell1 = 5;
    private int minimumNumberOfSeaShell2 = 20;
    private int minimumNumberOfSeaStar = 10;

    void OnEnable() 
    {
        questId = questDialogueController.questId;
        npc = questDialogueController.getTalkUIController().npc;

        npc.gaveItem += handleNPCGaveItem;
        npc.grabbedItem += handleNPCGrabbedItem;
        questDialogueController.checkStatus += handleCheckStatus;
        CheckStatus();
    }

    void OnDisable() 
    {
        npc.gaveItem -= handleNPCGaveItem;
        npc.grabbedItem -= handleNPCGrabbedItem;
        questDialogueController.checkStatus -= handleCheckStatus;
    }

    private void handleNPCGaveItem(object sender, Grabbable grabbable)
    {
        var item = Definitions.GetItemFromObject(grabbable);

        if (!item || item.itemId != rewardItem.itemId)
        {
            return;
        }

        GeneralQuestController.Instance.FinishQuest(questId);
        npc.BackToIdle();
    }

    private void handleNPCGrabbedItem(object sender, Grabbable grabbable)
    {
        if (CurrentDialogueIs(1))
        {
            if(!MeetsRequirement(grabbable, "SeaShell1", minimumNumberOfSeaShell1)) {
                return;
            }

            var itemStack = grabbable.GetComponent<ItemStack>();
            var stackSize = itemStack.GetStackSize();

            itemStack.SetStackSize(stackSize - minimumNumberOfSeaShell1);
            npc.handSlot.GetComponent<SnapZone>().ReleaseAll();
            if (stackSize - minimumNumberOfSeaShell1 == 0)
            {
                Destroy(grabbable.gameObject);
            }

            GeneralQuestController.Instance.UpdateQuest();
            questDialogueController.SetCurrentQuestDialog(2);
            return;
        }

        if (CurrentDialogueIs(2))
        {
            if(!MeetsRequirement(grabbable, "SeaShell2", minimumNumberOfSeaShell2)) {
                return;
            }

            var itemStack = grabbable.GetComponent<ItemStack>();
            var stackSize = itemStack.GetStackSize();

            itemStack.SetStackSize(stackSize - minimumNumberOfSeaShell2);
            npc.handSlot.GetComponent<SnapZone>().ReleaseAll();
            if (stackSize - minimumNumberOfSeaShell2 == 0)
            {
                Destroy(grabbable.gameObject);
            }

            GeneralQuestController.Instance.UpdateQuest();
            questDialogueController.SetCurrentQuestDialog(3);
            return;
        }

        if (CurrentDialogueIs(3))
        {
            if(!MeetsRequirement(grabbable, "SeaStar", minimumNumberOfSeaStar)) {
                return;
            }

            var itemStack = grabbable.GetComponent<ItemStack>();
            var stackSize = itemStack.GetStackSize();

            itemStack.SetStackSize(stackSize - minimumNumberOfSeaStar);
            npc.handSlot.GetComponent<SnapZone>().ReleaseAll();
            if (stackSize - minimumNumberOfSeaStar == 0)
            {
                Destroy(grabbable.gameObject);
            }

            GeneralQuestController.Instance.UpdateQuest();
            questDialogueController.SetCurrentQuestDialog(4);
            return;
        }
    }

    private void handleCheckStatus()
    {
        CheckStatus();
    }

    public void CheckStatus()
    {
        if (!QuestIsInProgress()) {
            return;
        }

        if (CurrentDialogueIs(1)) {
            npc.HoldOutHand();
        }

        if (CurrentDialogueIs(2)) {
            npc.HoldOutHand();
        }

        if (CurrentDialogueIs(3)) {
            npc.HoldOutHand();
        }

        if (CurrentDialogueIs(4)) {
            npc.SpawnQuestReward(rewardItem);
        }
    }
}
