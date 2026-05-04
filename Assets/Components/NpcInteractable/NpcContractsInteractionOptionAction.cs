using UnityEngine;

[CreateAssetMenu(
    fileName = "Npc Contracts Option Action",
    menuName = "OpenHarvest/NPC/Interaction Option Actions/Contracts",
    order = 2)]
public class NpcContractsInteractionOptionAction : NpcInteractionOptionAction
{
    [Tooltip("Prefab instantiated under the Interaction UI when this option is clicked.")]
    [SerializeField] private GameObject contractsPrefab;

    public override bool IsValid(NpcInteractionOption option)
    {
        return option != null
            && !string.IsNullOrWhiteSpace(option.displayName)
            && contractsPrefab != null;
    }

    public override void Execute(
        InteractionUIController interactionUI,
        NpcProximityInteractable interactable,
        NpcInteractionOption option)
    {
        if (interactionUI == null || contractsPrefab == null)
        {
            return;
        }

        interactionUI.ShowInstancedOptionContent(contractsPrefab);
    }
}
