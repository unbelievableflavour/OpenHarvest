using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainTutorialMenuController : MonoBehaviour
{
    public ViewSwitcher viewSwitcher;
    public List<Button> buttons = new List<Button>();

    void Start()
    {
        if (GameState.Instance.enteredSceneThrough == "MainMenu")
        {
            StartBasicTutorial();
        }
    }

    private void StartBasicTutorial()
    {
        viewSwitcher.setActiveView("basics");
    }

    public void StartTutorial(string tutorialId)
    {
        viewSwitcher.setActiveView(tutorialId);
    }

    public void ReturnToGame()
    {
        DisableAllButtons();
        SceneSwitcher.Instance.SwitchToScene(
            GameState.Instance.enteredSceneThrough == "MainMenu" ? 1 : (int.TryParse(GameState.Instance.enteredSceneThrough, out var i) ? i : 1),
            "DefaultSpawnPoint"
        );
    }

    public void ReturnToMainMenu()
    {
        viewSwitcher.setActiveView("main");
    }

    private void DisableAllButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }
}