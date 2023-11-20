using UnityEngine;
using UnityEngine.UI;

public class GameInformation : MonoBehaviour
{
    public int saveNumber;
    public Text GameButtonLabel;
    public MainMenuController mainMenuController;
    public SaveGame currentSave;
    public void UpdateInformation()
    {
        try {
            if (!SavingController.SaveExists(saveNumber))
            {
                GameButtonLabel.text = "New Game";
                SetAsNewGame();
                return;
            }

            currentSave = SavingController.GetSaveInformation(saveNumber);
            GameButtonLabel.text = currentSave.name;
            GetComponent<Button>().onClick.AddListener(OnClickLoadGame);
        } catch {
            GameButtonLabel.fontSize = 12;
            GameButtonLabel.text = "This save file is broken. Please find help in the discord.";
        }
    }

    public void SetAsNewGame()
    {
        GetComponent<Button>().onClick.AddListener(OnClickNewGame);
    }

    void OnClickNewGame()
    {
        GameState.Reset();
        GameState.Instance.saveNumber = saveNumber;
        mainMenuController.NewGame();
    }

    void OnClickLoadGame()
    {
        SavingController.LoadGame(saveNumber);
        mainMenuController.ShowProgressCanvasWithSave(currentSave);
    }
}