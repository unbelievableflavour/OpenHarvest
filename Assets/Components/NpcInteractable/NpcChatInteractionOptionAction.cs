using UnityEngine;

[CreateAssetMenu(
    fileName = "Npc Chat Option Action",
    menuName = "OpenHarvest/NPC/Interaction Option Actions/Chat",
    order = 4)]
public class NpcChatInteractionOptionAction : NpcInteractionOptionAction
{
    [Tooltip("UI root with NpcChatTreePresenter (spawned under current interaction).")]
    [SerializeField] private GameObject chatUIPrefab;

    [SerializeField] private NpcChatGraph chatGraph;

    public override bool IsValid(NpcInteractionOption option)
    {
        if (option == null || string.IsNullOrWhiteSpace(option.displayName) || chatUIPrefab == null || chatGraph == null)
        {
            return false;
        }

        return chatGraph.TryValidate(out _);
    }

    public override void Execute(
        InteractionUIController interactionUI,
        NpcProximityInteractable interactable,
        NpcInteractionOption option)
    {
        if (interactionUI == null || chatUIPrefab == null || chatGraph == null)
        {
            return;
        }

        NpcChatNode entryNode = chatGraph.GetEntryNode();
        if (entryNode == null)
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

            presenter.Begin(entryNode, npc);
        });
    }
}
