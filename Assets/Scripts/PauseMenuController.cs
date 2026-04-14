using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public GameObject MainMenuUI;
    public GameObject SettingsUI;

    private bool isOpen = false;

    public void OnEnable() {
        HarvestInputManager.Instance.OnMenuButton += ToggleMenu;
    }

    public void OnDisable() {
        HarvestInputManager.Instance.OnMenuButton -= ToggleMenu;
    }

    public void ToggleMenu() {
        if( isOpen) {
            CloseMainMenu();
        } else {
            OpenMainMenu();
        }
    }

    public void OpenMainMenu()
    {
        isOpen = true;
        MainMenuUI.SetActive(true);
    }

    public void CloseMainMenu()
    {
        isOpen = false;
        MainMenuUI.SetActive(false);
        SettingsUI.SetActive(false);
    }
}
