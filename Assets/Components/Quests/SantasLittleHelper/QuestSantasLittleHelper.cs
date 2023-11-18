using BNG;
using UnityEngine;

public class QuestSantasLittleHelper : MonoBehaviour
{
    private int minimumNeededHangedBaubleCount = 2;

    public QuestOption questDialogueController;
    public Transform baubleHangingLocations;
    public HarvestDataTypes.Item rewardItem; 

    private NPCController npc;
    private Definitions.Quests questId;

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
        if (GameState.Instance.questList[questId].currentProgress != Progress.InProgress)
        {
            return;
        }

        if (GameState.Instance.questList[questId].currentDialogue == 1)
        {
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

            Invoke("SpawnReward", 0.5f);
        }

        if (GameState.Instance.questList[questId].currentDialogue == 2)
        {
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

    private void SpawnReward()
    {
        GeneralQuestController.Instance.UpdateQuest();
        questDialogueController.SetCurrentQuestDialog(2);
    }
}
