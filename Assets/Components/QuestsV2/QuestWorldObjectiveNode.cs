using UnityEngine;

/// <summary>
/// Passive quest step completed by world scripts (e.g. bauble tree) via
/// <see cref="QuestRuntimeService.TryCompleteWorldObjective"/>. Not shown on the NPC interaction quest list.
/// </summary>
[CreateNodeMenu("OpenHarvest/Quests/World Objective Node")]
public class QuestWorldObjectiveNode : QuestNodeBase
{
    [Tooltip("Must match the key passed from gameplay code (e.g. BaubleHangingLocations).")]
    [SerializeField] private string objectiveKey = "default";

    public string ObjectiveKey => objectiveKey != null ? objectiveKey.Trim() : string.Empty;

    public override void RunAction(InteractionUIController interactionUI, NpcProximityInteractable interactable, QuestGraph graph)
    {
        // Progress is driven externally; do not advance from the interaction UI.
    }
}
