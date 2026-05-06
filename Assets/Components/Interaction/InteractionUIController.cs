using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class InteractionOptionIconMapping
{
    [Tooltip("Type name key, e.g. NpcGiftInteractionOptionAction or QuestGiftNode.")]
    public string actionType;
    public Sprite icon;
}

/// <summary>
/// Shows NPC name and optional subtitle from <see cref="NpcInteractableDefinition"/>, fills a scroll list with one
/// button per <see cref="NpcInteractionOption"/>, then a final <b>Goodbye</b> button.
/// </summary>
public class InteractionUIController : MonoBehaviour
{
    private const string GoodbyeLabel = "Goodbye";
    private const string MainViewId = "main";
    private const string CurrentInteractionViewId = "currentInteraction";
    private const string InteractionUiToMainEventName = "interactionUIToMain";
    private const string QuestOptionIconKey = "NpcQuestInteractableOptionAction";

    [SerializeField] private Text npcNameText;
    [SerializeField] private Text npcSubtitleText;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Button optionButtonTemplate;
    [SerializeField, Tooltip("ViewSwitcher used by this interaction UI (must contain a 'main' view).")]
    private ViewSwitcher viewSwitcher;
    [SerializeField, Tooltip("Container for spawned option content (your 'currentInteraction' view root/content).")]
    private Transform currentInteractionRoot;
    [SerializeField, Tooltip("Optional. If null, uses a UIEventSender on this object or a parent. Goodbye still emits \"default\" via EventManager if none.")]
    private UIEventSender uiEventSender;
    [SerializeField, Tooltip("Maps action/node type names to option button icons.")]
    private List<InteractionOptionIconMapping> optionIconMappings = new List<InteractionOptionIconMapping>();

    private readonly List<Button> _spawnedButtons = new List<Button>();
    private NpcInteractableDefinition _activeDefinition;
    private NpcProximityInteractable _activeInteractable;
    private bool _hasDefaultUiPlacement;
    private Transform _defaultUiParent;
    private Vector3 _defaultUiLocalPosition;
    private Quaternion _defaultUiLocalRotation;
    private Vector3 _defaultUiLocalScale;

    public event Action OnGoodbye;

    private void Awake()
    {
        CaptureDefaultUiPlacementIfNeeded();
    }

    private void OnEnable()
    {
        EventManager.Subscribe(InteractionUiToMainEventName, HandleInteractionUiToMain);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(InteractionUiToMainEventName, HandleInteractionUiToMain);
        SetNpcToIdle(_activeInteractable);
        RestoreDefaultUiPlacement();

        ClearOptions();
    }

    public void SetDefinition(NpcInteractableDefinition definition, NpcProximityInteractable interactable = null)
    {
        SetDefinitionInternal(definition, interactable, speakSubtitle: true);
    }

    private void SetDefinitionInternal(
        NpcInteractableDefinition definition,
        NpcProximityInteractable interactable,
        bool speakSubtitle)
    {
        SetMainViewIfReady();
        ClearOptions();
        _activeDefinition = definition;
        _activeInteractable = interactable;
        UpdateInteractionUiPlacement(interactable);
        SetNpcToIdle(interactable);

        if (npcNameText != null)
        {
            npcNameText.text = definition != null && !string.IsNullOrEmpty(definition.npcName)
                ? definition.npcName
                : "—";
        }

        string subtitle = definition != null && !string.IsNullOrWhiteSpace(definition.subtitle)
            ? definition.subtitle.Trim()
            : string.Empty;

        if (npcSubtitleText != null)
        {
            npcSubtitleText.text = subtitle;
            npcSubtitleText.gameObject.SetActive(subtitle.Length > 0);
        }

        if (speakSubtitle)
        {
            TrySpeakSubtitle(subtitle, interactable);
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
                string iconKey = selectedOption.action != null ? selectedOption.action.GetType().Name : null;
                AddRow(
                    selectedOption.GetDisplayName(interactable),
                    () => selectedOption.OnSelected(this, interactable),
                    iconKey);
            }
        }

        AddQuestRows(interactable);

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
                DestroyOwnedObject(_spawnedButtons[i].gameObject);
            }
        }

        _spawnedButtons.Clear();
        ClearCurrentInteractionChildren();

        if (npcSubtitleText != null)
        {
            npcSubtitleText.text = string.Empty;
            npcSubtitleText.gameObject.SetActive(false);
        }
    }

    private static void TrySpeakSubtitle(string subtitle, NpcProximityInteractable interactable)
    {
        if (string.IsNullOrEmpty(subtitle) || interactable == null)
        {
            return;
        }

        NPCController npc = interactable.GetComponentInParent<NPCController>();
        if (npc == null)
        {
            return;
        }

        NPCVoice voice = npc.GetComponent<NPCVoice>();
        if (voice == null)
        {
            return;
        }

        voice.Speak(subtitle);
    }

    private void AddRow(string label, Action onClick, string iconKey = null)
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

        ApplyRowIcon(row, iconKey);
        _spawnedButtons.Add(row);
    }

    private void AddQuestRows(NpcProximityInteractable interactable)
    {
        if (interactable == null || QuestRuntimeService.Instance == null)
        {
            return;
        }

        List<QuestNodeBase> nodes = QuestRuntimeService.Instance.GetVisibleNodesForNpc(interactable);
        for (int i = 0; i < nodes.Count; i++)
        {
            QuestNodeBase node = nodes[i];
            if (node == null)
            {
                continue;
            }

            string label = node.GetDisplayLabel();
            AddRow(label, () => QuestRuntimeService.Instance.RunNodeAction(node, this, interactable), QuestOptionIconKey);
        }
    }

    private void ApplyRowIcon(Button row, string iconKey)
    {
        if (row == null)
        {
            return;
        }

        Image iconImage = ResolveIconImage(row);
        if (iconImage == null)
        {
            return;
        }

        Sprite icon = ResolveIcon(iconKey);
        if (icon == null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
            return;
        }

        iconImage.sprite = icon;
        iconImage.enabled = true;
    }

    private static Image ResolveIconImage(Button row)
    {
        if (row == null)
        {
            return null;
        }

        Image buttonImage = row.GetComponent<Image>();
        Image[] images = row.GetComponentsInChildren<Image>(true);
        for (int i = 0; i < images.Length; i++)
        {
            Image image = images[i];
            if (image == null || image == buttonImage)
            {
                continue;
            }

            return image;
        }

        return null;
    }

    private Sprite ResolveIcon(string iconKey)
    {
        if (string.IsNullOrWhiteSpace(iconKey) || optionIconMappings == null)
        {
            return null;
        }
        for (int i = 0; i < optionIconMappings.Count; i++)
        {
            InteractionOptionIconMapping mapping = optionIconMappings[i];
            if (mapping == null || mapping.icon == null || string.IsNullOrWhiteSpace(mapping.actionType))
            {
                continue;
            }

            if (string.Equals(mapping.actionType.Trim(), iconKey, StringComparison.Ordinal))
            {
                return mapping.icon;
            }
        }
        return null;
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

    public void ShowInstancedOptionContent(GameObject prefab)
    {
        ShowInstancedOptionContent(prefab, _activeInteractable, null);
    }

    public void ShowInstancedOptionContent(GameObject prefab, Action<GameObject> afterInstantiate)
    {
        ShowInstancedOptionContent(prefab, _activeInteractable, afterInstantiate);
    }

    public void ShowInstancedOptionContent(
        GameObject prefab,
        NpcProximityInteractable interactable,
        Action<GameObject> afterInstantiate)
    {
        if (prefab == null)
        {
            return;
        }

        viewSwitcher?.setActiveView(CurrentInteractionViewId);
        ClearCurrentInteractionChildren();
        Transform targetParent = ResolveOptionContentParent(interactable);
        GameObject instance = Instantiate(prefab, targetParent, false);
        afterInstantiate?.Invoke(instance);
    }

    private void HandleInteractionUiToMain()
    {
        ClearCurrentInteractionChildren();
        SetNpcToIdle(_activeInteractable);
        SetMainViewIfReady();

        if (_activeDefinition != null)
        {
            SetDefinitionInternal(_activeDefinition, _activeInteractable, speakSubtitle: false);
        }
    }

    private void SetMainViewIfReady()
    {
        if (viewSwitcher == null || viewSwitcher.views == null || viewSwitcher.views.Count == 0 || viewSwitcher.currentView == null)
        {
            return;
        }

        viewSwitcher.setActiveView(MainViewId);
    }

    private void CaptureDefaultUiPlacementIfNeeded()
    {
        if (_hasDefaultUiPlacement)
        {
            return;
        }

        _defaultUiParent = transform.parent;
        _defaultUiLocalPosition = transform.localPosition;
        _defaultUiLocalRotation = transform.localRotation;
        _defaultUiLocalScale = transform.localScale;
        _hasDefaultUiPlacement = true;
    }

    private void UpdateInteractionUiPlacement(NpcProximityInteractable interactable)
    {
        CaptureDefaultUiPlacementIfNeeded();
        if (ShouldUseNpcInteractionRoot() && interactable != null && interactable.CurrentInteractionRoot != null)
        {
            transform.SetParent(interactable.CurrentInteractionRoot, false);
            return;
        }

        RestoreDefaultUiPlacement();
    }

    private void RestoreDefaultUiPlacement()
    {
        CaptureDefaultUiPlacementIfNeeded();
        transform.SetParent(_defaultUiParent, false);
        transform.localPosition = _defaultUiLocalPosition;
        transform.localRotation = _defaultUiLocalRotation;
        transform.localScale = _defaultUiLocalScale;
    }

    private static void SetNpcToIdle(NpcProximityInteractable interactable)
    {
        if (interactable == null)
        {
            return;
        }

        NPCController npc = interactable.GetComponentInParent<NPCController>();
        if (npc == null)
        {
            npc = interactable.GetComponent<NPCController>();
        }

        if (npc == null)
        {
            return;
        }

        npc.BackToIdle();
    }

    private void ClearCurrentInteractionChildren()
    {
        DestroyChildren(currentInteractionRoot);
        if (_activeInteractable != null)
        {
            DestroyChildren(_activeInteractable.CurrentInteractionRoot);
        }
    }

    private Transform ResolveOptionContentParent(NpcProximityInteractable interactable)
    {
        if (ShouldUseNpcInteractionRoot() && interactable != null && interactable.CurrentInteractionRoot != null)
        {
            return interactable.CurrentInteractionRoot;
        }

        return currentInteractionRoot != null ? currentInteractionRoot : transform;
    }

    private bool ShouldUseNpcInteractionRoot()
    {
        HarvestInputManager input = HarvestInputManager.Instance;
        if (input == null)
        {
            input = FindFirstObjectByType<HarvestInputManager>(FindObjectsInactive.Include);
        }

        HarvestSettings settings = input != null ? input.harvestSettings : null;

        if (settings != null && settings.playerMode == PlayerMode.FPS)
        {
            return false;
        }

        return true;
    }

    private static void DestroyChildren(Transform parent)
    {
        if (parent == null)
        {
            return;
        }

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (child == null)
            {
                continue;
            }

            DestroyImmediate(child.gameObject);
        }
    }

    /// <summary>
    /// Spawned list rows: deferred destroy in play mode; immediate in Edit Mode / editor-not-playing.
    /// </summary>
    private static void DestroyOwnedObject(UnityEngine.Object obj)
    {
        if (obj == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(obj);
            return;
        }

        DestroyImmediate(obj);
    }
}
