using BNG;
using UnityEngine;

public class QuestSantasLittleHelper : QuestOption
{
    public int minimumNeededHangedBaubleCount = 2;

    public QuestDialogueController questDialogueController;
    public Transform baubleHangingLocations;
    public HarvestDataTypes.Item rewardItem; 

    void Start()
    {
        npc.gaveItem += handleNPCGaveItem;
        Invoke("CheckStatus", 1f);
    }

    public void CheckStatus( )
    {
        if (GameState.Instance.questList[questId].currentProgress != Progress.InProgress)
        {
            return;
        }

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

        if(hangingBaubleCount >= minimumNeededHangedBaubleCount)
        {
            SpawnReward();
        }
    }

    private void handleNPCGaveItem(object sender, Grabbable e)
    {
        GeneralQuestController.Instance.FinishQuest(questId);
        npc.BackToIdle();
    }

    private void SpawnReward()
    {
        GeneralQuestController.Instance.UpdateQuest();
        questDialogueController.SetCurrentQuestDialog(2);

        npc.SpawnQuestReward(rewardItem);
    }
}
