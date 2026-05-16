using UnityEngine;

[CreateNodeMenu("OpenHarvest/Quests/Gift Node")]
public class QuestGiftNode : QuestNodeBase
{
    [TextArea(2, 6)]
    [Tooltip("Message shown in chat-style UI when this gift step is requested.")]
    public string giftPrompt = "Could you give me this item?";

    [TextArea(2, 6)]
    [Tooltip("Optional player-facing hint for quest tracker / journal UI. Not shown in NPC chat.")]
    public string tip = string.Empty;

    [Tooltip("Required item for this gift step. Leave empty to accept any item.")]
    public HarvestDataTypes.Item requiredItem;
    [Min(1)]
    [Tooltip("Required stack amount for this gift step.")]
    public int requiredAmount = 1;

    public override void RunAction(InteractionUIController interactionUI, NpcProximityInteractable interactable, QuestGraph graph)
    {
        if (interactionUI == null || graph == null || graph.chatUIPrefab == null)
        {
            return;
        }

        NPCController npc = interactable != null
            ? interactable.GetComponentInParent<NPCController>()
            : null;

        interactionUI.ShowInstancedOptionContent(graph.chatUIPrefab, interactable, go =>
        {
            NpcChatTreePresenter presenter = go.GetComponentInChildren<NpcChatTreePresenter>(true);
            if (presenter == null)
            {
                return;
            }

            presenter.BeginSingleLine(giftPrompt ?? string.Empty, npc, showContinue: false);
        });
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
