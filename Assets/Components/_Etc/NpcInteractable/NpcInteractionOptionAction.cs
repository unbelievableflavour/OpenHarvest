using UnityEngine;

public abstract class NpcInteractionOptionAction : ScriptableObject
{
    public virtual string ResolveDisplayName(NpcInteractionOption option, NpcProximityInteractable interactable)
    {
        if (option == null || string.IsNullOrWhiteSpace(option.displayName))
        {
            return "Option";
        }

        return option.displayName.Trim();
    }

    public abstract bool IsValid(NpcInteractionOption option);

    public abstract void Execute(
        InteractionUIController interactionUI,
        NpcProximityInteractable interactable,
        NpcInteractionOption option);
}
