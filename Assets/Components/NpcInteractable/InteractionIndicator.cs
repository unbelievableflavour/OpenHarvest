using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Drives the <c>interactionIndicator</c> <see cref="GameObject"/> (and its hierarchy) from F-key NPC proximity,
/// via <see cref="NpcProximityInteractable.FKeyInRangeWithNpcChanged"/> (no per-frame scene scans).
/// </summary>
public class InteractionIndicator : MonoBehaviour
{
    [SerializeField, Tooltip("Root shown when F-key interaction is in range (children move with it). Do not set to this GameObject or it will stop receiving events when hidden—put visuals under a child, or use another object as root.")]
    private GameObject interactionIndicator;

    private bool _warnedAboutSelfTarget;

    [SerializeField, Tooltip("If true, interactionIndicator is on when an in-range F-key interactable is available. If false, inverted.")]
    private bool showWhenInteractionAvailable = true;

    [SerializeField, Tooltip("If true, only show when an in-range F-key NPC has a NpcInteractableDefinition.")]
    private bool requireNpcDefinition;

    private readonly HashSet<NpcProximityInteractable> _inRangeFKeyNpcs = new();

    private void OnEnable()
    {
        NpcProximityInteractable.FKeyInRangeWithNpcChanged += OnFKeyRange;
        Resync();
    }

    private void OnDisable()
    {
        NpcProximityInteractable.FKeyInRangeWithNpcChanged -= OnFKeyRange;
        _inRangeFKeyNpcs.Clear();
        ApplyVisibility();
    }

    private void Resync()
    {
        _inRangeFKeyNpcs.Clear();
        NpcProximityInteractable[] all = FindObjectsByType<NpcProximityInteractable>(FindObjectsSortMode.None);
        for (int i = 0; i < all.Length; i++)
        {
            NpcProximityInteractable n = all[i];
            if (n == null || !n.UseFKeyInteraction || !n.IsInRange)
            {
                continue;
            }

            if (requireNpcDefinition && n.Definition == null)
            {
                continue;
            }

            _inRangeFKeyNpcs.Add(n);
        }

        ApplyVisibility();
    }

    private void OnFKeyRange(NpcProximityInteractable npc, bool inRange)
    {
        if (npc == null)
        {
            return;
        }

        if (inRange)
        {
            if (requireNpcDefinition && npc.Definition == null)
            {
                return;
            }

            _inRangeFKeyNpcs.Add(npc);
        }
        else
        {
            _inRangeFKeyNpcs.Remove(npc);
        }

        ApplyVisibility();
    }

    private void ApplyVisibility()
    {
        if (interactionIndicator == null)
        {
            return;
        }

        if (interactionIndicator == gameObject)
        {
            if (!_warnedAboutSelfTarget)
            {
                _warnedAboutSelfTarget = true;
                Debug.LogWarning(
                    "[InteractionIndicator] interactionIndicator must not be this GameObject. Parent your visuals and assign that child (or another object).",
                    this);
            }

            return;
        }

        bool available = _inRangeFKeyNpcs.Count > 0;
        bool show = showWhenInteractionAvailable ? available : !available;
        interactionIndicator.SetActive(show);
    }
}
