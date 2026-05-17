using System;
using System.Collections.Generic;
using System.Text;
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
        return action != null
            && action.IsValid(this);
    }

    public string GetDisplayName(NpcProximityInteractable interactable)
    {
        if (action == null)
        {
            return string.IsNullOrWhiteSpace(displayName) ? "Option" : displayName.Trim();
        }

        return action.ResolveDisplayName(this, interactable);
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
    [Tooltip("Stable identifier used to persist follow state across scenes. Auto-generated from npcName.")]
    public string npcId = "";

    [Tooltip("Display name of this character.")]
    public string npcName = "NPC";

    [TextArea(1, 4)]
    [Tooltip("Optional line under the name; shown on the interaction panel and spoken when the panel opens (requires NPCVoice on this NPC).")]
    public string subtitle = "";

    [Tooltip("Choices offered when the player opens interaction with this NPC.")]
    public List<NpcInteractionOption> options = new List<NpcInteractionOption>();

    private void OnValidate()
    {
        string generated = GenerateNpcId(npcName);
        if (!string.IsNullOrWhiteSpace(generated))
        {
            npcId = generated;
        }
    }

    private static string GenerateNpcId(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return "npc";
        }

        string trimmed = source.Trim().ToLowerInvariant();
        var sb = new StringBuilder(trimmed.Length);
        bool previousWasUnderscore = false;

        for (int i = 0; i < trimmed.Length; i++)
        {
            char c = trimmed[i];
            bool isAlphaNumeric = (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9');
            if (isAlphaNumeric)
            {
                sb.Append(c);
                previousWasUnderscore = false;
                continue;
            }

            if (previousWasUnderscore)
            {
                continue;
            }

            sb.Append('_');
            previousWasUnderscore = true;
        }

        string value = sb.ToString().Trim('_');
        return string.IsNullOrWhiteSpace(value) ? "npc" : value;
    }
}
