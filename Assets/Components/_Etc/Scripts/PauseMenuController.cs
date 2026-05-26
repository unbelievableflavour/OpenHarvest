using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private ViewSwitcher viewSwitcher;
    [SerializeField] private string menuViewId = "menu";
    [SerializeField] private string gameplayViewId = "default";

    private bool IsMenuViewActive =>
        viewSwitcher?.currentView?.id == menuViewId;

    public void OnEnable() {
        HarvestInputManager.Instance.OnMenuButton += ToggleMenu;
    }

    public void OnDisable() {
        HarvestInputManager.Instance.OnMenuButton -= ToggleMenu;
    }

    public void ToggleMenu() {
        if (IsMenuViewActive) {
            CloseMainMenu();
        } else {
            OpenMainMenu();
        }
    }

    public void OpenMainMenu()
    {
        viewSwitcher.setActiveView(menuViewId);
    }

    public void CloseMainMenu()
    {
        viewSwitcher.setActiveView(gameplayViewId);
    }
}
