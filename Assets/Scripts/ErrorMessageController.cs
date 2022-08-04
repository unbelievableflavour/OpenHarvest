using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorMessageController : MonoBehaviour
{
    public Text errorTextLabel;
    private GameObject previousCanvas;

    public void SetErrorMessage(string newErrorMessage, GameObject newPreviousCanvas)
    {
        errorTextLabel.text = newErrorMessage;
        previousCanvas = newPreviousCanvas;
    }

    public void ShowPreviousCanvas()
    {
        previousCanvas.SetActive(true);
        gameObject.SetActive(false);
    }
}
