using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainTutorialMenuController : MonoBehaviour
{
    public List<Button> buttons = new List<Button>();
    public GameObject mainCanvas;
    public GameObject basicsTutorial;
    public GameObject fishingTutorial;
    public GameObject cookingTutorial;
    public GameObject miningTutorial;
    public GameObject woodcuttingTutorial;
    public GameObject ranchingTutorial;

    void Start()
    {
        if (GameState.Instance.enteredSceneThrough == "MainMenu")
        {
            StartBasicTutorial();
        }
    }

    public void StartBasicTutorial()
    {
        DisableMainMenu();
        basicsTutorial.SetActive(true);
    }

    public void StartFishingTutorial()
    {
        DisableMainMenu();
        fishingTutorial.SetActive(true);
    }

    public void StartCookingTutorial()
    {
        DisableMainMenu();
        cookingTutorial.SetActive(true);
    }

    public void StartMiningTutorial()
    {
        DisableMainMenu();
        miningTutorial.SetActive(true);
    }

    public void StartWoodcuttingTutorial()
    {
        DisableMainMenu();
        woodcuttingTutorial.SetActive(true);
    }

    public void StartRanchingTutorial()
    {
        DisableMainMenu();
        ranchingTutorial.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        mainCanvas.SetActive(true);
    }

    public void ReturnToGame()
    {
        DisableAllButtons();
        SceneSwitcher.Instance.SwitchToScene(
            GameState.Instance.enteredSceneThrough == "MainMenu" ? 1 : (int.TryParse(GameState.Instance.enteredSceneThrough, out var i) ? i : 1),
            "DefaultSpawnPoint"
        );
    }

    private void DisableMainMenu()
    {
        mainCanvas.SetActive(false);
    }

    private void DisableAllButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }
}