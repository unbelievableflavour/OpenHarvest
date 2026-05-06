using UnityEngine;

[CreateAssetMenu(
    fileName = "Npc Chat Option Action",
    menuName = "OpenHarvest/NPC/Interaction Option Actions/Chat",
    order = 4)]
public class NpcChatInteractionOptionAction : NpcInteractionOptionAction
{
    [Tooltip("UI root with NpcChatTreePresenter (spawned under current interaction).")]
    [SerializeField] private GameObject chatUIPrefab;

    [SerializeField] private NpcChatNode chatNode;

    public override bool IsValid(NpcInteractionOption option)
    {
        if (option == null || string.IsNullOrWhiteSpace(option.displayName) || chatUIPrefab == null || chatNode == null)
        {
            return false;
        }

        NpcChatGraph graph = chatNode.graph as NpcChatGraph;
        if (graph == null)
        {
            return true;
        }

        return graph.TryValidate(out _);
    }

    public override void Execute(
        InteractionUIController interactionUI,
        NpcProximityInteractable interactable,
        NpcInteractionOption option)
    {
        if (interactionUI == null || chatUIPrefab == null || chatNode == null)
        {
            return;
        }

        NPCController npc = interactable != null
            ? interactable.GetComponentInParent<NPCController>()
            : null;

        interactionUI.ShowInstancedOptionContent(chatUIPrefab, interactable, go =>
        {
            NpcChatTreePresenter presenter = go.GetComponentInChildren<NpcChatTreePresenter>(true);
            if (presenter == null)
            {
                return;
            }

            presenter.Begin(chatNode, npc);
        });
    }
}
