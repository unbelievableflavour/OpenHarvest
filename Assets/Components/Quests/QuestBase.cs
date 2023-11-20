using BNG;
using UnityEngine;

public class QuestBase : MonoBehaviour
{
    public QuestOption questDialogueController;
    public HarvestDataTypes.Item rewardItem;

    protected NPCController npc;
    protected Definitions.Quests questId;

    protected bool CurrentDialogueIs(int dialogueNumber){
        return GameState.Instance.questList[questId].currentDialogue == dialogueNumber;
    }

    protected bool QuestIsInProgress(){
        return GameState.Instance.questList[questId].currentProgress == Progress.InProgress;
    }

    protected bool MeetsRequirement(Grabbable grabbable, string itemName, int minimumNumberOfItems)
    {
        var item = Definitions.GetItemFromObject(grabbable);
        if (item.itemId != itemName)
        {
            return false;
        }

        if (minimumNumberOfItems == 1) {
            return true;
        }

        if (grabbable.GetComponent<ItemStack>() == null)
        {
            return false;
        }

        int stackSize = grabbable.GetComponent<ItemStack>().GetStackSize();

        if (stackSize < minimumNumberOfItems)
        {
            return false;
        }

        return true;
    }
}
