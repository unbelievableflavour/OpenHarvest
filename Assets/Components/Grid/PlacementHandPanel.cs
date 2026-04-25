using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class PlacementHandPanel : MonoBehaviour
{
    private const string EmptyOwnedItemsText = "No owned placeables";

    private PlacementSystem placementSystem;
    private Text itemLabel;
    private ScrollRect itemListScroll;
    private RectTransform itemListContent;
    private Button itemTemplateButton;
    private Button buttonPlace;
    private Button buttonPrev;
    private Button buttonNext;
    private Button buttonDelete;
    private Button buttonMove;
    private ViewSwitcher viewSwitcher;
    private readonly List<Button> runtimeItemButtons = new List<Button>();
    private readonly List<Text> runtimeItemLabels = new List<Text>();
    private readonly List<int> runtimeItemIndices = new List<int>();
    private bool isBoundToPlacementSystem;

    [Header("Optional ViewSwitcher visibility")]
    [SerializeField] private ViewSwitcher panelVisibilityViewSwitcher;
    [SerializeField] private string panelVisibleViewId = "build";
    [SerializeField] private string panelHiddenViewId = "default";

    [Header("Panel UI (required)")]
    [Tooltip("Drag the `PlacementHandPanelUI` root here. This component may live elsewhere on the player.")]
    [SerializeField] private GameObject panelUiRoot;

    [Header("Visibility")]
    [SerializeField]
    private bool autoShowInBuildMode = true;

    [SerializeField]
    private bool enableKeyboardTabToggle = false;

    private bool isPanelVisibleOverride = true;
    private bool isPanelUiWired;

    private void Awake()
    {
        isPanelVisibleOverride = autoShowInBuildMode;
        TryResolvePlacementSystem();
        TryInitializePanelUi();
    }

    private void OnEnable()
    {
        TryInitializePanelUi();
        TryBindToPlacementSystem();
    }

    private void OnDisable()
    {
        UnbindFromPlacementSystem();
    }

    private void Update()
    {
        if (!isBoundToPlacementSystem)
        {
            TryBindToPlacementSystem();
        }

        if (enableKeyboardTabToggle &&
            Keyboard.current != null &&
            Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (GameState.Instance == null)
            {
                return;
            }

            if (GameState.Instance.GetMode() != "build")
            {
                GameState.Instance.SwitchToMode("build");
            }
            TogglePanelVisibilityFromKeyboard();
        }
    }

    private void TryInitializePanelUi()
    {
        if (isPanelUiWired)
        {
            return;
        }

        CachePanelReferences();
        EnsurePointerUiInfrastructure();
        ConfigurePanelCanvasForPointerInput();
        SetPanelVisible(false);
        isPanelUiWired = true;
    }

    private bool TryResolvePlacementSystem()
    {
        if (placementSystem != null)
        {
            return true;
        }

        placementSystem = FindFirstObjectByType<PlacementSystem>();
        return placementSystem != null;
    }

    private void TryBindToPlacementSystem()
    {
        if (isBoundToPlacementSystem)
        {
            return;
        }

        if (!TryResolvePlacementSystem())
        {
            return;
        }

        placementSystem.OnPlacementModeChanged += HandlePlacementModeChanged;
        placementSystem.OnPlacementSelectionChanged += HandlePlacementSelectionChanged;
        placementSystem.OnPlacementInventoryChanged += HandlePlacementInventoryChanged;
        placementSystem.OnPlacementToolChanged += HandlePlacementToolChanged;
        isBoundToPlacementSystem = true;

        RebuildItemList();
        HandlePlacementModeChanged(placementSystem.IsPlacementModeActive());
        HandlePlacementSelectionChanged(placementSystem.GetCurrentPlacementItemName());
    }

    private void UnbindFromPlacementSystem()
    {
        if (!isBoundToPlacementSystem || placementSystem == null)
        {
            return;
        }

        placementSystem.OnPlacementModeChanged -= HandlePlacementModeChanged;
        placementSystem.OnPlacementSelectionChanged -= HandlePlacementSelectionChanged;
        placementSystem.OnPlacementInventoryChanged -= HandlePlacementInventoryChanged;
        placementSystem.OnPlacementToolChanged -= HandlePlacementToolChanged;
        isBoundToPlacementSystem = false;
    }

    private void HandlePlacementModeChanged(bool isActive)
    {
        if (panelVisibilityViewSwitcher == null)
        {
            return;
        }

        bool shouldShow = ShouldShowPanel(isActive);
        SetPanelVisible(shouldShow);
        if (isActive)
        {
            RebuildItemList();
            RefreshItemButtonTexts();
            RefreshItemButtonHighlights();
            RefreshModeViews();
        }
    }

    private void SetPanelVisible(bool visible)
    {
        panelVisibilityViewSwitcher.setActiveView(visible ? panelVisibleViewId : panelHiddenViewId);
    }

    private void HandlePlacementSelectionChanged(string selectedItemName)
    {
        if (itemLabel == null)
        {
            return;
        }

        itemLabel.text = string.IsNullOrWhiteSpace(selectedItemName) ? "Build" : selectedItemName;
        RefreshItemButtonTexts();
        RefreshItemButtonHighlights();
    }

    private void HandlePlacementInventoryChanged()
    {
        RebuildItemList();
    }

    private void HandlePlacementToolChanged(PlacementSystem.PlacementToolMode _)
    {
        RefreshToolButtonHighlights();
        RefreshModeViews();
    }

    public void ConfigureVisibilityForPcToggle()
    {
        TryInitializePanelUi();
        autoShowInBuildMode = false;
        enableKeyboardTabToggle = true;
        isPanelVisibleOverride = false;
        ConfigureCanvasForPcToggle();
        SetPanelVisible(false);
    }

    private bool ShouldShowPanel(bool isBuildModeActive)
    {
        if (!isBuildModeActive)
        {
            return false;
        }

        if (enableKeyboardTabToggle)
        {
            return isPanelVisibleOverride;
        }

        return autoShowInBuildMode;
    }

    private void TogglePanelVisibilityFromKeyboard()
    {
        if (placementSystem == null || panelVisibilityViewSwitcher == null)
        {
            return;
        }

        if (!placementSystem.IsPlacementModeActive())
        {
            isPanelVisibleOverride = false;
            SetPanelVisible(false);
            return;
        }

        isPanelVisibleOverride = !isPanelVisibleOverride;
        SetPanelVisible(isPanelVisibleOverride);
        if (isPanelVisibleOverride)
        {
            RebuildItemList();
        }
    }

    private void ConfigurePanelCanvasForPointerInput()
    {
        Camera eventCamera = GetComponentInParent<Camera>();
        if (eventCamera == null)
        {
            eventCamera = Camera.main;
        }

        Canvas[] canvases = panelUiRoot.GetComponentsInChildren<Canvas>(true);
        for (int i = 0; i < canvases.Length; i++)
        {
            Canvas canvas = canvases[i];
            if (canvas == null)
            {
                continue;
            }

            if (canvas.renderMode == RenderMode.WorldSpace && eventCamera != null)
            {
                canvas.worldCamera = eventCamera;
            }

            if (canvas.GetComponent<GraphicRaycaster>() == null)
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }
        }
    }

    private void ConfigureCanvasForPcToggle()
    {
        Camera eventCamera = GetComponentInParent<Camera>();
        if (eventCamera == null)
        {
            eventCamera = Camera.main;
        }

        Canvas[] canvases = panelUiRoot.GetComponentsInChildren<Canvas>(true);
        for (int i = 0; i < canvases.Length; i++)
        {
            Canvas canvas = canvases[i];
            if (canvas == null)
            {
                continue;
            }

            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = eventCamera;
            canvas.planeDistance = 1f;
        }
    }

    private static void EnsurePointerUiInfrastructure()
    {
        EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            eventSystem = eventSystemObject.GetComponent<EventSystem>();
        }

        if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
        {
            eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
        }
    }

    private void CachePanelReferences()
    {
        var root = panelUiRoot.transform;
        itemLabel = FindTextByName(root, "CurrentItemLabel");
        itemListScroll = FindScrollRectByName(root, "ItemListScroll");
        itemListContent = FindRectTransformByName(root, "ItemListContent");
        itemTemplateButton = FindButtonByName(root, "ItemTemplateButton");
        buttonPlace = FindButtonByName(root, "ButtonPlace");
        buttonPrev = FindButtonByName(root, "ButtonPrev");
        buttonNext = FindButtonByName(root, "ButtonNext");
        buttonDelete = FindButtonByName(root, "ButtonDelete");
        buttonMove = FindButtonByName(root, "ButtonMove");
        viewSwitcher = panelUiRoot.GetComponent<ViewSwitcher>();
        EnsureActionButtons();
    }

    private void EnsureActionButtons()
    {
        BindButton(buttonPlace, HandlePlaceClicked);
        BindButton(buttonPrev, HandlePreviousClicked);
        BindButton(buttonNext, HandleNextClicked);
        BindButton(buttonDelete, HandleDeleteClicked);
        BindButton(buttonMove, HandleMoveClicked);
        RefreshToolButtonHighlights();
        RefreshModeViews();
    }

    private static void BindButton(Button button, Action handler)
    {
        if (button == null || handler == null)
        {
            return;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => handler());
    }

    private void RebuildItemList()
    {
        if (itemListContent == null || itemTemplateButton == null || placementSystem == null)
        {
            Debug.LogWarning("[PlacementHandPanel] Missing scroll list references for placement UI.");
            return;
        }

        for (int i = 0; i < runtimeItemButtons.Count; i++)
        {
            if (runtimeItemButtons[i] != null)
            {
                Destroy(runtimeItemButtons[i].gameObject);
            }
        }
        runtimeItemButtons.Clear();
        runtimeItemLabels.Clear();
        runtimeItemIndices.Clear();

        itemTemplateButton.gameObject.SetActive(false);

        int count = placementSystem.GetPlacementItemCount();
        for (int i = 0; i < count; i++)
        {
            int ownedCount = placementSystem.GetPlacementItemOwnedCountAt(i);
            if (ownedCount <= 0)
            {
                continue;
            }

            int itemIndex = i;
            Button button = Instantiate(itemTemplateButton, itemListContent);
            button.name = "ItemButton_" + i;
            button.gameObject.SetActive(true);
            button.interactable = true;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => HandleItemClicked(itemIndex));

            RectTransform buttonRect = button.GetComponent<RectTransform>();
            if (buttonRect != null)
            {
                buttonRect.anchorMin = new Vector2(0f, 1f);
                buttonRect.anchorMax = new Vector2(1f, 1f);
                buttonRect.sizeDelta = new Vector2(0f, 34f);
            }

            var layoutElement = button.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = button.gameObject.AddComponent<LayoutElement>();
            }
            layoutElement.preferredHeight = 34f;
            layoutElement.minHeight = 30f;
            layoutElement.flexibleWidth = 1f;

            Text label = ResolveButtonLabel(button);
            if (label == null)
            {
                label = EnsureRowText(button.transform);
            }

            if (label != null)
            {
                label.color = Color.white;
            }

            runtimeItemButtons.Add(button);
            runtimeItemLabels.Add(label);
            runtimeItemIndices.Add(itemIndex);
        }

        if (runtimeItemButtons.Count == 0)
        {
            Button emptyRow = Instantiate(itemTemplateButton, itemListContent);
            emptyRow.name = "ItemButton_EmptyState";
            emptyRow.gameObject.SetActive(true);
            emptyRow.interactable = false;
            emptyRow.onClick.RemoveAllListeners();

            Text emptyLabel = ResolveButtonLabel(emptyRow);
            if (emptyLabel != null)
            {
                emptyLabel.text = EmptyOwnedItemsText;
                emptyLabel.color = new Color(0.8f, 0.82f, 0.87f, 1f);
            }

            runtimeItemButtons.Add(emptyRow);
            runtimeItemLabels.Add(emptyLabel);
            runtimeItemIndices.Add(-1);
        }

        if (itemListScroll != null)
        {
            itemListScroll.verticalNormalizedPosition = 1f;
        }

        RefreshItemButtonTexts();
        RefreshItemButtonHighlights();
    }

    private void RefreshItemButtonTexts()
    {
        if (placementSystem == null)
        {
            return;
        }

        int count = runtimeItemButtons.Count;
        for (int i = 0; i < count; i++)
        {
            if (i >= runtimeItemIndices.Count)
            {
                continue;
            }

            int dataIndex = runtimeItemIndices[i];
            Text label = i < runtimeItemLabels.Count ? runtimeItemLabels[i] : null;
            if (label == null)
            {
                continue;
            }

            if (dataIndex < 0)
            {
                label.text = EmptyOwnedItemsText;
                continue;
            }

            string itemName = placementSystem.GetPlacementItemNameAt(dataIndex);
            if (string.IsNullOrWhiteSpace(itemName))
            {
                itemName = "Item " + dataIndex;
            }

            int ownedCount = placementSystem.GetPlacementItemOwnedCountAt(dataIndex);
            label.text = itemName + " (x" + ownedCount + ")";
        }
    }

    private void RefreshItemButtonHighlights()
    {
        if (placementSystem == null)
        {
            return;
        }

        int selectedIndex = placementSystem.GetCurrentPlacementItemIndex();
        for (int i = 0; i < runtimeItemButtons.Count; i++)
        {
            if (i >= runtimeItemIndices.Count)
            {
                continue;
            }

            int dataIndex = runtimeItemIndices[i];
            Button button = runtimeItemButtons[i];
            if (button == null)
            {
                continue;
            }

            Image image = button.GetComponent<Image>();
            if (image == null)
            {
                continue;
            }

            if (dataIndex < 0)
            {
                image.color = new Color(0.17f, 0.17f, 0.2f, 0.9f);
                continue;
            }

            image.color = dataIndex == selectedIndex
                ? new Color(0.16f, 0.38f, 0.7f, 0.95f)
                : new Color(0.2f, 0.2f, 0.24f, 0.95f);
        }
    }

    private static Text FindTextByName(Transform root, string name)
    {
        Text[] texts = root.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] != null && texts[i].name == name)
            {
                return texts[i];
            }
        }

        return null;
    }

    private static Button FindButtonByName(Transform root, string name)
    {
        Button[] buttons = root.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null && buttons[i].name == name)
            {
                return buttons[i];
            }
        }

        return null;
    }

    private static ScrollRect FindScrollRectByName(Transform root, string name)
    {
        ScrollRect[] scrollRects = root.GetComponentsInChildren<ScrollRect>(true);
        for (int i = 0; i < scrollRects.Length; i++)
        {
            if (scrollRects[i] != null && scrollRects[i].name == name)
            {
                return scrollRects[i];
            }
        }

        return null;
    }

    private static RectTransform FindRectTransformByName(Transform root, string name)
    {
        RectTransform[] rects = root.GetComponentsInChildren<RectTransform>(true);
        for (int i = 0; i < rects.Length; i++)
        {
            if (rects[i] != null && rects[i].name == name)
            {
                return rects[i];
            }
        }

        return null;
    }

    private static Text EnsureRowText(Transform buttonTransform)
    {
        GameObject textObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(buttonTransform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(8f, 0f);
        textRect.offsetMax = new Vector2(-8f, 0f);

        Text text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.alignment = TextAnchor.MiddleLeft;
        text.color = Color.white;
        text.fontSize = 18;
        text.raycastTarget = false;
        text.text = string.Empty;
        return text;
    }

    private static Text ResolveButtonLabel(Button button)
    {
        if (button == null)
        {
            return null;
        }

        Text label = FindTextByName(button.transform, "Label");
        if (label != null)
        {
            return label;
        }

        label = FindTextByName(button.transform, "Text");
        if (label != null)
        {
            return label;
        }

        return button.GetComponentInChildren<Text>(true);
    }

    private void HandleItemClicked(int itemIndex)
    {
        if (!TryResolvePlacementSystem())
        {
            return;
        }

        placementSystem.SetPlacementToolPlace();
        placementSystem.SelectItemByIndex(itemIndex);
    }

    private void HandlePlaceClicked()
    {
        if (!TryResolvePlacementSystem())
        {
            return;
        }

        placementSystem.SetPlacementToolPlace();
        RefreshToolButtonHighlights();
    }

    private void HandlePreviousClicked()
    {
        if (!TryResolvePlacementSystem())
        {
            return;
        }

        placementSystem.SetPlacementToolPlace();
        placementSystem.SelectPreviousItem();
    }

    private void HandleNextClicked()
    {
        if (!TryResolvePlacementSystem())
        {
            return;
        }

        placementSystem.SetPlacementToolPlace();
        placementSystem.SelectNextItem();
    }

    private void HandleDeleteClicked()
    {
        if (!TryResolvePlacementSystem())
        {
            return;
        }

        placementSystem.SetPlacementToolDelete();
        RefreshToolButtonHighlights();
    }

    private void HandleMoveClicked()
    {
        if (!TryResolvePlacementSystem())
        {
            return;
        }

        placementSystem.SetPlacementToolMove();
        RefreshToolButtonHighlights();
    }

    private void RefreshToolButtonHighlights()
    {
        if (!TryResolvePlacementSystem())
        {
            return;
        }

        PlacementSystem.PlacementToolMode activeMode = placementSystem.GetCurrentPlacementToolMode();
        ApplyToolButtonColor(buttonPlace, activeMode == PlacementSystem.PlacementToolMode.Place);
        ApplyToolButtonColor(buttonDelete, activeMode == PlacementSystem.PlacementToolMode.Delete);
        ApplyToolButtonColor(buttonMove, activeMode == PlacementSystem.PlacementToolMode.Move);
    }

    private void RefreshModeViews()
    {
        if (!TryResolvePlacementSystem())
        {
            return;
        }

        if (viewSwitcher == null || viewSwitcher.views == null || viewSwitcher.views.Count == 0)
        {
            return;
        }

        PlacementSystem.PlacementToolMode activeMode = placementSystem.GetCurrentPlacementToolMode();
        string targetViewId = activeMode == PlacementSystem.PlacementToolMode.Move
            ? "move"
            : activeMode == PlacementSystem.PlacementToolMode.Delete
                ? "delete"
                : "place";
        viewSwitcher.setActiveView(targetViewId);
    }

    private static void ApplyToolButtonColor(Button button, bool isActive)
    {
        if (button == null)
        {
            return;
        }

        Image image = button.GetComponent<Image>();
        if (image == null)
        {
            return;
        }

        image.color = isActive
            ? new Color(0.16f, 0.38f, 0.7f, 0.95f)
            : new Color(0.2f, 0.2f, 0.24f, 0.95f);
    }
}
