using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SwitchToMode : MonoBehaviour
{
    public string mode;

    private void Start()
    {
        if (string.Equals(mode, "build", StringComparison.OrdinalIgnoreCase) && !IsBuildModeAllowedInCurrentScene())
        {
            gameObject.SetActive(false);
        }
    }

    public void SwitchMode() {
        if(GameState.Instance.GetMode() != mode) {
            GameState.Instance.SwitchToMode(mode);
        } else {
            GameState.Instance.SwitchToMode("default");
        }
    }

    private static bool IsBuildModeAllowedInCurrentScene()
    {
        if (DatabaseManager.Instance == null || DatabaseManager.Instance.scenes == null)
        {
            return true;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        HarvestDataTypes.SceneSettings sceneSettings = DatabaseManager.Instance.scenes.FindByScene(activeScene.name, activeScene.buildIndex);
        if (sceneSettings == null)
        {
            return true;
        }

        return sceneSettings.allowBuildMode;
    }
}
