using BNG;
using UnityEngine;

public class QuestSantasLittleHelper : QuestBase
{
    public Transform baubleHangingLocations;

    private int minimumNeededHangedBaubleCount = 2;

    void OnEnable()
    {
        questId = questDialogueController.questId;
        npc = questDialogueController.getTalkUIController().npc;

        npc.gaveItem += handleNPCGaveItem;
        questDialogueController.checkStatus += handleCheckStatus;
        CheckStatus();
    }

    void OnDisable() 
    {
        npc.gaveItem -= handleNPCGaveItem;
        questDialogueController.checkStatus -= handleCheckStatus;
    }


    public void CheckStatus( )
    {
        if (!QuestIsInProgress()) {
            return;
        }

        if (CurrentDialogueIs(1)) {
            int hangingBaubleCount = 0;
            foreach (Transform inventorySlot in baubleHangingLocations)
            {
                var slot = inventorySlot.GetComponent<SnapZone>();
                if (!slot)
                {
                    continue;
                }

                if (!slot.HeldItem)
                {
                    continue;
                }
                hangingBaubleCount++;
            }

            if(hangingBaubleCount < minimumNeededHangedBaubleCount)
            {
                return;
            }

            GeneralQuestController.Instance.UpdateQuest();
            questDialogueController.SetCurrentQuestDialog(2);
            return;
        }

        if (CurrentDialogueIs(2)) {
            npc.SpawnQuestReward(rewardItem);
        }
    }

    private void handleCheckStatus()
    {
        CheckStatus();
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
}
