using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject gameButton1;
    public GameObject gameButton2;
    public GameObject gameButton3;

    public GameObject mainCanvas;
    public GameObject deleteSaveCanvas;
    public GameObject playInRealTimeCanvas;
    public GameObject chooseNameCanvas;
    public GameObject chooseFarmNameCanvas;
    public GameObject doYouWantToPlayTutorialCanvas;
    public GameObject errorCanvas;
    public GameObject progressCanvas;
    public GameObject settingsCanvas;

    static bool hasChosenIfWantsToPlayInRealTime = false;
    static bool hasChosenName = false;
    static bool hasChosenFarmName = false;
    static bool hasChosenIfWantsToPlayTutorial = false;

    // Start is called before the first frame update
    void Start()
    {
        gameButton1.GetComponent<GameInformation>().UpdateInformation();
        gameButton2.GetComponent<GameInformation>().UpdateInformation();
        gameButton3.GetComponent<GameInformation>().UpdateInformation();
    }

    public void NewGame()
    {
        if (!hasChosenIfWantsToPlayInRealTime)
        {
            hasChosenIfWantsToPlayInRealTime = true;
            DisableAllCanvas();
            playInRealTimeCanvas.SetActive(true);
            return;
        }

        if (!hasChosenName)
        {
            hasChosenName = true;
            DisableAllCanvas();
            chooseNameCanvas.SetActive(true);
            return;
        }

        if (GameState.Instance.name == "")
        {
            ShowErrorCanvas("Name cannot be empty.", chooseNameCanvas);
            return;
        }

        if (!hasChosenFarmName)
        {
            hasChosenFarmName = true;
            DisableAllCanvas();
            chooseFarmNameCanvas.SetActive(true);
            return;
        }

        if (GameState.Instance.farmName == "")
        {
            ShowErrorCanvas("Farm name cannot be empty.", chooseFarmNameCanvas);
            return;
        }

        if (!hasChosenIfWantsToPlayTutorial)
        {
            hasChosenIfWantsToPlayTutorial = true;
            DisableAllCanvas();
            doYouWantToPlayTutorialCanvas.SetActive(true);
            return;
        }

        GameState.Reset();
        AddSomeStartingEquipmentToPlayer();
        SavingController.SaveGame();
        SceneSwitcher.Instance.SwitchToScene(1, "DefaultSpawnPoint");
    }

    private void AddSomeStartingEquipmentToPlayer()
    {
        GameState.Instance.itemStashes["backpack"] = new List<SaveableItem>();
        GameState.Instance.itemStashes["backpack"].Add(new SaveableItem()
        {
            id = "SeedBagTomato",
            currentStackSize = 10,
        });
    }

    public void PlayTutorial()
    {
        GameState.Reset();
        AddSomeStartingEquipmentToPlayer();
        SavingController.SaveGame();
        SceneSwitcher.Instance.SwitchToScene(7, "MainMenu");
    }

    public void SetPlayInRealTime()
    {
        TimeController.SetPlayInRealTime();
    }

    public void SetPlayInSimulatedTime()
    {
        TimeController.SetPlayInSimulatedTime();
    }

    public void SetName(Text label)
    {
        GameState.Instance.name = label.text;
    }

    public void SetFarmName(Text label)
    {
        GameState.Instance.farmName = label.text;
    }

    private void DisableAllCanvas()
    {
        mainCanvas.SetActive(false);
        deleteSaveCanvas.SetActive(false);
        playInRealTimeCanvas.SetActive(false);
        doYouWantToPlayTutorialCanvas.SetActive(false);
        chooseNameCanvas.SetActive(false);
        chooseFarmNameCanvas.SetActive(false);
        errorCanvas.SetActive(false);
        progressCanvas.SetActive(false);
        settingsCanvas.SetActive(false);
    }

    public void ShowMainCanvas()
    {
        gameButton1.GetComponent<GameInformation>().UpdateInformation();
        gameButton2.GetComponent<GameInformation>().UpdateInformation();
        gameButton3.GetComponent<GameInformation>().UpdateInformation();

        DisableAllCanvas();
        mainCanvas.SetActive(true);
    }

    public void ShowErrorCanvas(string errorMessage, GameObject previousDialog)
    {
        var errorMessageController = errorCanvas.GetComponent<ErrorMessageController>();
        errorMessageController.SetErrorMessage(errorMessage, previousDialog);

        DisableAllCanvas();
        errorCanvas.SetActive(true);
    }

    public void ShowProgressCanvasWithSave(SaveGame saveGame)
    {
        var progressCanvasController = progressCanvas.GetComponent<ProgressCanvasController>();
        progressCanvasController.UpdateProgressCanvasWithSave(saveGame);

        var settingsController = settingsCanvas.GetComponent<SettingsController>();
        settingsController.UpdateSettingsWithPrefs();

        DisableAllCanvas();
        progressCanvas.SetActive(true);
        settingsCanvas.SetActive(true);
    }

    public void ShowDeleteSaveCanvas()
    {
        DisableAllCanvas();
        deleteSaveCanvas.SetActive(true);
    }
}
