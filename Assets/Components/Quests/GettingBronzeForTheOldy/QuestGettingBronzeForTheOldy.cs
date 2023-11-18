using BNG;
using UnityEngine;

public class QuestGettingBronzeForTheOldy : MonoBehaviour
{
    public QuestOption questDialogueController;
    public HarvestDataTypes.Item rewardItem;
    private int minimumNumberOfBronzeOres = 20;

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
            if (item.itemId != "OreBronze")
            {
                return;
            }

            if (grabbable.GetComponent<ItemStack>() == null)
            {
                return;
            }

            int stackSize = grabbable.GetComponent<ItemStack>().GetStackSize();
            if (stackSize < minimumNumberOfBronzeOres)
            {
                return;
            }

            grabbable.GetComponent<ItemStack>().SetStackSize(stackSize - minimumNumberOfBronzeOres);
            npc.ReleaseItemWithoutCallingEvent();
            if (stackSize - minimumNumberOfBronzeOres == 0)
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

        if(GameState.Instance.questList[questId].currentDialogue == 1)
        {
            npc.HoldOutHand();
        }

        if (GameState.Instance.questList[questId].currentDialogue == 2)
        {
            npc.SpawnQuestReward(rewardItem);
        }
    }

    private void SpawnReward()
    {
        GeneralQuestController.Instance.UpdateQuest();
        questDialogueController.SetCurrentQuestDialog(2);
    }
}
