using System.Collections.Generic;
using BNG;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private ViewSwitcher viewSwitcher;
    [SerializeField] private string mainViewId = "default";

    private FirstPersonController fpsController;
    private SmoothLocomotion smoothLocomotion;
    private PlayerTeleport playerTeleport;
    private LocomotionManager locomotionManager;

    private readonly List<ViewSwitcher> additionalViewSwitchers = new List<ViewSwitcher>();

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
        locomotionManager = FindFirstObjectByType<LocomotionManager>();
        playerTeleport = ResolvePlayerTeleport();
        CacheAdditionalViewSwitchers();
    }

    private PlayerTeleport ResolvePlayerTeleport()
    {
        if (locomotionManager != null)
        {
            PlayerTeleport locomotionTeleport = locomotionManager.GetComponentInChildren<PlayerTeleport>(true);
            if (locomotionTeleport != null)
            {
                return locomotionTeleport;
            }
        }

        return FindFirstObjectByType<PlayerTeleport>();
    }

    private void CacheAdditionalViewSwitchers()
    {
        additionalViewSwitchers.Clear();
        ViewSwitcher[] allViewSwitchers = FindObjectsByType<ViewSwitcher>(FindObjectsSortMode.None);
        for (int i = 0; i < allViewSwitchers.Length; i++)
        {
            ViewSwitcher candidate = allViewSwitchers[i];
            if (candidate == null || candidate == viewSwitcher || !UsesMainViewId(candidate, mainViewId))
            {
                continue;
            }

            additionalViewSwitchers.Add(candidate);
        }
    }

    private static bool UsesMainViewId(ViewSwitcher switcher, string mainId)
    {
        if (switcher.views == null)
        {
            return false;
        }

        for (int i = 0; i < switcher.views.Count; i++)
        {
            View view = switcher.views[i];
            if (view != null && string.Equals(view.id, mainId))
            {
                return true;
            }
        }

        return false;
    }

    private void OnEnable()
    {
        if (viewSwitcher != null)
        {
            viewSwitcher.OnViewChanged += HandleViewChanged;
        }

        for (int i = 0; i < additionalViewSwitchers.Count; i++)
        {
            additionalViewSwitchers[i].OnViewChanged += HandleViewChanged;
        }

        RefreshPlayerInteractionState();
    }

    private void OnDisable()
    {
        if (viewSwitcher != null)
        {
            viewSwitcher.OnViewChanged -= HandleViewChanged;
        }

        for (int i = 0; i < additionalViewSwitchers.Count; i++)
        {
            additionalViewSwitchers[i].OnViewChanged -= HandleViewChanged;
        }
    }

    private void HandleViewChanged(string viewId)
    {
        RefreshPlayerInteractionState();
    }

    private void RefreshPlayerInteractionState()
    {
        ApplyPlayerInteractionState(IsAnyBlockingUiOpen());
    }

    private bool IsAnyBlockingUiOpen()
    {
        if (viewSwitcher != null && !IsMainView(viewSwitcher, mainViewId))
        {
            return true;
        }

        for (int i = 0; i < additionalViewSwitchers.Count; i++)
        {
            ViewSwitcher additionalSwitcher = additionalViewSwitchers[i];
            if (additionalSwitcher != null && !IsMainView(additionalSwitcher, mainViewId))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsMainView(ViewSwitcher switcher, string mainId)
    {
        if (switcher == null || switcher.currentView == null)
        {
            return true;
        }

        return string.Equals(switcher.currentView.id, mainId);
    }

    public void HandleLocomotionSettingsChanged()
    {
        if (!IsAnyBlockingUiOpen())
        {
            return;
        }

        UpdateCachedLocomotionState();

        if (playerTeleport != null)
        {
            playerTeleport.enabled = false;
        }
    }

    private void UpdateCachedLocomotionState()
    {
        if (locomotionManager == null)
        {
            return;
        }

        playerTeleportEnabled = locomotionManager.SelectedLocomotion == LocomotionType.Teleport;
        if (smoothLocomotion != null)
        {
            smoothLocomotionAllowInput = locomotionManager.SelectedLocomotion == LocomotionType.SmoothLocomotion;
        }
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
