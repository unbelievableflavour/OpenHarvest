using UnityEngine;

public class DeleteSaveCanvasController : MonoBehaviour
{
    public static int currentSaveToDelete;

    public MainMenuController mainMenuController;

    public void DeleteSave()
    {
        SavingController.DeleteGame(currentSaveToDelete);
        mainMenuController.ShowMainCanvas();
    }
}
