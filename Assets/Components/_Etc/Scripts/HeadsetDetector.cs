using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HeadsetDetector : MonoBehaviour
{
    public GameObject LeftHand;
    public GameObject RightHand;
    public GameObject RightHandPointer;
    
    //not working in XR
    //private bool bPause = ifInOculusHomeOrHeadsetIsNotWorn();

    private bool bPause = false;
    private void Update()
    {
        //pause if in oculus home universal menu, and if headset not worn
        bool bPauseNow = false;

        //pause state change
        if (bPause != bPauseNow)
        {
            bPause = bPauseNow;
            if (bPauseNow)
            {
                DisableGame();
            }
            else
            {
                EnableGame();
            }
        }
    }

    private void EnableGame()
    {
        Time.timeScale = 1.0f;
        AudioListener.pause = false;
        LeftHand.SetActive(true);
        RightHand.SetActive(true);
        RightHandPointer.SetActive(true);
    }

    private void DisableGame()
    {
        Time.timeScale = 0.0f;
        AudioListener.pause = true;
        LeftHand.SetActive(false);
        RightHand.SetActive(false);
        RightHandPointer.SetActive(false);
    }

    private bool ifInOculusHomeOrHeadsetIsNotWorn()
    {
        // Using XR APIs instead of OVR APIs
        // Check if XR is active and has input focus
        return !(XRSettings.enabled && XRSettings.isDeviceActive);
    }
}
