using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
using UnityEngine.InputSystem;

public class SmoothLocomotionCustom : MonoBehaviour
{
    public SmoothLocomotion smoothLocomotion;
    public List<ControllerBinding> SprintInput = new List<ControllerBinding>() { ControllerBinding.None };
    public InputActionReference SprintAction;
    public float SprintSpeed = 1.5f;
    public float StrafeSprintSpeed = 1.25f;

    Vector2 emptyVector;
    bool isMoving = false;
    bool isSprinting = false;
    float strafeSpeedBackup;
    float movementSpeedBackup;

    void Start() {
        strafeSpeedBackup = smoothLocomotion.StrafeSpeed;
        movementSpeedBackup = smoothLocomotion.MovementSpeed;
        emptyVector = new Vector2(0, 0);
    }

    void Update() {
        if(smoothLocomotion.GetMovementAxis() == emptyVector) {
            UpdateMovingState(false);
            return;
        }
        UpdateMovingState(true);
        if(!isSprinting) {
            UpdateSprintState(CheckSprint());
        }
    }

    void UpdateMovingState(bool newState)
    {
        if (isMoving == newState)
        {
            return;
        }
        isMoving = newState;

        if (!isMoving) {
            UpdateSprintState(false);
            Debug.Log("stopped moving");
        }
    }

    void UpdateSprintState(bool newState)
    {
        if (isSprinting == newState)
        {
            return;
        }

        isSprinting = newState;
        if(isSprinting) {
            Debug.Log("sprint this");
            smoothLocomotion.StrafeSpeed = StrafeSprintSpeed;
            smoothLocomotion.MovementSpeed = SprintSpeed;
        } else {
            ResetSpeeds();
            Debug.Log("stopped sprinting");
        }
    }

    void ResetSpeeds() {
        smoothLocomotion.StrafeSpeed = strafeSpeedBackup;
        smoothLocomotion.MovementSpeed = movementSpeedBackup;
    }

    public virtual bool CheckSprint() {
        // Check for bound controller button
        for (int x = 0; x < SprintInput.Count; x++) {
            if (InputBridge.Instance.GetControllerBindingValue(SprintInput[x])) {
                return true;
            }
        }

        // Check Unity Input Action
        if (SprintAction != null) {
            return SprintAction.action.ReadValue<float>() == 1f;
        }

        return false;
    }
}
