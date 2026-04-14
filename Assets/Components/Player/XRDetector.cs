using BNG;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class XRDetector : MonoBehaviour
{
    private void Update()
    {
        // Note: Original OVRPlugin.shouldQuit functionality is not directly available in generic XR
        // For now, we'll disable this check as it was causing false positives
        // TODO: Implement proper XR quit detection if needed for specific XR platforms
        
        // Commented out the problematic quit detection:
        // if (XRSettings.enabled && !XRSettings.isDeviceActive)
        // {
        //     Debug.Log("[SceneController] XR device quit detected");
        // #if UNITY_EDITOR
        //     UnityEditor.EditorApplication.isPlaying = false;
        // #else
        //     Application.Quit();
        // #endif
        // }
    }
}