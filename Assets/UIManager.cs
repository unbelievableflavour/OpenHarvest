using BNG;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private ViewSwitcher viewSwitcher;
    [SerializeField] private string mainViewId = "default";

    private FirstPersonController fpsController;
    private SmoothLocomotion smoothLocomotion;
    private PlayerTeleport playerTeleport;

    private bool cachedControllerState;
    private bool cachedCameraCanMove = true;
    private bool cachedPlayerCanMove = true;
    private bool smoothLocomotionAllowInput = true;
    private bool playerTeleportEnabled = true;
    private CursorLockMode cachedCursorLockMode = CursorLockMode.Locked;
    private bool cachedCursorVisible = false;

    private void Awake()
    {
        if (viewSwitcher == null)
        {
            viewSwitcher = FindFirstObjectByType<ViewSwitcher>();
        }

        fpsController = FindFirstObjectByType<FirstPersonController>();
        smoothLocomotion = FindFirstObjectByType<SmoothLocomotion>();
        playerTeleport = FindFirstObjectByType<PlayerTeleport>();
    }

    private void OnEnable()
    {
        if (viewSwitcher != null)
        {
            viewSwitcher.OnViewChanged += HandleViewChanged;
        }

        if (viewSwitcher != null && viewSwitcher.currentView != null)
        {
            HandleViewChanged(viewSwitcher.currentView.id);
        }
    }

    private void OnDisable()
    {
        if (viewSwitcher != null)
        {
            viewSwitcher.OnViewChanged -= HandleViewChanged;
        }
    }

    private void HandleViewChanged(string viewId)
    {
        bool isMain = string.Equals(viewId, mainViewId);
        ApplyPlayerInteractionState(!isMain);
    }

    private void ApplyPlayerInteractionState(bool uiVisible)
    {
        if (uiVisible)
        {
            if (!cachedControllerState)
            {
                cachedCursorLockMode = Cursor.lockState;
                cachedCursorVisible = Cursor.visible;
                if (fpsController != null)
                {
                    cachedCameraCanMove = fpsController.cameraCanMove;
                    cachedPlayerCanMove = fpsController.playerCanMove;
                }
                if (smoothLocomotion != null) smoothLocomotionAllowInput = smoothLocomotion.AllowInput;
                if (playerTeleport != null) playerTeleportEnabled = playerTeleport.enabled;
                cachedControllerState = true;
            }

            if (fpsController != null)
            {
                fpsController.cameraCanMove = false;
                fpsController.playerCanMove = false;
            }
            if (smoothLocomotion != null) smoothLocomotion.AllowInput = false;
            if (playerTeleport != null) playerTeleport.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        if (fpsController != null)
        {
            fpsController.cameraCanMove = cachedCameraCanMove;
            fpsController.playerCanMove = cachedPlayerCanMove;
        }
        if (smoothLocomotion != null) smoothLocomotion.AllowInput = smoothLocomotionAllowInput;
        if (playerTeleport != null) playerTeleport.enabled = playerTeleportEnabled;
        Cursor.lockState = cachedCursorLockMode;
        Cursor.visible = cachedCursorVisible;
        cachedControllerState = false;
    }
}
