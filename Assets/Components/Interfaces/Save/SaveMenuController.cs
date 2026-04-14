using UnityEngine;

public class SaveMenuController : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject saveSuccessfulCanvas;

    public void SaveGame()
    {
        SavingController.SaveGame();
        mainCanvas.SetActive(false);
        saveSuccessfulCanvas.SetActive(true);
    }

    public void CloseSaveSuccessfull()
    {
        mainCanvas.SetActive(true);
        saveSuccessfulCanvas.SetActive(false);
    }
}
