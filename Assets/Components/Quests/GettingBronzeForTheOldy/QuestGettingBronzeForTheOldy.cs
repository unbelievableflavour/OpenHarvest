using BNG;
using UnityEngine;

public class QuestGettingBronzeForTheOldy : QuestOption
{
    private int minimumNumberOfBronzeOres = 20;
    public QuestDialogueController questDialogueController;

    private string RewardItemId = "HatMiner";

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
            if (grabbable.GetComponent<ItemInformation>().getItemId() != "OreBronze")
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
            npc.handSlot.GetComponent<SnapZone>().ReleaseAll();
            if (stackSize - minimumNumberOfBronzeOres == 0)
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
        if (GameState.questList[questId].currentProgress != Progress.InProgress)
        {
            return;
        }

        if(GameState.questList[questId].currentDialogue == 1)
        {
            npc.HoldOutHand();
        }

        if (GameState.questList[questId].currentDialogue == 2)
        {
            npc.HoldOutHand();
            npc.SpawnQuestReward(RewardItemId);
        }
    }

    private void SpawnReward()
    {
        GeneralQuestController.Instance.UpdateQuest();
        questDialogueController.SetCurrentQuestDialog(2);

        npc.SpawnQuestReward(RewardItemId);
    }
}
