using UnityEngine;

public abstract class NpcInteractionOptionAction : ScriptableObject
{
    public abstract bool IsValid(NpcInteractionOption option);

    public abstract void Execute(
        InteractionUIController interactionUI,
        NpcProximityInteractable interactable,
        NpcInteractionOption option);
}
