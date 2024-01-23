using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubMenuTutorialMenuController : MonoBehaviour
{
    public ViewSwitcher globalViewSwitcher;
    public GameObject mainCanvas;
    public GameObject ranchingCanvas;

    public void StartTutorial(GameObject tutorialObject)
    {
        DisableMenu();
        tutorialObject.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        globalViewSwitcher.setActiveView("main");
    }

    private void DisableMenu()
    {
        ranchingCanvas.SetActive(false);
    }
}