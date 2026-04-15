using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class PlacementHandPanel : MonoBehaviour
{
    private const string DefaultPanelPrefabPath = "UI/PlacementHandPanelUI";
    private const string EmptyOwnedItemsText = "No owned placeables";
    public static bool IsPcPanelInteractionActive { get; private set; }

    private PlacementSystem placementSystem;
    private GameObject panelRoot;
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
    private GameObject placeViewRoot;
    private GameObject moveViewRoot;
    private GameObject deleteViewRoot;
    private Text moveViewHelpText;
    private Text deleteViewHelpText;
    private readonly List<Button> runtimeItemButtons = new List<Button>();
    private readonly List<Text> runtimeItemLabels = new List<Text>();
    private readonly List<int> runtimeItemIndices = new List<int>();
    private bool isBoundToPlacementSystem;

    [SerializeField]
    private GameObject placementPanelPrefab;

    [SerializeField]
    private Vector3 panelLocalPosition = new Vector3(0.06f, 0.02f, 0.08f);

    [SerializeField]
    private Vector3 panelLocalEulerAngles = new Vector3(20f, 180f, 0f);

    [SerializeField]
    private Vector3 panelLocalScale = new Vector3(0.0008f, 0.0008f, 0.0008f);

    [Header("Visibility")]
    [SerializeField]
    private bool autoShowInBuildMode = true;

    [SerializeField]
    private bool enableKeyboardTabToggle = false;

    private bool isPanelVisibleOverride = true;
    private FirstPersonController cachedFpsController;
    private bool cachedControllerState;
    private bool cachedCameraCanMove = true;
    private bool cachedPlayerCanMove = true;
    private CursorLockMode cachedCursorLockMode = CursorLockMode.Locked;
    private bool cachedCursorVisible = false;

    private void Awake()
    {
        isPanelVisibleOverride = autoShowInBuildMode;
        TryResolvePlacementSystem();
        EnsurePanelExists();
    }

    private void OnEnable()
    {
        TryBindToPlacementSystem();
    }

    private void OnDisable()
    {
        ApplyPcInteractionState(false);
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
            TogglePanelVisibilityFromKeyboard();
        }
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
        if (panelRoot == null)
        {
            return;
        }

        panelRoot.SetActive(ShouldShowPanel(isActive));
        ApplyPcInteractionState(panelRoot.activeSelf);
        if (isActive)
        {
            RefreshItemButtonTexts();
            RefreshItemButtonHighlights();
            RefreshModeViews();
        }
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

    private void EnsurePanelExists()
    {
        if (panelRoot == gameObject)
        {
            panelRoot = null;
        }

        if (panelRoot == null)
        {
            panelRoot = transform.Find("PlacementSelectionPanel")?.gameObject;
        }

        if (panelRoot == null)
        {
            if (placementPanelPrefab == null)
            {
                placementPanelPrefab = Resources.Load<GameObject>(DefaultPanelPrefabPath);
            }

            if (placementPanelPrefab == null)
            {
                Debug.LogWarning("[PlacementHandPanel] Missing placement panel prefab at Resources/" + DefaultPanelPrefabPath);
                return;
            }

            panelRoot = Instantiate(placementPanelPrefab, transform);
            panelRoot.name = "PlacementSelectionPanel";
            panelRoot.transform.localPosition = panelLocalPosition;
            panelRoot.transform.localRotation = Quaternion.Euler(panelLocalEulerAngles);
            panelRoot.transform.localScale = panelLocalScale;
        }

        CachePanelReferences();
        EnsurePointerUiInfrastructure();
        ConfigurePanelCanvasForPointerInput();
        panelRoot.SetActive(false);
    }

    public void ConfigureVisibilityForPcToggle()
    {
        autoShowInBuildMode = false;
        enableKeyboardTabToggle = true;
        isPanelVisibleOverride = false;
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
        ApplyPcInteractionState(false);
    }

    public void ConfigurePanelLocalTransform(Vector3 localPosition, Vector3 localEulerAngles, Vector3 localScale)
    {
        panelLocalPosition = localPosition;
        panelLocalEulerAngles = localEulerAngles;
        panelLocalScale = localScale;

        if (panelRoot == null)
        {
            return;
        }

        panelRoot.transform.localPosition = panelLocalPosition;
        panelRoot.transform.localRotation = Quaternion.Euler(panelLocalEulerAngles);
        panelRoot.transform.localScale = panelLocalScale;
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
        if (placementSystem == null || panelRoot == null)
        {
            return;
        }

        if (!placementSystem.IsPlacementModeActive())
        {
            isPanelVisibleOverride = false;
            panelRoot.SetActive(false);
            return;
        }

        isPanelVisibleOverride = !isPanelVisibleOverride;
        panelRoot.SetActive(isPanelVisibleOverride);
        ApplyPcInteractionState(isPanelVisibleOverride);
        if (isPanelVisibleOverride)
        {
            RebuildItemList();
        }
    }

    private void ApplyPcInteractionState(bool panelVisible)
    {
        if (!enableKeyboardTabToggle)
        {
            IsPcPanelInteractionActive = false;
            return;
        }

        if (panelVisible)
        {
            if (!cachedControllerState)
            {
                cachedCursorLockMode = Cursor.lockState;
                cachedCursorVisible = Cursor.visible;
                cachedFpsController = GetComponentInParent<FirstPersonController>();
                if (cachedFpsController != null)
                {
                    cachedCameraCanMove = cachedFpsController.cameraCanMove;
                    cachedPlayerCanMove = cachedFpsController.playerCanMove;
                }
                cachedControllerState = true;
            }

            if (cachedFpsController != null)
            {
                cachedFpsController.cameraCanMove = false;
                cachedFpsController.playerCanMove = false;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            IsPcPanelInteractionActive = true;
            return;
        }

        if (cachedFpsController != null && cachedControllerState)
        {
            cachedFpsController.cameraCanMove = cachedCameraCanMove;
            cachedFpsController.playerCanMove = cachedPlayerCanMove;
        }

        if (cachedControllerState)
        {
            Cursor.lockState = cachedCursorLockMode;
            Cursor.visible = cachedCursorVisible;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        cachedControllerState = false;
        IsPcPanelInteractionActive = false;
    }

    private void ConfigurePanelCanvasForPointerInput()
    {
        if (panelRoot == null)
        {
            return;
        }

        Camera eventCamera = GetComponentInParent<Camera>();
        if (eventCamera == null)
        {
            eventCamera = Camera.main;
        }

        Canvas[] canvases = panelRoot.GetComponentsInChildren<Canvas>(true);
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

        StandaloneInputModule legacyModule = eventSystem.GetComponent<StandaloneInputModule>();
        if (legacyModule != null)
        {
            legacyModule.enabled = false;
        }
    }

    private void CachePanelReferences()
    {
        if (panelRoot == null)
        {
            return;
        }

        itemLabel = FindTextByName(panelRoot.transform, "CurrentItemLabel");
        itemListScroll = FindScrollRectByName(panelRoot.transform, "ItemListScroll");
        itemListContent = FindRectTransformByName(panelRoot.transform, "ItemListContent");
        itemTemplateButton = FindButtonByName(panelRoot.transform, "ItemTemplateButton");
        buttonPlace = FindButtonByName(panelRoot.transform, "ButtonPlace");
        buttonPrev = FindButtonByName(panelRoot.transform, "ButtonPrev");
        buttonNext = FindButtonByName(panelRoot.transform, "ButtonNext");
        buttonDelete = FindButtonByName(panelRoot.transform, "ButtonDelete");
        buttonMove = FindButtonByName(panelRoot.transform, "ButtonMove");
        viewSwitcher = panelRoot.GetComponent<ViewSwitcher>();
        placeViewRoot = FindGameObjectByName(panelRoot.transform, "PlaceView");
        moveViewRoot = FindGameObjectByName(panelRoot.transform, "MoveView");
        deleteViewRoot = FindGameObjectByName(panelRoot.transform, "DeleteView");
        moveViewHelpText = FindTextByName(panelRoot.transform, "MoveViewHelpText");
        deleteViewHelpText = FindTextByName(panelRoot.transform, "DeleteViewHelpText");
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

    private static GameObject FindGameObjectByName(Transform root, string name)
    {
        if (root == null || string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        Transform[] children = root.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] != null && children[i].name == name)
            {
                return children[i].gameObject;
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

        PlacementSystem.PlacementToolMode activeMode = placementSystem.GetCurrentPlacementToolMode();
        bool isPlaceMode = activeMode == PlacementSystem.PlacementToolMode.Place;
        bool isMoveMode = activeMode == PlacementSystem.PlacementToolMode.Move;
        bool isDeleteMode = activeMode == PlacementSystem.PlacementToolMode.Delete;

        // Prefer shared ViewSwitcher pattern when available.
        if (viewSwitcher != null && viewSwitcher.views != null && viewSwitcher.views.Count > 0)
        {
            string targetViewId = isMoveMode ? "move" : isDeleteMode ? "delete" : "place";
            viewSwitcher.setActiveView(targetViewId);
        }
        // If dedicated views exist in prefab but no switcher, drive manually.
        else if (placeViewRoot != null || moveViewRoot != null || deleteViewRoot != null)
        {
            if (placeViewRoot != null)
            {
                placeViewRoot.SetActive(isPlaceMode);
            }

            if (moveViewRoot != null)
            {
                moveViewRoot.SetActive(isMoveMode);
            }

            if (deleteViewRoot != null)
            {
                deleteViewRoot.SetActive(isDeleteMode);
            }
        }
        else
        {
            // Fallback: no separate view roots defined, so hide list outside Place mode.
            if (itemListScroll != null)
            {
                itemListScroll.gameObject.SetActive(isPlaceMode);
            }
        }

        if (moveViewHelpText != null)
        {
            moveViewHelpText.text = "Click an object to pick it up, then click again to place it.";
        }

        if (deleteViewHelpText != null)
        {
            deleteViewHelpText.text = "Click a placed object to remove it and refund one item.";
        }
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
