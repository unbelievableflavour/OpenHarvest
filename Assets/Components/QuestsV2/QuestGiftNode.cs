using UnityEngine;

[CreateNodeMenu("OpenHarvest/Quests/Gift Node")]
public class QuestGiftNode : QuestNodeBase
{
    [Tooltip("Required item for this gift step. Leave empty to accept any item.")]
    public HarvestDataTypes.Item requiredItem;
    [Min(1)]
    [Tooltip("Required stack amount for this gift step.")]
    public int requiredAmount = 1;

    private void OnValidate()
    {
        completesQuest = false;
    }

    public override void RunAction(InteractionUIController interactionUI, NpcProximityInteractable interactable, QuestGraph graph)
    {
        // Gift handling is managed by QuestRuntimeService pending-gift flow.
    }

    public override bool IsGiftMatch(string itemId)
    {
        if (requiredItem == null || string.IsNullOrWhiteSpace(requiredItem.itemId))
        {
            return true;
        }

        return string.Equals(requiredItem.itemId.Trim(), itemId != null ? itemId.Trim() : string.Empty, System.StringComparison.Ordinal);
    }

    public int GetRequiredAmount()
    {
        return requiredAmount < 1 ? 1 : requiredAmount;
    }
}
