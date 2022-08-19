using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressCanvasController : MonoBehaviour
{
    public Text nameLabel;
    public Text farmNameLabel;
    public Text PlayTimeLabel;
    public Text playModeLabel;
    public Text creditsLabel;
    public Button loadButton;
    public Button deleteButton;
    public Button settingsButton;

    private int saveNumber;

    public void UpdateProgressCanvasWithSave(SaveGame save)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(save.totalSecondsSpentIngame);

        saveNumber = save.saveNumber;

        nameLabel.text = "Name: " + save.name;
        farmNameLabel.text = "Farm: " + save.farmName;
        PlayTimeLabel.text = "Playtime: " + timeSpan.Hours + "H " + timeSpan.Minutes + "M " + timeSpan.Seconds + "S";
        playModeLabel.text = "Time mode: " + (save.useRealTime ? "Real" : "Simulated");
        creditsLabel.text = "Gold: " + save.money;

        loadButton.onClick.AddListener(OnClickStartFarm);
        DeleteSaveCanvasController.currentSaveToDelete = saveNumber;
    }

    void OnClickStartFarm()
    {
        // Game is not actually loaded here. It's already loaded when selecting the game.
        deleteButton.GetComponent<Button>().interactable = false;
        settingsButton.GetComponent<Button>().interactable = false;
        loadButton.GetComponent<Button>().interactable = false;

        SceneSwitcher.Instance.SwitchToScene(1, "DefaultSpawnPoint");
    }
}