using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

public class QuestShellFinder : MonoBehaviour
{
    private int minimumNumberOfSeaShell1 = 5;
    private int minimumNumberOfSeaShell2 = 20;
    private int minimumNumberOfSeaStar = 10;

    public QuestOption questDialogueController;
    public HarvestDataTypes.Item rewardItem;

    private NPCController npc;
    private Definitions.Quests questId;

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
        if (GameState.Instance.questList[questId].currentDialogue == 1)
        {
            var item = Definitions.GetItemFromObject(grabbable);

            if (item.itemId != "SeaShell1")
            {
                return;
            }

            if (grabbable.GetComponent<ItemStack>() == null)
            {
                return;
            }

            int stackSize = grabbable.GetComponent<ItemStack>().GetStackSize();

            if (stackSize < minimumNumberOfSeaShell1)
            {
                return;
            }

            grabbable.GetComponent<ItemStack>().SetStackSize(stackSize - minimumNumberOfSeaShell1);
            npc.handSlot.GetComponent<SnapZone>().ReleaseAll();
            if (stackSize - minimumNumberOfSeaShell1 == 0)
            {
                Destroy(grabbable.gameObject);
            }

            GeneralQuestController.Instance.UpdateQuest();
            questDialogueController.SetCurrentQuestDialog(2);
            return;
        }

        if (GameState.Instance.questList[questId].currentDialogue == 2)
        {
            var item = Definitions.GetItemFromObject(grabbable);

            if (item.itemId != "SeaShell2")
            {
                return;
            }

            if (grabbable.GetComponent<ItemStack>() == null)
            {
                return;
            }

            int stackSize = grabbable.GetComponent<ItemStack>().GetStackSize();

            if (stackSize < minimumNumberOfSeaShell2)
            {
                return;
            }

            grabbable.GetComponent<ItemStack>().SetStackSize(stackSize - minimumNumberOfSeaShell2);
            npc.handSlot.GetComponent<SnapZone>().ReleaseAll();
            if (stackSize - minimumNumberOfSeaShell2 == 0)
            {
                Destroy(grabbable.gameObject);
            }

            GeneralQuestController.Instance.UpdateQuest();
            questDialogueController.SetCurrentQuestDialog(3);
            return;
        }

        if (GameState.Instance.questList[questId].currentDialogue == 3)
        {
            var item = Definitions.GetItemFromObject(grabbable);

            if (item.itemId != "SeaStar")
            {
                return;
            }

            if (grabbable.GetComponent<ItemStack>() == null)
            {
                return;
            }

            int stackSize = grabbable.GetComponent<ItemStack>().GetStackSize();

            if (stackSize < minimumNumberOfSeaStar)
            {
                return;
            }

            grabbable.GetComponent<ItemStack>().SetStackSize(stackSize - minimumNumberOfSeaStar);
            npc.handSlot.GetComponent<SnapZone>().ReleaseAll();
            if (stackSize - minimumNumberOfSeaStar == 0)
            {
                Destroy(grabbable.gameObject);
            }

            Invoke("SpawnReward", 0.5f);
            return;
        }
    }

    private void handleCheckStatus()
    {
        CheckStatus();
    }

    public void CheckStatus()
    {
        if (GameState.Instance.questList[questId].currentProgress != Progress.InProgress)
        {
            return;
        }

        if (GameState.Instance.questList[questId].currentDialogue == 1)
        {
            npc.HoldOutHand();
        }

        if (GameState.Instance.questList[questId].currentDialogue == 2)
        {
            npc.HoldOutHand();
        }

        if (GameState.Instance.questList[questId].currentDialogue == 3)
        {
            npc.HoldOutHand();
        }

        if (GameState.Instance.questList[questId].currentDialogue == 4)
        {
            npc.SpawnQuestReward(rewardItem);
        }
    }

    private void SpawnReward()
    {
        GeneralQuestController.Instance.UpdateQuest();
        questDialogueController.SetCurrentQuestDialog(3);
    }
}
