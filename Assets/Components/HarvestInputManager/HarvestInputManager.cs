using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using BNG;

public class HarvestInputManager : MonoBehaviour
{
	public static HarvestInputManager Instance = null;
    public HarvestSettings harvestSettings;

    [SerializeField]
    private Transform pointer;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;
    
    [SerializeField]
    private LineRenderer pointerRayRenderer;

    [SerializeField]
    private float pointerRayLength = 100f;
    private int cachedSelectedMapHitFrame = -1;
    private bool hasCachedSelectedMapHit = false;
    private RaycastHit cachedSelectedMapHit;

    public event Action OnTriggerRight, OnBButton, OnAButton, OnMenuButton;
        
    public List<ControllerBinding> triggerRight = new List<ControllerBinding>() { ControllerBinding.None };
    public List<ControllerBinding> BButton = new List<ControllerBinding>() { ControllerBinding.None };
    public List<ControllerBinding> AButton = new List<ControllerBinding>() { ControllerBinding.None };
    public List<ControllerBinding> menuButton = new List<ControllerBinding>() { ControllerBinding.None };

    // Initialize instance.
    private void Awake()
    {
        if (Instance != this)
        {
            Instance = this;
        }

        if (pointerRayRenderer != null)
        {
            pointerRayRenderer.useWorldSpace = true;
        }
    }

    private void Update() {        
        UpdatePointerRayRenderer();

        // vr
        if(this.KeyDown(triggerRight)){
            OnTriggerRight?.Invoke();
        }

        if(this.KeyDown(BButton)){
            OnBButton?.Invoke();
        }

        if(this.KeyDown(AButton)){
            OnAButton?.Invoke();
        }

        if(this.KeyDown(menuButton)){
            OnMenuButton?.Invoke();
        }

#if UNITY_EDITOR
        if (harvestSettings.playerMode == PlayerMode.FPS)
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame){
                OnTriggerRight?.Invoke();
            }

            if (Keyboard.current != null && Keyboard.current.leftArrowKey.wasPressedThisFrame){
                OnBButton?.Invoke();
            }

            if (Keyboard.current != null && Keyboard.current.rightArrowKey.wasPressedThisFrame){
                OnAButton?.Invoke();
            }
        }
#endif
    }

    public virtual bool KeyDown(List<ControllerBinding> input)
    {
        // Check for bound controller button
        for (int x = 0; x < input.Count; x++)
        {
            if (InputBridge.Instance.GetControllerBindingValue(input[x]))
            {
                return true;
            }
        }

        return false;
    }

    public Vector3 GetSelectedMapPosition()
    {
        if (TryGetSelectedMapHit(out var hit))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }

    public bool TryGetSelectedMapHit(out RaycastHit hit)
    {
        int currentFrame = Time.frameCount;
        if (cachedSelectedMapHitFrame != currentFrame)
        {
            cachedSelectedMapHitFrame = currentFrame;
            hasCachedSelectedMapHit = false;
            cachedSelectedMapHit = default;

            if (pointer != null)
            {
                hasCachedSelectedMapHit = Physics.Raycast(
                    pointer.position,
                    pointer.TransformDirection(Vector3.forward),
                    out cachedSelectedMapHit,
                    100,
                    placementLayermask,
                    QueryTriggerInteraction.Collide
                );
            }
        }

        if (!hasCachedSelectedMapHit)
        {
            hit = default;
            return false;
        }

        hit = cachedSelectedMapHit;
        return true;
    }

    public bool TryGetPointerRotation(out Quaternion rotation)
    {
        if (pointer == null)
        {
            rotation = Quaternion.identity;
            return false;
        }

        rotation = pointer.rotation;
        return true;
    }

    public bool TryGetPointerRay(out Ray ray)
    {
        if (pointer == null)
        {
            ray = default;
            return false;
        }

        ray = new Ray(pointer.position, pointer.TransformDirection(Vector3.forward));
        return true;
    }

    private void UpdatePointerRayRenderer()
    {
        if (pointerRayRenderer == null || pointer == null)
        {
            return;
        }

        Vector3 start = pointer.position;
        Vector3 direction = pointer.TransformDirection(Vector3.forward);
        Vector3 end = start + (direction * pointerRayLength);

        if (TryGetSelectedMapHit(out var hit))
        {
            end = hit.point;
        }

        pointerRayRenderer.positionCount = 2;
        pointerRayRenderer.SetPosition(0, start);
        pointerRayRenderer.SetPosition(1, end);
    }

}
