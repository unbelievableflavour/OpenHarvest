using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private ViewSwitcher viewSwitcher;
    [SerializeField] private string mainViewId = "default";

    private FirstPersonController cachedFpsController;
    private bool cachedControllerState;
    private bool cachedCameraCanMove = true;
    private bool cachedPlayerCanMove = true;
    private CursorLockMode cachedCursorLockMode = CursorLockMode.Locked;
    private bool cachedCursorVisible = false;

    private void Awake()
    {
        if (viewSwitcher == null)
        {
            viewSwitcher = FindFirstObjectByType<ViewSwitcher>();
        }

        cachedFpsController = FindFirstObjectByType<FirstPersonController>();
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
        ApplyPcInteractionState(!isMain);
    }

    private void ApplyPcInteractionState(bool uiVisible)
    {
        if (uiVisible)
        {
            if (!cachedControllerState)
            {
                cachedCursorLockMode = Cursor.lockState;
                cachedCursorVisible = Cursor.visible;
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
    }
}
