using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    }

    private void Update() {        
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
            if(Input.GetMouseButtonDown(0)){
                OnTriggerRight?.Invoke();
            }

            if(Input.GetKeyDown(KeyCode.LeftArrow)){
                OnBButton?.Invoke();
            }

            if(Input.GetKeyDown(KeyCode.RightArrow)){
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
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(pointer.position, pointer.TransformDirection(Vector3.forward), out hit, 100, placementLayermask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }
}
