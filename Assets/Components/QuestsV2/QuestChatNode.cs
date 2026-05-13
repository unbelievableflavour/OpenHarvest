using UnityEngine;

[CreateNodeMenu("OpenHarvest/Quests/Chat Node")]
public class QuestChatNode : QuestNodeBase
{
    [TextArea(3, 10)]
    [SerializeField] private string body = "";

    [TextArea(2, 6)]
    [Tooltip("Optional player-facing hint for quest tracker / journal UI. Not shown in NPC chat.")]
    public string tip = string.Empty;

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

            presenter.BeginSingleLine(
                body,
                npc,
                showContinue: true,
                onContinueOverride: () => QuestRuntimeService.Instance.ContinueQuestChatForNpc(interactable, interactionUI));
        });
    }
}
