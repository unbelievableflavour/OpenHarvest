using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public List<ControllerBinding> PauzeInput = new List<ControllerBinding>() { ControllerBinding.None };
    public GameObject MainMenuUI;
    public GameObject SettingsUI;

    private bool isOpen = false;
    // Update is called once per frame
    void Update()
    {
        updateInputs();
    }

    void updateInputs()
    {
        if (PauzeKeyDown())
        {
            if(isOpen)
            {
                isOpen = false;
                MainMenuUI.SetActive(false);
                SettingsUI.SetActive(false);
            } else
            {
                OpenMainMenu();
            }
            
        }
    }

    public virtual bool PauzeKeyDown()
    {
        // Check for bound controller button
        for (int x = 0; x < PauzeInput.Count; x++)
        {
            if (InputBridge.Instance.GetControllerBindingValue(PauzeInput[x]))
            {
                return true;
            }
        }

        return false;
    }

    public void OpenMainMenu()
    {
        isOpen = true;
        MainMenuUI.SetActive(true);
    }
}
