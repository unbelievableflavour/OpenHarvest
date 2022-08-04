using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

public class QuestShellFinder : QuestOption
{
    private int minimumNumberOfSeaShell1 = 5;
    private int minimumNumberOfSeaShell2 = 20;
    private int minimumNumberOfSeaStar = 10;

    public QuestDialogueController questDialogueController;

    private string RewardItemId = "HatSea";

    void Start()
    {
        npc.gaveItem += handleNPCGaveItem;
        npc.grabbedItem += handleNPCGrabbedItem;
        npc.talks += handleNPCTalks;
    }

    private void handleNPCGaveItem(object sender, Grabbable grabbable)
    {
        var itemInfo = grabbable.GetComponent<ItemInformation>();

        if (!itemInfo || itemInfo.getItemId() != RewardItemId)
        {
            return;
        }

        GeneralQuestController.Instance.FinishQuest(questId);
        npc.BackToIdle();
    }

    private void handleNPCGrabbedItem(object sender, Grabbable grabbable)
    {
        if (GameState.questList[questId].currentDialogue == 1)
        {
            if (grabbable.GetComponent<ItemInformation>().getItemId() != "SeaShell1")
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

        if (GameState.questList[questId].currentDialogue == 2)
        {
            if (grabbable.GetComponent<ItemInformation>().getItemId() != "SeaShell2")
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

        if (GameState.questList[questId].currentDialogue == 3)
        {
            if (grabbable.GetComponent<ItemInformation>().getItemId() != "SeaStar")
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

    private void handleNPCTalks(object sender, GameObject talkObject)
    {
        var quest = talkObject.GetComponent<QuestDialogueController>();
        if (!quest)
        {
            return;
        }
        if(quest.questId != questId){
            return;
        }

        CheckStatus();
    }

    public void CheckStatus()
    {
        if (GameState.questList[questId].currentProgress != Progress.InProgress)
        {
            return;
        }

        if (GameState.questList[questId].currentDialogue == 1)
        {
            npc.HoldOutHand();
        }

        if (GameState.questList[questId].currentDialogue == 2)
        {
            npc.HoldOutHand();
        }

        if (GameState.questList[questId].currentDialogue == 3)
        {
            npc.HoldOutHand();
            npc.SpawnQuestReward(RewardItemId);
        }
    }

    private void SpawnReward()
    {
        GeneralQuestController.Instance.UpdateQuest();
        questDialogueController.SetCurrentQuestDialog(3);

        npc.SpawnQuestReward(RewardItemId);
    }
}
