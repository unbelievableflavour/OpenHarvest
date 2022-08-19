using BNG;
using UnityEngine;

public class QuestCleanupCrew : QuestOption
{
    private int minimumNumberOfBottles = 10;
    private int minimumNumberOfCans = 15;

    public QuestDialogueController questDialogueController;
    public HarvestDataTypes.Item rewardItem; 

    void Start()
    {
        npc.gaveItem += handleNPCGaveItem;
        npc.grabbedItem += handleNPCGrabbedItem;
        npc.talks += handleNPCTalks;
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
            if (item.itemId != "Bottle")
            {
                return;
            }

            if (grabbable.GetComponent<ItemStack>() == null)
            {
                return;
            }

            int stackSize = grabbable.GetComponent<ItemStack>().GetStackSize();

            if (stackSize < minimumNumberOfBottles)
            {
                return;
            }

            grabbable.GetComponent<ItemStack>().SetStackSize(stackSize - minimumNumberOfBottles);
            npc.handSlot.GetComponent<SnapZone>().ReleaseAll();
            if (stackSize - minimumNumberOfBottles == 0)
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
            if (item.itemId != "Can")
            {
                return;
            }

            if (grabbable.GetComponent<ItemStack>() == null)
            {
                return;
            }

            int stackSize = grabbable.GetComponent<ItemStack>().GetStackSize();

            if (stackSize < minimumNumberOfCans)
            {
                return;
            }

            grabbable.GetComponent<ItemStack>().SetStackSize(stackSize - minimumNumberOfCans);
            npc.handSlot.GetComponent<SnapZone>().ReleaseAll();
            if (stackSize - minimumNumberOfCans == 0)
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
        if (quest.questId != questId)
        {
            return;
        }

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
            npc.SpawnQuestReward(rewardItem);
        }
    }

    private void SpawnReward()
    {
        GeneralQuestController.Instance.UpdateQuest();
        questDialogueController.SetCurrentQuestDialog(3);

        npc.SpawnQuestReward(rewardItem);
    }
}
