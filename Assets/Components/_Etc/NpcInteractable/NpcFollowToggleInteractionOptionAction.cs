using UnityEngine;

[CreateAssetMenu(
    fileName = "Npc Follow Toggle Option Action",
    menuName = "OpenHarvest/NPC/Interaction Option Actions/Follow Toggle",
    order = 5)]
public class NpcFollowToggleInteractionOptionAction : NpcInteractionOptionAction
{
    [SerializeField] private string followLabel = "Follow me";
    [SerializeField] private string stopFollowingLabel = "Stop following";

    public override bool IsValid(NpcInteractionOption option)
    {
        return option != null;
    }

    public override string ResolveDisplayName(NpcInteractionOption option, NpcProximityInteractable interactable)
    {
        NPCNavAgent nav = interactable != null ? interactable.GetComponentInParent<NPCNavAgent>() : null;
        if (nav == null)
        {
            return string.IsNullOrWhiteSpace(option?.displayName) ? "Follow" : option.displayName.Trim();
        }

        if (nav.followTarget != null)
        {
            return string.IsNullOrWhiteSpace(stopFollowingLabel) ? "Stop following" : stopFollowingLabel.Trim();
        }

        return string.IsNullOrWhiteSpace(followLabel) ? "Follow me" : followLabel.Trim();
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

        NPCNavAgent nav = interactable.GetComponentInParent<NPCNavAgent>();
        if (nav == null)
        {
            return;
        }

        Transform t = NPCNavAgent.ResolvePlayerFollowTarget();
        if (nav.followTarget != null)
        {
            nav.StopFollowing();
        }
        else if (t != null)
        {
            nav.Follow(t);
        }

        if (interactionUI != null && interactable.Definition != null)
        {
            interactionUI.SetDefinition(interactable.Definition, interactable);
        }
    }
}
