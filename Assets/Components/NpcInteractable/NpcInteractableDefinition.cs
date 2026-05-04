using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NpcInteractionOption
{
    [Tooltip("Text shown to the player for this choice.")]
    public string displayName = "Option";

    [Tooltip("Polymorphic behavior asset for this option.")]
    public NpcInteractionOptionAction action;

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(displayName)
            && action != null
            && action.IsValid(this);
    }

    public void OnSelected(InteractionUIController interactionUI, NpcProximityInteractable interactable)
    {
        if (interactionUI == null || action == null) return;
        action.Execute(interactionUI, interactable, this);
    }
}

[CreateAssetMenu(fileName = "New Npc Interactable", menuName = "OpenHarvest/NPC/Interactable Definition", order = 0)]
public class NpcInteractableDefinition : ScriptableObject
{
    [Tooltip("Display name of this character.")]
    public string npcName = "NPC";

    [TextArea(1, 4)]
    [Tooltip("Optional line under the name; shown on the interaction panel and spoken when the panel opens (requires NPCVoice on this NPC).")]
    public string subtitle = "";

    [Tooltip("If true and this NPC has an NPCNavAgent, show Follow / Stop following above Goodbye.")]
    public bool showFollowToggle = true;

    [Tooltip("Choices offered when the player opens interaction with this NPC.")]
    public List<NpcInteractionOption> options = new List<NpcInteractionOption>();
}
