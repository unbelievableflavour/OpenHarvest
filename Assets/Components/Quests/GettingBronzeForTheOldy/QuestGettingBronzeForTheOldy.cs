using BNG;
using UnityEngine;

public class QuestGettingBronzeForTheOldy : QuestBase
{
    private int minimumNumberOfBronzeOres = 20;

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
        if (CurrentDialogueIs(1)) {
            if(!MeetsRequirement(grabbable, "OreBronze", minimumNumberOfBronzeOres)) {
                return;
            }
          
            var itemStack = grabbable.GetComponent<ItemStack>();
            var stackSize = itemStack.GetStackSize();
           
            itemStack.SetStackSize(stackSize - minimumNumberOfBronzeOres);
            npc.ReleaseItemWithoutCallingEvent();
            if (stackSize - minimumNumberOfBronzeOres == 0)
            {
                Destroy(grabbable.gameObject);
            }

            GeneralQuestController.Instance.UpdateQuest();
            questDialogueController.SetCurrentQuestDialog(2);
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
            npc.SpawnQuestReward(rewardItem);
        }
    }
}
