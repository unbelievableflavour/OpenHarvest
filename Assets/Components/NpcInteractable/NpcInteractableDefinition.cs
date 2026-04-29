using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NpcInteractionOption
{
    [Tooltip("Stable id for code, events, or saves (e.g. shop, quest_greeting).")]
    public string optionId;

    [Tooltip("Text shown to the player for this choice.")]
    public string displayName;

    [TextArea(2, 6)]
    [Tooltip("Optional extra line for tooltips or dialogue previews.")]
    public string details;
}

[CreateAssetMenu(fileName = "New Npc Interactable", menuName = "OpenHarvest/NPC/Interactable Definition", order = 0)]
public class NpcInteractableDefinition : ScriptableObject
{
    [Tooltip("Display name of this character.")]
    public string npcName = "NPC";

    [Tooltip("If true and this NPC has an NPCNavAgent, show Follow / Stop following above Goodbye.")]
    public bool showFollowToggle = true;

    [Tooltip("Choices offered when the player opens interaction with this NPC.")]
    public List<NpcInteractionOption> options = new List<NpcInteractionOption>();
}
