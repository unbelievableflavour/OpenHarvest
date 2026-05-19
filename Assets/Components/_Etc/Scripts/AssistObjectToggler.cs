using System;
using UnityEngine;

public class AssistObjectToggler : MonoBehaviour
{
    public GameObject objectToToggle;

    void Start()
    {
        PlayerCustomSettings.Instance.assistModeToggled += handleAssistModeToggled;
        ToggleObject();
    }

    private void handleAssistModeToggled(object sender, EventArgs e)
    {
        ToggleObject();
    }

    private void ToggleObject()
    {
        objectToToggle.SetActive(bool.Parse(GameState.Instance.settings["useAssistMode"]) ? true : false);    
    }

    void OnDestroy()
    {
        PlayerCustomSettings.Instance.assistModeToggled -= handleAssistModeToggled;
    }
}
