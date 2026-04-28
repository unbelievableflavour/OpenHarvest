using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// On <b>F</b>, if the player is in range of an <see cref="NpcProximityInteractable"/> with
/// F-key enabled, opens the <c>interaction</c> <see cref="ViewSwitcher"/> view (closest NPC wins
/// if several overlap; view id is not per-NPC).
/// </summary>
public class NpcInteractionKeyListener : MonoBehaviour
{
    private const string InteractionViewId = "interaction";

    [SerializeField, Tooltip("Usually the root Interfaces canvas ViewSwitcher (same one as UIManager). Add a view with id \"interaction\".")]
    private ViewSwitcher viewSwitcher;

    [SerializeField, Tooltip("If unset, the first InteractionUIController in the scene is used (including on inactive objects).")]
    private InteractionUIController interactionUI;

    private void OnEnable()
    {
        if (viewSwitcher == null)
        {
            viewSwitcher = FindFirstObjectByType<ViewSwitcher>();
        }

        if (viewSwitcher != null)
        {
            viewSwitcher.OnViewChanged += OnViewIdChanged;
        }

        HarvestInputManager.Instance.OnAButton += TryOpenClosestNpcInteraction;
    }

    private void OnDisable()
    {
        if (viewSwitcher != null)
        {
            viewSwitcher.OnViewChanged -= OnViewIdChanged;
        }

        HarvestInputManager.Instance.OnAButton -= TryOpenClosestNpcInteraction;
    }

    private void OnViewIdChanged(string viewId)
    {
        if (viewId != InteractionViewId)
        {
            NPCNavAgent.EndAnyInteractionAim();
        }
    }

    private void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.fKey.wasPressedThisFrame)
        {
            return;
        }

        TryOpenClosestNpcInteraction();
    }

    private void TryOpenClosestNpcInteraction()
    {
        if (GameState.Instance?.GetMode() == "build")
        {
            return;
        }

        if (viewSwitcher == null)
        {
            return;
        }

        NpcProximityInteractable[] npcs = FindObjectsByType<NpcProximityInteractable>(FindObjectsSortMode.None);
        NpcProximityInteractable best = null;
        float bestSqr = float.MaxValue;
        var cam = Camera.main;
        Vector3 from = cam != null ? cam.transform.position : transform.position;

        for (int i = 0; i < npcs.Length; i++)
        {
            NpcProximityInteractable n = npcs[i];
            if (n == null || !n.UseFKeyInteraction || !n.IsInRange)
            {
                continue;
            }

            float sqr = (n.transform.position - from).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                best = n;
            }
        }

        if (best == null)
        {
            return;
        }

        // Show the view first so the canvas / layout under \"interaction\" is active when we build rows.
        viewSwitcher.setActiveView(InteractionViewId);

        InteractionUIController ui = interactionUI;
        if (ui == null)
        {
            ui = FindFirstObjectByType<InteractionUIController>(FindObjectsInactive.Include);
        }

        ui?.SetDefinition(best.Definition, best);

        NPCNavAgent nav = best.GetComponentInParent<NPCNavAgent>();
        nav?.BeginInteractionAim(NPCNavAgent.ResolvePlayerLookAtTransform());
    }

}
