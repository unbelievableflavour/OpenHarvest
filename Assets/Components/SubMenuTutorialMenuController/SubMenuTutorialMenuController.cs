using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubMenuTutorialMenuController : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject ranchingCanvas;

    public void StartTutorial(GameObject tutorialObject)
    {
        DisableMenu();
        tutorialObject.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        DisableMenu();
        mainCanvas.SetActive(true);
    }

    private void DisableMenu()
    {
        ranchingCanvas.SetActive(false);
    }
}