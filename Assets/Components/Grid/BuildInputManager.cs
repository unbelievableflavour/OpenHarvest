using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using BNG;

public class BuildInputManager : MonoBehaviour
{
	public static BuildInputManager Instance = null;
    public HarvestSettings harvestSettings;

    [SerializeField]
    private Transform pointer;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;

    public event Action OnClicked, OnPreviousItem, OnNextItem;
        
    public List<ControllerBinding> placeButton = new List<ControllerBinding>() { ControllerBinding.None };
    public List<ControllerBinding> previousButton = new List<ControllerBinding>() { ControllerBinding.None };
    public List<ControllerBinding> nextButton = new List<ControllerBinding>() { ControllerBinding.None };

    // Initialize instance.
    private void Awake()
    {
        if (Instance != this)
        {
            Instance = this;
        }
    }

    private void Update() {
        if(GameState.Instance.GetMode() != "build") {
            return;
        }
        
        // vr
        if(this.KeyDown(placeButton)){
            OnClicked?.Invoke();
        }

        if(this.KeyDown(previousButton)){
            OnPreviousItem?.Invoke();
        }

        if(this.KeyDown(nextButton)){
            OnNextItem?.Invoke();
        }

#if UNITY_EDITOR
        if (harvestSettings.isPCMode)
        {
            if(Input.GetMouseButtonDown(0)){
                OnClicked?.Invoke();
            }

            if(Input.GetKeyDown(KeyCode.LeftArrow)){
                OnPreviousItem?.Invoke();
            }

            if(Input.GetKeyDown(KeyCode.RightArrow)){
                OnNextItem?.Invoke();
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
