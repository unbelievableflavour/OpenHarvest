using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlacementSystem : MonoBehaviour, IPlacementToolHost
{
    public enum PlacementToolMode
    {
        Place,
        Delete,
        Move
    }

    private const string PlacementBlockerName = "__PlacementBlocker";
    private const int IgnoreRaycastLayer = 2;
    private static readonly int PlacementBlockerLayerMask = 1 << IgnoreRaycastLayer;

    [SerializeField]
    private GameObject mouseIndicator;
    
    [SerializeField]
    private ObjectsDatabaseSO database;

    private int selectedObjectIndex = -1;

    private List<GameObject> placedGameObjects = new List<GameObject>();

    [SerializeField]
    private PreviewSystem preview;

    [Header("Debug")]
    [SerializeField]
    private bool showDebugBoundingBoxes = false;

    [SerializeField]
    private Color candidateBoundsColor = new Color(0f, 0.8f, 1f, 0.8f);

    [SerializeField]
    private Color blockerBoundsColor = new Color(1f, 0.8f, 0f, 0.8f);

    [SerializeField]
    private Color overlappingCandidateBoundsColor = new Color(1f, 0f, 0f, 0.9f);

    [Header("Test Data")]
    [SerializeField]
    private bool loadTestOwnedItemsOnStart = false;

    [SerializeField]
    private int testOwnedItemCount = 10;

    private Vector3 lastDetectedPosition = Vector3.positiveInfinity;
    private const float PreviewMoveThreshold = 0.01f;
    private const float RotationStepDegrees = 15f;
    private const float WristRotationEpsilonDegrees = 0.5f;
    private float currentRotationY = 0f;
    private PlacementToolMode currentToolMode = PlacementToolMode.Place;
    private GameObject pendingMoveObject = null;
    private Vector3 pendingMoveOriginalPosition = Vector3.zero;
    private Quaternion pendingMoveOriginalRotation = Quaternion.identity;
    private bool hasPendingMoveLocalBounds = false;
    private Bounds pendingMoveLocalBounds = default;
    private bool hasLastXrPointerRotation = false;
    private Quaternion lastXrPointerRotation = Quaternion.identity;
    private bool placementPreviewDirty = false;
    private Collider currentPlacementTargetCollider = null;
    private readonly Dictionary<int, Bounds> cachedLocalBoundsByObjectId = new Dictionary<int, Bounds>();
    private bool hasDebugCandidateBounds = false;
    private bool debugCandidateOverlapping = false;
    private Vector3 debugCandidateCenter = Vector3.zero;
    private Vector3 debugCandidateHalfExtents = Vector3.one * 0.1f;
    private Quaternion debugCandidateRotation = Quaternion.identity;
    private PlacementToolMode activePreviewMode = (PlacementToolMode)(-1);
    private int activePreviewSourceId = int.MinValue;

    public event Action<bool> OnPlacementModeChanged;
    public event Action<string> OnPlacementSelectionChanged;
    public event Action OnPlacementInventoryChanged;
    public event Action<PlacementToolMode> OnPlacementToolChanged;
    private readonly Dictionary<PlacementToolMode, IPlacementToolController> toolControllers = new Dictionary<PlacementToolMode, IPlacementToolController>();

    private void Awake()
    {
        InitializeToolControllers();
    }

    void Start() {
        SeedTestOwnedItemsIfEnabled();
        StopPlacement();
        OnToggleMode();
    }

    private void InitializeToolControllers()
    {
        toolControllers[PlacementToolMode.Place] = new PlaceToolController(this);
        toolControllers[PlacementToolMode.Delete] = new DeleteToolController(this);
        toolControllers[PlacementToolMode.Move] = new MoveToolController(this);
    }

    private IPlacementToolController GetActiveToolController()
    {
        if (toolControllers.TryGetValue(currentToolMode, out var controller))
        {
            return controller;
        }

        return null;
    }

    private void SeedTestOwnedItemsIfEnabled()
    {
        if (!loadTestOwnedItemsOnStart || database == null || database.objectsData == null)
        {
            return;
        }

        if (GameState.Instance == null || GameState.Instance.unlockables == null)
        {
            return;
        }

        int seededCount = 0;
        int amount = Mathf.Max(0, testOwnedItemCount);
        for (int i = 0; i < database.objectsData.Count; i++)
        {
            ObjectData objectData = database.objectsData[i];
            string lookupName = GetPrimaryUnlockableLookupKey(objectData);
            if (string.IsNullOrWhiteSpace(lookupName))
            {
                continue;
            }

            // Only seed missing/empty entries so real saved amounts are preserved.
            if (!GameState.Instance.unlockables.TryGetValue(lookupName, out int existingCount) || existingCount <= 0)
            {
                GameState.Instance.unlockables[lookupName] = amount;
                seededCount++;
            }
        }

        Debug.Log("[PlacementSystem] Seeded test ownership for " + seededCount + " placeable items with x" + amount + ".");
    }

    public void OnEnable() {
        GameState.Instance.OnToggleMode += OnToggleMode;
    }

    public void OnDisable() {
        GameState.Instance.OnToggleMode -= OnToggleMode;
    }

    void OnToggleMode() {
        if(GameState.Instance.GetMode() == "build") {
            StartPlacement(0);
        } else {
            StopPlacement();
        }
    }

    private void Update(){
        if(selectedObjectIndex == -1) {
            return;
        }

        HandlePlacementRotationInput();
        GetActiveToolController()?.UpdateTool();
    }

    private void StartPlacementByIndex(int index)
    {
        StopPlacement();
        selectedObjectIndex = index;
        if (selectedObjectIndex < 0 || selectedObjectIndex >= database.objectsData.Count)
        {
            Debug.LogWarning("[PlacementSystem] Selected placeable index is out of range: " + selectedObjectIndex);
            return;
        }

        var currentObject = database.objectsData[selectedObjectIndex];

        if(currentObject == null) {
            Debug.Log("selectObjectIndex not set");
            return;
        }

        preview.StartShowingPlacementPreview(currentObject.prefab);
        SetPlacementToolMode(PlacementToolMode.Place);
        if (mouseIndicator != null)
        {
            mouseIndicator.SetActive(true);
        }
        OnPlacementModeChanged?.Invoke(true);
        NotifyPlacementSelectionChanged();

        if (HarvestInputManager.Instance != null)
        {
            HarvestInputManager.Instance.OnTriggerRight += PlaceStructure;
            HarvestInputManager.Instance.OnBButton += OnPreviousItem;
            HarvestInputManager.Instance.OnAButton += OnNextItem;
        }
    }

    private void StartPlacement(int objectId)
    {
        int resolvedIndex = ResolveObjectIndex(objectId);
        if (resolvedIndex == -1)
        {
            Debug.LogWarning("[PlacementSystem] Unable to resolve placeable object: " + objectId);
            return;
        }
        StartPlacementByIndex(resolvedIndex);
    }

    private void PlaceStructure() {
        if (IsUiInteractionActive())
        {
            return;
        }

        GetActiveToolController()?.HandlePrimaryAction();
    }

    private bool TryGetToolTargetHit(out RaycastHit hit)
    {
        hit = default;
        if (HarvestInputManager.Instance == null || !HarvestInputManager.Instance.TryGetPointerRay(out var pointerRay))
        {
            return false;
        }

        return Physics.Raycast(pointerRay, out hit, 100f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
    }

    private void HandlePlaceAction(Vector3 mousePosition)
    {
        if (GetPlacementItemOwnedCountAt(selectedObjectIndex) <= 0)
        {
            return;
        }

        int selectedObjectId = database.objectsData[selectedObjectIndex].ID;
        bool placementValidity = CheckPlacementValidity(mousePosition, selectedObjectId);
        if (!placementValidity)
        {
            return;
        }

        PlaceStructureWithoutPreview(mousePosition, selectedObjectId);
        ConsumeSelectedPlacementItem();
        OnPlacementInventoryChanged?.Invoke();
        NotifyPlacementSelectionChanged();
        preview.UpdatePosition(mousePosition, true, Quaternion.Euler(0f, currentRotationY, 0f));
    }

    private void HandleDeleteAction(Collider hitCollider)
    {
        if (!TryGetPlacedObjectRoot(hitCollider, out var placedRoot))
        {
            return;
        }

        DeletePlacedObject(placedRoot, true);
        OnPlacementInventoryChanged?.Invoke();
        NotifyPlacementSelectionChanged();
    }

    private void HandleMoveAction(Vector3 mousePosition, Collider hitCollider)
    {
        if (pendingMoveObject == null)
        {
            if (!TryGetPlacedObjectRoot(hitCollider, out var placedRoot))
            {
                return;
            }

            pendingMoveObject = placedRoot;
            pendingMoveOriginalPosition = placedRoot.transform.position;
            pendingMoveOriginalRotation = placedRoot.transform.rotation;
            hasPendingMoveLocalBounds = TryGetLocalBoundsForPlacement(placedRoot.transform, out pendingMoveLocalBounds);
            EnsurePreviewForCurrentToolMode();
            pendingMoveObject.SetActive(false);
            return;
        }

        Quaternion moveRotation = Quaternion.Euler(0f, currentRotationY, 0f);
        if (!CheckPlacementValidityForExistingObject(pendingMoveObject, mousePosition, moveRotation))
        {
            return;
        }

        pendingMoveObject.transform.position = mousePosition;
        pendingMoveObject.transform.rotation = moveRotation;
        pendingMoveObject.SetActive(true);
        RegisterPlacedObjectIfMissing(pendingMoveObject);
        ClearPendingMoveState();
        EnsurePreviewForCurrentToolMode();
    }

    private void StopPlacement() {
        CancelPendingMove(restoreOriginal: true);
        bool wasActive = selectedObjectIndex >= 0;
        selectedObjectIndex = -1;
        hasLastXrPointerRotation = false;
        preview.StopShowingPreview();
        if (mouseIndicator != null)
        {
            mouseIndicator.SetActive(false);
        }
        if (HarvestInputManager.Instance != null)
        {
            HarvestInputManager.Instance.OnTriggerRight -= PlaceStructure;
            HarvestInputManager.Instance.OnBButton -= OnPreviousItem;
            HarvestInputManager.Instance.OnAButton -= OnNextItem;
        }

        lastDetectedPosition = Vector3.positiveInfinity;
        if (wasActive)
        {
            OnPlacementModeChanged?.Invoke(false);
            OnPlacementSelectionChanged?.Invoke(string.Empty);
        }
    }

    public void PlaceStructureWithoutPreview(Vector3 worldPosition, int objectId)
    {
        int resolvedIndex = ResolveObjectIndex(objectId);
        if (resolvedIndex == -1)
        {
            Debug.LogWarning("[PlacementSystem] Unable to place object with id/index: " + objectId);
            return;
        }

        GameObject newObject = Instantiate(database.objectsData[resolvedIndex].prefab);
        newObject.transform.position = worldPosition;
        newObject.transform.rotation = Quaternion.Euler(0f, currentRotationY, 0f);
        EnsurePlacementBlocker(newObject);
        placedGameObjects.Add(newObject);
    }

    public bool CheckPlacementValidity(Vector3 worldPosition, int objectId)
    {
        int resolvedIndex = ResolveObjectIndex(objectId);
        if (resolvedIndex == -1)
        {
            return false;
        }

        if (GetPlacementItemOwnedCountAt(resolvedIndex) <= 0)
        {
            return false;
        }

        var objectData = database.objectsData[resolvedIndex];
        if (objectData == null || objectData.prefab == null)
        {
            return false;
        }

        if (IsHitPlacedObject(currentPlacementTargetCollider))
        {
            return false;
        }

        Quaternion placementRotation = Quaternion.Euler(0f, currentRotationY, 0f);
        if (WouldOverlapPlacedObjects(objectData, worldPosition, placementRotation))
        {
            return false;
        }

        return true;
    }

    private bool CheckPlacementValidityForExistingObject(GameObject existingObject, Vector3 worldPosition, Quaternion worldRotation)
    {
        if (existingObject == null)
        {
            return false;
        }

        if (IsHitPlacedObject(currentPlacementTargetCollider))
        {
            return false;
        }

        Bounds localBounds;
        if (existingObject == pendingMoveObject && hasPendingMoveLocalBounds)
        {
            localBounds = pendingMoveLocalBounds;
        }
        else if (!TryGetLocalBoundsForPlacement(existingObject.transform, out localBounds))
        {
            return false;
        }

        if (existingObject == pendingMoveObject)
        {
            pendingMoveLocalBounds = localBounds;
            hasPendingMoveLocalBounds = true;
        }

        return !WouldOverlapPlacedObjects(worldPosition, worldRotation, localBounds);
    }

    private int ResolveObjectIndex(int objectIdOrIndex)
    {
        int idMatch = database.objectsData.FindIndex(data => data.ID == objectIdOrIndex);
        if (idMatch >= 0)
        {
            return idMatch;
        }

        if (objectIdOrIndex >= 0 && objectIdOrIndex < database.objectsData.Count)
        {
            return objectIdOrIndex;
        }

        return -1;
    }

    private bool TryGetPlacementPoint(out Vector3 position, out Collider hitCollider)
    {
        position = Vector3.zero;
        hitCollider = null;

        // Unified aiming source is handled by HarvestInputManager:
        // - FPS: center-camera ray
        // - XR/VR: controller pointer ray
        if (HarvestInputManager.Instance != null &&
            HarvestInputManager.Instance.TryGetSelectedMapHit(out var mappedHit))
        {
            position = mappedHit.point;
            hitCollider = mappedHit.collider;
            return true;
        }

        return false;
    }

    private void EnsurePlacementBlocker(GameObject placedObject)
    {
        if (placedObject == null)
        {
            return;
        }

        Transform existing = placedObject.transform.Find(PlacementBlockerName);
        if (existing != null)
        {
            existing.gameObject.layer = IgnoreRaycastLayer;
            return;
        }

        if (!TryGetLocalBoundsForPlacement(placedObject.transform, out var localBounds))
        {
            return;
        }

        GameObject blocker = new GameObject(PlacementBlockerName);
        blocker.transform.SetParent(placedObject.transform, false);
        blocker.layer = IgnoreRaycastLayer;
        blocker.AddComponent<PlacementBlockerMarker>();

        BoxCollider box = blocker.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.center = localBounds.center;
        box.size = localBounds.size;
    }

    private bool TryGetLocalBoundsForPlacement(Transform root, out Bounds localBounds)
    {
        localBounds = default;
        if (root == null)
        {
            return false;
        }

        bool hasBounds = false;
        Matrix4x4 rootWorldToLocal = root.worldToLocalMatrix;

        MeshFilter[] meshFilters = root.GetComponentsInChildren<MeshFilter>(true);
        foreach (var meshFilter in meshFilters)
        {
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                continue;
            }

            Matrix4x4 meshToRoot = rootWorldToLocal * meshFilter.transform.localToWorldMatrix;
            EncapsulateTransformedBounds(meshFilter.sharedMesh.bounds, meshToRoot, ref hasBounds, ref localBounds);
        }

        SkinnedMeshRenderer[] skinnedMeshes = root.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        foreach (var skinnedMesh in skinnedMeshes)
        {
            if (skinnedMesh == null)
            {
                continue;
            }

            Matrix4x4 meshToRoot = rootWorldToLocal * skinnedMesh.transform.localToWorldMatrix;
            EncapsulateTransformedBounds(skinnedMesh.localBounds, meshToRoot, ref hasBounds, ref localBounds);
        }

        Collider[] colliders = root.GetComponentsInChildren<Collider>(true);
        foreach (var collider in colliders)
        {
            if (collider == null || collider.isTrigger)
            {
                continue;
            }

            if (collider is BoxCollider boxCollider)
            {
                Matrix4x4 colliderToRoot = rootWorldToLocal * boxCollider.transform.localToWorldMatrix;
                Bounds boxLocalBounds = new Bounds(boxCollider.center, boxCollider.size);
                EncapsulateTransformedBounds(boxLocalBounds, colliderToRoot, ref hasBounds, ref localBounds);
            }
            else if (collider is SphereCollider sphereCollider)
            {
                Matrix4x4 colliderToRoot = rootWorldToLocal * sphereCollider.transform.localToWorldMatrix;
                float diameter = sphereCollider.radius * 2f;
                Bounds sphereLocalBounds = new Bounds(sphereCollider.center, new Vector3(diameter, diameter, diameter));
                EncapsulateTransformedBounds(sphereLocalBounds, colliderToRoot, ref hasBounds, ref localBounds);
            }
            else if (collider is CapsuleCollider capsuleCollider)
            {
                Matrix4x4 colliderToRoot = rootWorldToLocal * capsuleCollider.transform.localToWorldMatrix;
                float diameter = capsuleCollider.radius * 2f;
                Vector3 size = new Vector3(diameter, diameter, diameter);
                int direction = capsuleCollider.direction;
                size[direction] = Mathf.Max(capsuleCollider.height, diameter);
                Bounds capsuleLocalBounds = new Bounds(capsuleCollider.center, size);
                EncapsulateTransformedBounds(capsuleLocalBounds, colliderToRoot, ref hasBounds, ref localBounds);
            }
            else if (collider is MeshCollider meshCollider && meshCollider.sharedMesh != null)
            {
                Matrix4x4 colliderToRoot = rootWorldToLocal * meshCollider.transform.localToWorldMatrix;
                EncapsulateTransformedBounds(meshCollider.sharedMesh.bounds, colliderToRoot, ref hasBounds, ref localBounds);
            }
        }

        return hasBounds;
    }

    private void EncapsulateTransformedBounds(Bounds sourceBounds, Matrix4x4 transformMatrix, ref bool hasBounds, ref Bounds targetBounds)
    {
        Vector3 min = sourceBounds.min;
        Vector3 max = sourceBounds.max;

        Vector3[] corners = new Vector3[8];
        corners[0] = new Vector3(min.x, min.y, min.z);
        corners[1] = new Vector3(max.x, min.y, min.z);
        corners[2] = new Vector3(min.x, max.y, min.z);
        corners[3] = new Vector3(max.x, max.y, min.z);
        corners[4] = new Vector3(min.x, min.y, max.z);
        corners[5] = new Vector3(max.x, min.y, max.z);
        corners[6] = new Vector3(min.x, max.y, max.z);
        corners[7] = new Vector3(max.x, max.y, max.z);

        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 transformedCorner = transformMatrix.MultiplyPoint3x4(corners[i]);

            if (!hasBounds)
            {
                targetBounds = new Bounds(transformedCorner, Vector3.zero);
                hasBounds = true;
            }
            else
            {
                targetBounds.Encapsulate(transformedCorner);
            }
        }
    }

    private bool WouldOverlapPlacedObjects(ObjectData objectData, Vector3 worldPosition, Quaternion worldRotation)
    {
        if (!TryGetLocalBoundsForObject(objectData, out var localBounds))
        {
            hasDebugCandidateBounds = false;
            return false;
        }

        return WouldOverlapPlacedObjects(worldPosition, worldRotation, localBounds);
    }

    private bool WouldOverlapPlacedObjects(Vector3 worldPosition, Quaternion worldRotation, Bounds localBounds)
    {
        Vector3 worldCenter = worldPosition + (worldRotation * localBounds.center);
        Vector3 halfExtents = localBounds.extents;
        halfExtents = new Vector3(
            Mathf.Max(halfExtents.x, 0.01f),
            Mathf.Max(halfExtents.y, 0.01f),
            Mathf.Max(halfExtents.z, 0.01f)
        );

        hasDebugCandidateBounds = true;
        debugCandidateCenter = worldCenter;
        debugCandidateHalfExtents = halfExtents;
        debugCandidateRotation = worldRotation;
        debugCandidateOverlapping = false;

        Collider[] hits = Physics.OverlapBox(
            worldCenter,
            halfExtents,
            worldRotation,
            PlacementBlockerLayerMask,
            QueryTriggerInteraction.Collide
        );

        foreach (var hit in hits)
        {
            if (hit == null)
            {
                continue;
            }

            if (hit.GetComponent<PlacementBlockerMarker>() == null)
            {
                continue;
            }

            debugCandidateOverlapping = true;
            return true;
        }

        return false;
    }

    private bool TryGetLocalBoundsForObject(ObjectData objectData, out Bounds localBounds)
    {
        localBounds = default;
        if (objectData == null || objectData.prefab == null)
        {
            return false;
        }

        if (cachedLocalBoundsByObjectId.TryGetValue(objectData.ID, out localBounds))
        {
            return true;
        }

        if (!TryGetLocalBoundsForPlacement(objectData.prefab.transform, out localBounds))
        {
            return false;
        }

        cachedLocalBoundsByObjectId[objectData.ID] = localBounds;
        return true;
    }

    private bool IsFpsInputMode()
    {
        if (HarvestInputManager.Instance == null || HarvestInputManager.Instance.harvestSettings == null)
        {
            return Mouse.current != null;
        }

        return HarvestInputManager.Instance.harvestSettings.playerMode == PlayerMode.FPS;
    }

    private bool IsHitPlacedObject(Collider hitCollider)
    {
        if (hitCollider == null)
        {
            return false;
        }

        // Placement blockers are trigger-only overlap helpers and should not
        // invalidate a target hit when aiming near an existing placed object.
        if (hitCollider.GetComponent<PlacementBlockerMarker>() != null)
        {
            return false;
        }

        foreach (var placed in placedGameObjects)
        {
            if (placed == null)
            {
                continue;
            }

            if (hitCollider.transform.IsChildOf(placed.transform))
            {
                return true;
            }
        }

        return false;
    }

    private void HandlePlacementRotationInput()
    {
        // Wrist rotation should always be considered when a pointer exists.
        // This keeps XR behavior working even if player mode config is wrong.
        HandleXrWristRotationInput();
        HandleFpsPlacementInput();
    }

    private void HandleXrWristRotationInput()
    {
        if (HarvestInputManager.Instance == null ||
            !HarvestInputManager.Instance.TryGetPointerRotation(out var pointerRotation))
        {
            return;
        }

        if (!hasLastXrPointerRotation)
        {
            hasLastXrPointerRotation = true;
            lastXrPointerRotation = pointerRotation;
            return;
        }

        Vector3 previousUp = lastXrPointerRotation * Vector3.up;
        Vector3 currentUp = pointerRotation * Vector3.up;
        Vector3 twistAxis = pointerRotation * Vector3.forward;
        float wristTwistDelta = Vector3.SignedAngle(previousUp, currentUp, twistAxis);
        lastXrPointerRotation = pointerRotation;

        if (Mathf.Abs(wristTwistDelta) < WristRotationEpsilonDegrees)
        {
            return;
        }

        currentRotationY = Mathf.Repeat(currentRotationY + wristTwistDelta, 360f);
        placementPreviewDirty = true;
    }

    private void HandleFpsPlacementInput()
    {
        if (!IsFpsInputMode() || Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            OnPreviousItem();
            return;
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            OnNextItem();
            return;
        }

        if (Mouse.current == null)
        {
            return;
        }

        float scrollY = Mouse.current.scroll.ReadValue().y;
        if (scrollY > 0.01f)
        {
            currentRotationY = Mathf.Repeat(currentRotationY + RotationStepDegrees, 360f);
            placementPreviewDirty = true;
        }
        else if (scrollY < -0.01f)
        {
            currentRotationY = Mathf.Repeat(currentRotationY - RotationStepDegrees, 360f);
            placementPreviewDirty = true;
        }
    }

    private void OnPreviousItem () {
        if (database.objectsData.Count == 0 || selectedObjectIndex < 0)
        {
            return;
        }

        int previousIndex = selectedObjectIndex > 0 ? selectedObjectIndex - 1 : database.objectsData.Count - 1;
        StartPlacementByIndex(previousIndex);
    }

    private void OnNextItem () {
        if (database.objectsData.Count == 0 || selectedObjectIndex < 0)
        {
            return;
        }

        int nextIndex = selectedObjectIndex < database.objectsData.Count - 1 ? selectedObjectIndex + 1 : 0;
        StartPlacementByIndex(nextIndex);
    }

    public bool IsPlacementModeActive()
    {
        return selectedObjectIndex >= 0;
    }

    public void SelectPreviousItem()
    {
        OnPreviousItem();
    }

    public void SelectNextItem()
    {
        OnNextItem();
    }

    public void SelectItemByIndex(int index)
    {
        StartPlacementByIndex(index);
    }

    public int GetPlacementItemCount()
    {
        if (database == null || database.objectsData == null)
        {
            return 0;
        }

        return database.objectsData.Count;
    }

    public string GetPlacementItemNameAt(int index)
    {
        if (database == null || database.objectsData == null || index < 0 || index >= database.objectsData.Count)
        {
            return string.Empty;
        }

        var objectData = database.objectsData[index];
        if (objectData == null)
        {
            return string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(objectData.name))
        {
            return objectData.name;
        }

        return objectData.prefab != null ? objectData.prefab.name : string.Empty;
    }

    public int GetCurrentPlacementItemIndex()
    {
        return selectedObjectIndex;
    }

    public int GetPlacementItemOwnedCountAt(int index)
    {
        if (database == null || database.objectsData == null || index < 0 || index >= database.objectsData.Count)
        {
            return 0;
        }

        if (GameState.Instance == null || GameState.Instance.unlockables == null)
        {
            return 0;
        }

        var objectData = database.objectsData[index];
        if (objectData == null)
        {
            return 0;
        }

        List<string> lookupKeys = GetUnlockableLookupKeys(objectData);
        if (lookupKeys.Count == 0)
        {
            return 0;
        }

        int ownedCount = 0;
        for (int i = 0; i < lookupKeys.Count; i++)
        {
            string key = lookupKeys[i];
            if (GameState.Instance.unlockables.TryGetValue(key, out int exactCount) && exactCount > 0)
            {
                ownedCount += exactCount;
            }
        }

        return ownedCount;
    }

    private void ConsumeSelectedPlacementItem()
    {
        if (selectedObjectIndex < 0 ||
            database == null ||
            database.objectsData == null ||
            selectedObjectIndex >= database.objectsData.Count ||
            GameState.Instance == null ||
            GameState.Instance.unlockables == null)
        {
            return;
        }

        ObjectData selectedObject = database.objectsData[selectedObjectIndex];
        List<string> lookupKeys = GetUnlockableLookupKeys(selectedObject);
        if (lookupKeys.Count == 0)
        {
            return;
        }

        for (int i = 0; i < lookupKeys.Count; i++)
        {
            if (TryConsumeUnlockableCount(lookupKeys[i]))
            {
                return;
            }
        }
    }

    private bool TryGetPlacedObjectRoot(Collider hitCollider, out GameObject rootObject)
    {
        rootObject = null;
        if (hitCollider == null)
        {
            return false;
        }

        PlacementBlockerMarker blockerMarker = hitCollider.GetComponentInParent<PlacementBlockerMarker>();
        if (blockerMarker != null && blockerMarker.transform.parent != null)
        {
            rootObject = blockerMarker.transform.parent.gameObject;
            return true;
        }

        Transform current = hitCollider.transform;
        while (current != null)
        {
            if (current.Find(PlacementBlockerName) != null)
            {
                rootObject = current.gameObject;
                return true;
            }
            current = current.parent;
        }

        for (int i = 0; i < placedGameObjects.Count; i++)
        {
            GameObject placed = placedGameObjects[i];
            if (placed == null)
            {
                continue;
            }

            if (hitCollider.transform.IsChildOf(placed.transform))
            {
                rootObject = placed;
                return true;
            }
        }

        return false;
    }

    private void RegisterPlacedObjectIfMissing(GameObject placedObject)
    {
        if (placedObject == null)
        {
            return;
        }

        if (!placedGameObjects.Contains(placedObject))
        {
            placedGameObjects.Add(placedObject);
        }
    }

    private void DeletePlacedObject(GameObject placedObject, bool refundToInventory)
    {
        if (placedObject == null)
        {
            return;
        }

        if (refundToInventory)
        {
            TryRefundPlacedObject(placedObject);
        }

        placedGameObjects.Remove(placedObject);
        Destroy(placedObject);
    }

    private void TryRefundPlacedObject(GameObject placedObject)
    {
        if (placedObject == null || GameState.Instance == null || GameState.Instance.unlockables == null)
        {
            return;
        }

        string itemKey = ResolvePlacedObjectItemKey(placedObject);
        if (string.IsNullOrWhiteSpace(itemKey))
        {
            return;
        }

        if (GameState.Instance.unlockables.TryGetValue(itemKey, out int currentCount))
        {
            GameState.Instance.unlockables[itemKey] = currentCount + 1;
        }
        else
        {
            GameState.Instance.unlockables[itemKey] = 1;
        }
    }

    private string ResolvePlacedObjectItemKey(GameObject placedObject)
    {
        if (placedObject == null)
        {
            return string.Empty;
        }

        string cleanName = placedObject.name.Replace("(Clone)", string.Empty).Trim();
        if (GameState.Instance != null && GameState.Instance.unlockables != null &&
            GameState.Instance.unlockables.ContainsKey(cleanName))
        {
            return cleanName;
        }

        if (database != null && database.objectsData != null)
        {
            for (int i = 0; i < database.objectsData.Count; i++)
            {
                ObjectData objectData = database.objectsData[i];
                if (objectData == null)
                {
                    continue;
                }

                string objectName = objectData.name;
                string prefabName = objectData.prefab != null ? objectData.prefab.name : string.Empty;
                if (string.Equals(objectName, cleanName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(prefabName, cleanName, StringComparison.OrdinalIgnoreCase))
                {
                    return GetPrimaryUnlockableLookupKey(objectData);
                }
            }
        }

        if (GameState.Instance != null && GameState.Instance.unlockables != null)
        {
            foreach (var entry in GameState.Instance.unlockables)
            {
                if (string.IsNullOrWhiteSpace(entry.Key))
                {
                    continue;
                }

                if (string.Equals(entry.Key, cleanName, StringComparison.OrdinalIgnoreCase))
                {
                    return entry.Key;
                }
            }
        }

        return cleanName;
    }

    private void CancelPendingMove(bool restoreOriginal)
    {
        if (pendingMoveObject == null)
        {
            return;
        }

        if (restoreOriginal)
        {
            pendingMoveObject.transform.position = pendingMoveOriginalPosition;
            pendingMoveObject.transform.rotation = pendingMoveOriginalRotation;
        }

        pendingMoveObject.SetActive(true);
        RegisterPlacedObjectIfMissing(pendingMoveObject);
        ClearPendingMoveState();
    }

    private void ClearPendingMoveState()
    {
        pendingMoveObject = null;
        hasPendingMoveLocalBounds = false;
        pendingMoveLocalBounds = default;
    }

    private static bool TryConsumeUnlockableCount(string key)
    {
        if (GameState.Instance == null || GameState.Instance.unlockables == null || string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        if (!GameState.Instance.unlockables.TryGetValue(key, out int currentCount) || currentCount <= 0)
        {
            return false;
        }

        GameState.Instance.unlockables[key] = currentCount - 1;
        return true;
    }

    private static List<string> GetUnlockableLookupKeys(ObjectData objectData)
    {
        List<string> lookupKeys = new List<string>();
        if (objectData == null)
        {
            return lookupKeys;
        }

        if (objectData.unlockableIds != null)
        {
            for (int i = 0; i < objectData.unlockableIds.Count; i++)
            {
                AddLookupKeyIfMissing(lookupKeys, objectData.unlockableIds[i]);
            }
        }

        AddLookupKeyIfMissing(lookupKeys, objectData.name);
        if (objectData.prefab != null)
        {
            AddLookupKeyIfMissing(lookupKeys, objectData.prefab.name);
        }

        return lookupKeys;
    }

    private static string GetPrimaryUnlockableLookupKey(ObjectData objectData)
    {
        List<string> lookupKeys = GetUnlockableLookupKeys(objectData);
        return lookupKeys.Count > 0 ? lookupKeys[0] : string.Empty;
    }

    private static void AddLookupKeyIfMissing(List<string> keys, string key)
    {
        if (keys == null || string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        if (!keys.Contains(key))
        {
            keys.Add(key);
        }
    }

    private bool IsUiInteractionActive()
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }

        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null)
        {
            return false;
        }

        return selected.GetComponentInParent<Canvas>() != null;
    }

    public string GetCurrentPlacementItemName()
    {
        if (database == null || database.objectsData == null || selectedObjectIndex < 0 || selectedObjectIndex >= database.objectsData.Count)
        {
            return string.Empty;
        }

        var objectData = database.objectsData[selectedObjectIndex];
        if (objectData == null)
        {
            return string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(objectData.name))
        {
            return objectData.name;
        }

        return objectData.prefab != null ? objectData.prefab.name : string.Empty;
    }

    private void NotifyPlacementSelectionChanged()
    {
        OnPlacementSelectionChanged?.Invoke(GetCurrentPlacementItemName());
    }

    public void SetPlacementToolPlace()
    {
        CancelPendingMove(restoreOriginal: true);
        SetPlacementToolMode(PlacementToolMode.Place);
    }

    public void SetPlacementToolDelete()
    {
        CancelPendingMove(restoreOriginal: true);
        SetPlacementToolMode(PlacementToolMode.Delete);
    }

    public void SetPlacementToolMove()
    {
        CancelPendingMove(restoreOriginal: true);
        SetPlacementToolMode(PlacementToolMode.Move);
    }

    public PlacementToolMode GetCurrentPlacementToolMode()
    {
        return currentToolMode;
    }

    private void SetPlacementToolMode(PlacementToolMode mode)
    {
        currentToolMode = mode;
        EnsurePreviewForCurrentToolMode();
        OnPlacementToolChanged?.Invoke(currentToolMode);
    }

    private void EnsurePreviewForCurrentToolMode()
    {
        if (currentToolMode == PlacementToolMode.Delete)
        {
            if (activePreviewMode != PlacementToolMode.Delete || activePreviewSourceId != -1)
            {
                preview.StopShowingPreview();
                activePreviewMode = PlacementToolMode.Delete;
                activePreviewSourceId = -1;
            }

            placementPreviewDirty = false;
            return;
        }

        if (currentToolMode == PlacementToolMode.Move)
        {
            if (pendingMoveObject == null)
            {
                if (activePreviewMode != PlacementToolMode.Move || activePreviewSourceId != -1)
                {
                    preview.StopShowingPreview();
                    activePreviewMode = PlacementToolMode.Move;
                    activePreviewSourceId = -1;
                }

                placementPreviewDirty = false;
                return;
            }

            int movePreviewId = pendingMoveObject.GetInstanceID();
            if (activePreviewMode == PlacementToolMode.Move && activePreviewSourceId == movePreviewId)
            {
                return;
            }

            preview.StartShowingPlacementPreview(pendingMoveObject);
            activePreviewMode = PlacementToolMode.Move;
            activePreviewSourceId = movePreviewId;
            placementPreviewDirty = true;
            return;
        }

        if (database == null ||
            database.objectsData == null ||
            selectedObjectIndex < 0 ||
            selectedObjectIndex >= database.objectsData.Count)
        {
            return;
        }

        var currentObject = database.objectsData[selectedObjectIndex];
        if (currentObject == null)
        {
            return;
        }

        if (activePreviewMode == PlacementToolMode.Place && activePreviewSourceId == selectedObjectIndex)
        {
            return;
        }

        preview.StartShowingPlacementPreview(currentObject.prefab);
        activePreviewMode = PlacementToolMode.Place;
        activePreviewSourceId = selectedObjectIndex;
        placementPreviewDirty = true;
    }

    bool IPlacementToolHost.TryGetPlacementPoint(out Vector3 position, out Collider hitCollider)
    {
        return TryGetPlacementPoint(out position, out hitCollider);
    }

    bool IPlacementToolHost.TryGetToolTargetHit(out RaycastHit hit)
    {
        return TryGetToolTargetHit(out hit);
    }

    void IPlacementToolHost.SetCurrentPlacementTargetCollider(Collider hitCollider)
    {
        currentPlacementTargetCollider = hitCollider;
    }

    bool IPlacementToolHost.ShouldRefreshPlacePreview(Vector3 position)
    {
        bool positionChanged = Vector3.Distance(lastDetectedPosition, position) >= PreviewMoveThreshold;
        if (!positionChanged && !placementPreviewDirty)
        {
            return false;
        }

        lastDetectedPosition = position;
        placementPreviewDirty = false;
        return true;
    }

    int IPlacementToolHost.GetSelectedPlacementObjectId()
    {
        if (database == null ||
            database.objectsData == null ||
            selectedObjectIndex < 0 ||
            selectedObjectIndex >= database.objectsData.Count)
        {
            return -1;
        }

        return database.objectsData[selectedObjectIndex].ID;
    }

    bool IPlacementToolHost.CheckPlacementValidity(Vector3 worldPosition, int objectId)
    {
        return CheckPlacementValidity(worldPosition, objectId);
    }

    Quaternion IPlacementToolHost.GetCurrentPlacementRotation()
    {
        return Quaternion.Euler(0f, currentRotationY, 0f);
    }

    void IPlacementToolHost.SetMouseIndicatorPosition(Vector3 worldPosition)
    {
        if (mouseIndicator != null)
        {
            mouseIndicator.transform.position = worldPosition;
        }
    }

    void IPlacementToolHost.UpdatePreviewPosition(Vector3 worldPosition, bool isValid, Quaternion rotation)
    {
        preview.UpdatePosition(worldPosition, isValid, rotation);
    }

    void IPlacementToolHost.EnsurePreviewForCurrentToolMode()
    {
        EnsurePreviewForCurrentToolMode();
    }

    bool IPlacementToolHost.HasPendingMoveObject()
    {
        return pendingMoveObject != null;
    }

    bool IPlacementToolHost.CheckPendingMovePlacementValidity(Vector3 worldPosition, Quaternion rotation)
    {
        return CheckPlacementValidityForExistingObject(pendingMoveObject, worldPosition, rotation);
    }

    void IPlacementToolHost.MarkPlacementPreviewClean()
    {
        placementPreviewDirty = false;
    }

    void IPlacementToolHost.HandlePlaceAction(Vector3 worldPosition)
    {
        HandlePlaceAction(worldPosition);
    }

    void IPlacementToolHost.HandleDeleteAction(Collider hitCollider)
    {
        HandleDeleteAction(hitCollider);
    }

    void IPlacementToolHost.HandleMoveAction(Vector3 worldPosition, Collider hitCollider)
    {
        HandleMoveAction(worldPosition, hitCollider);
    }

    private void OnDrawGizmos()
    {
        if (!showDebugBoundingBoxes)
        {
            return;
        }

        // Draw current candidate overlap box.
        if (hasDebugCandidateBounds)
        {
            Gizmos.color = debugCandidateOverlapping ? overlappingCandidateBoundsColor : candidateBoundsColor;
            Matrix4x4 previousMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(debugCandidateCenter, debugCandidateRotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, debugCandidateHalfExtents * 2f);
            Gizmos.matrix = previousMatrix;
        }

        // Draw all placed blocker colliders.
        var blockers = FindObjectsByType<PlacementBlockerMarker>(FindObjectsSortMode.None);
        Gizmos.color = blockerBoundsColor;
        for (int i = 0; i < blockers.Length; i++)
        {
            var blocker = blockers[i];
            if (blocker == null)
            {
                continue;
            }

            var box = blocker.GetComponent<BoxCollider>();
            if (box == null)
            {
                continue;
            }

            Matrix4x4 previousMatrix = Gizmos.matrix;
            Gizmos.matrix = box.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(box.center, box.size);
            Gizmos.matrix = previousMatrix;
        }
    }
}

public class PlacementBlockerMarker : MonoBehaviour {}