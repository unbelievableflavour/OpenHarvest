using BNG;
using System.Collections;
using UnityEngine;

public class OVRDetector : MonoBehaviour
{
    private void Update()
    {
        if (OVRPlugin.shouldQuit)
        {
            Debug.Log("[SceneController] OVRPlugin.shouldQuit detected");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }
    }
}