using UnityEngine;

[CreateAssetMenu(
    fileName = "Npc Gift Option Action",
    menuName = "OpenHarvest/NPC/Interaction Option Actions/Gift",
    order = 6)]
public class NpcGiftInteractionOptionAction : NpcInteractionOptionAction
{
    [SerializeField, Tooltip("UI root with NpcChatTreePresenter shown when gift is requested.")]
    private GameObject chatUIPrefab;
    [TextArea(2, 6)]
    [SerializeField, Tooltip("Message shown in chat-style UI when this gift action is selected.")]
    private string giftPrompt = "Would you like to give me something?";

    public override bool IsValid(NpcInteractionOption option)
    {
        return option != null;
    }

    public override string ResolveDisplayName(NpcInteractionOption option, NpcProximityInteractable interactable)
    {
        if (option == null || string.IsNullOrWhiteSpace(option.displayName))
        {
            return "Give gift";
        }

        return option.displayName.Trim();
    }

    public override void Execute(
        InteractionUIController interactionUI,
        NpcProximityInteractable interactable,
        NpcInteractionOption option)
    {
        if (interactable == null)
        {
            return;
        }

        ShowGiftPrompt(interactionUI, interactable);

        if (QuestRuntimeService.Instance == null)
        {
            NPCController npc = interactable.GetComponentInParent<NPCController>();
            if (npc == null)
            {
                npc = interactable.GetComponent<NPCController>();
            }

            if (npc != null)
            {
                npc.HoldOutHand();
            }

            return;
        }

        QuestRuntimeService.Instance.RequestGenericGift(interactable);
    }

    private void ShowGiftPrompt(InteractionUIController interactionUI, NpcProximityInteractable interactable)
    {
        if (interactionUI == null || chatUIPrefab == null)
        {
            return;
        }

        NPCController npc = interactable.GetComponentInParent<NPCController>();
        if (npc == null)
        {
            npc = interactable.GetComponent<NPCController>();
        }

        interactionUI.ShowInstancedOptionContent(chatUIPrefab, interactable, go =>
        {
            NpcChatTreePresenter presenter = go.GetComponentInChildren<NpcChatTreePresenter>(true);
            if (presenter == null)
            {
                return;
            }

            presenter.BeginSingleLine(giftPrompt ?? string.Empty, npc, showContinue: false);
        });
    }
}
