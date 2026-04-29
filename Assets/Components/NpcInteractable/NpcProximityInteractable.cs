using System;
using UnityEngine;

/// <summary>
/// Place on an NPC. Uses <see cref="NpcInteractableDefinition"/> for name + options.
/// Tracks when <see cref="Camera.main"/> is within range so you can show UI or call <see cref="TryOpenInteraction"/>.
/// </summary>
public class NpcProximityInteractable : MonoBehaviour
{
    [SerializeField] private NpcInteractableDefinition definition;

    [Header("Proximity")]
    [SerializeField] private float maxDistance = 2f;

    public NpcInteractableDefinition Definition => definition;
    public bool IsInRange { get; private set; }
    public float MaxDistance => maxDistance;
    public bool UseFKeyInteraction => true;

    /// <summary>
    /// Fires when an NPC with <see cref="UseFKeyInteraction"/> has its in-range state change for F-key.
    /// Use this for prompts instead of polling. <paramref name="inRangeFKey"/> is the new in-range value for that NPC.
    /// </summary>
    public static event Action<NpcProximityInteractable, bool> FKeyInRangeWithNpcChanged;

    public event Action EnteredRange;
    public event Action ExitedRange;

    public event Action<NpcInteractableDefinition, NpcProximityInteractable> InteractionOpened;

    private Transform _target;

    private void Awake()
    {
        ResolveTarget();
    }

    private void OnEnable()
    {
        ResolveTarget();
    }

    private void Update()
    {
        if (_target == null)
        {
            ResolveTarget();
        }

        if (definition == null)
        {
            return;
        }

        bool inRange = IsCloseEnough();
        if (inRange == IsInRange)
        {
            return;
        }

        IsInRange = inRange;
        if (IsInRange)
        {
            EnteredRange?.Invoke();
        }
        else
        {
            ExitedRange?.Invoke();
        }

        FKeyInRangeWithNpcChanged?.Invoke(this, IsInRange);
    }

    private void OnDestroy()
    {
        if (IsInRange)
        {
            FKeyInRangeWithNpcChanged?.Invoke(this, false);
        }
    }

    public bool TryOpenInteraction()
    {
        if (definition == null)
        {
            return false;
        }

        if (!IsCloseEnough())
        {
            return false;
        }

        InteractionOpened?.Invoke(definition, this);
        return true;
    }

    private bool IsCloseEnough()
    {
        if (_target == null)
        {
            return false;
        }

        float d = Vector3.Distance(transform.position, _target.position);
        return d <= maxDistance;
    }

    private void ResolveTarget()
    {
        _target = Camera.main != null ? Camera.main.transform : null;
    }
}
