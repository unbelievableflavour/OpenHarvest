using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindmillFlourButtonController : MonoBehaviour
{
    public WindmillController windmillController;
    public GameObject errorMessage;
    public Text errorLabel;

    void Start()
    {
        errorMessage.SetActive(false);
    }

    public void PushButton()
    {
        if (windmillController.getCurrentWheatCount() == 0)
        {
            CancelInvoke("HideErrorMessage");
            ShowErrorMessage("There is no wheat in the grinder.");
            Invoke("HideErrorMessage", 1.0f);
            return;
        }

        if (!windmillController.IsFlourReady())
        {
            CancelInvoke("HideErrorMessage");
            ShowErrorMessage("Flour is not done grinding.");
            Invoke("HideErrorMessage", 1.0f);
            return;
        }

        windmillController.DropFlour();
    }

    void ShowErrorMessage(string errorText)
    {
        errorLabel.text = errorText;
        errorMessage.SetActive(true);
    }

    void HideErrorMessage()
    {
        errorMessage.SetActive(false);
    }
}
