using UnityEngine;

[CreateAssetMenu(
    fileName = "Npc Gift Option Action",
    menuName = "OpenHarvest/NPC/Interaction Option Actions/Gift",
    order = 6)]
public class NpcGiftInteractionOptionAction : NpcInteractionOptionAction
{
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
        if (interactable == null || QuestRuntimeService.Instance == null)
        {
            return;
        }

        QuestRuntimeService.Instance.RequestGenericGift(interactable);
    }
}
