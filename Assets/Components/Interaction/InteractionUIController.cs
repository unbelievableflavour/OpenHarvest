using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Fills a scroll list with one button per <see cref="NpcInteractionOption"/> on a
/// <see cref="NpcInteractableDefinition"/>, then a final <b>Goodbye</b> button.
/// </summary>
public class InteractionUIController : MonoBehaviour
{
    private const string GoodbyeLabel = "Goodbye";
    private const string FollowStartLabel = "Follow me";
    private const string FollowStopLabel = "Stop following";
    private const string MainViewId = "main";
    private const string CurrentInteractionViewId = "currentInteraction";
    private const string InteractionUiToMainEventName = "interactionUIToMain";

    [SerializeField] private Text npcNameText;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Button optionButtonTemplate;
    [SerializeField, Tooltip("ViewSwitcher used by this interaction UI (must contain a 'main' view).")]
    private ViewSwitcher viewSwitcher;
    [SerializeField, Tooltip("Container for spawned option content (your 'currentInteraction' view root/content).")]
    private Transform currentInteractionRoot;
    [SerializeField, Tooltip("Optional. If null, uses a UIEventSender on this object or a parent. Goodbye still emits \"default\" via EventManager if none.")]
    private UIEventSender uiEventSender;

    private readonly List<Button> _spawnedButtons = new List<Button>();
    private GameObject _spawnedOptionContent;

    public event Action<string> OnOptionChosen;
    public event Action OnGoodbye;

    private void OnEnable()
    {
        EventManager.Subscribe(InteractionUiToMainEventName, HandleInteractionUiToMain);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(InteractionUiToMainEventName, HandleInteractionUiToMain);

        ClearOptions();
    }

    public void SetDefinition(NpcInteractableDefinition definition, NpcProximityInteractable interactable = null)
    {
        ClearOptions();

        if (npcNameText != null)
        {
            npcNameText.text = definition != null && !string.IsNullOrEmpty(definition.npcName)
                ? definition.npcName
                : "—";
        }

        if (definition != null && definition.options != null)
        {
            for (int i = 0; i < definition.options.Count; i++)
            {
                NpcInteractionOption option = definition.options[i];
                if (option == null || !option.IsValid())
                {
                    continue;
                }

                NpcInteractionOption selectedOption = option;
                AddRow(selectedOption.displayName, () => selectedOption.OnSelected(this, interactable));
            }
        }

        if (ShouldShowFollowToggle(definition, interactable))
        {
            AddFollowToggleRow(interactable);
        }

        AddRow(GoodbyeLabel, OnGoodbyeClicked);
        if (scrollRect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
            scrollRect.normalizedPosition = new Vector2(0f, 1f);
        }
    }

    public void ClearOptions()
    {
        for (int i = 0; i < _spawnedButtons.Count; i++)
        {
            if (_spawnedButtons[i] != null)
            {
                Destroy(_spawnedButtons[i].gameObject);
            }
        }

        _spawnedButtons.Clear();
        ClearCurrentInteractionChildren();
    }

    private void AddRow(string label, Action onClick)
    {
        if (optionButtonTemplate == null || scrollRect?.content == null)
        {
            return;
        }

        Button row = Instantiate(optionButtonTemplate, scrollRect.content);
        row.gameObject.SetActive(true);
        row.onClick.RemoveAllListeners();
        row.onClick.AddListener(() => onClick?.Invoke());

        if (row.GetComponent<LayoutElement>() == null)
        {
            var layout = row.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = 32f;
            layout.preferredHeight = 32f;
            layout.flexibleHeight = 0f;
        }

        Text text = row.GetComponentInChildren<Text>(true);
        if (text != null)
        {
            text.text = label;
        }

        _spawnedButtons.Add(row);
    }

    private static bool ShouldShowFollowToggle(NpcInteractableDefinition definition, NpcProximityInteractable interactable)
    {
        if (interactable == null)
        {
            return false;
        }

        if (interactable.GetComponentInParent<NPCNavAgent>() == null)
        {
            return false;
        }

        if (definition != null && !definition.showFollowToggle)
        {
            return false;
        }

        return true;
    }

    private void AddFollowToggleRow(NpcProximityInteractable interactable)
    {
        if (optionButtonTemplate == null || scrollRect?.content == null)
        {
            return;
        }

        NPCNavAgent nav = interactable.GetComponentInParent<NPCNavAgent>();
        if (nav == null)
        {
            return;
        }

        Button row = Instantiate(optionButtonTemplate, scrollRect.content);
        row.gameObject.SetActive(true);
        row.onClick.RemoveAllListeners();

        if (row.GetComponent<LayoutElement>() == null)
        {
            var layout = row.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = 32f;
            layout.preferredHeight = 32f;
            layout.flexibleHeight = 0f;
        }

        Text text = row.GetComponentInChildren<Text>(true);
        if (text == null)
        {
            return;
        }

        void UpdateFollowLabel()
        {
            text.text = nav.followTarget != null ? FollowStopLabel : FollowStartLabel;
        }

        UpdateFollowLabel();
        row.onClick.AddListener(() =>
        {
            Transform t = NPCNavAgent.ResolvePlayerFollowTarget();
            if (nav.followTarget != null)
            {
                nav.StopFollowing();
            }
            else if (t != null)
            {
                nav.Follow(t);
            }

            UpdateFollowLabel();
        });

        _spawnedButtons.Add(row);
    }

    private void OnGoodbyeClicked()
    {
        UIEventSender sender = uiEventSender != null
            ? uiEventSender
            : GetComponent<UIEventSender>() ?? GetComponentInParent<UIEventSender>();
        if (sender != null)
        {
            sender.SendEvent("default");
        }
        else
        {
            EventManager.Emit("default");
        }

        OnGoodbye?.Invoke();
    }

    public void NotifyOptionChosen(string optionId)
    {
        OnOptionChosen?.Invoke(optionId);
    }

    public void ShowInstancedOptionContent(GameObject prefab)
    {
        ShowInstancedOptionContent(prefab, null);
    }

    public void ShowInstancedOptionContent(GameObject prefab, Action<GameObject> afterInstantiate)
    {
        if (prefab == null)
        {
            return;
        }

        viewSwitcher?.setActiveView(CurrentInteractionViewId);
        ClearCurrentInteractionChildren();
        Transform targetParent = currentInteractionRoot != null ? currentInteractionRoot : transform;
        _spawnedOptionContent = Instantiate(prefab, targetParent, false);
        afterInstantiate?.Invoke(_spawnedOptionContent);
    }

    private void HandleInteractionUiToMain()
    {
        ClearCurrentInteractionChildren();
        viewSwitcher?.setActiveView(MainViewId);
    }

    private void ClearCurrentInteractionChildren()
    {
        Transform parent = currentInteractionRoot != null ? currentInteractionRoot : null;
        if (parent == null)
        {
            return;
        }

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (child != null)
            {
                Destroy(child.gameObject);
            }
        }

        _spawnedOptionContent = null;
    }
}
