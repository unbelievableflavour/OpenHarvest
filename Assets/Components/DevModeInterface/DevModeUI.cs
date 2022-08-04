using UnityEngine;

public class DevModeUI : MonoBehaviour
{
    public bool devMode = false;
    void Start()
    {
        if (!devMode)
        {
            gameObject.SetActive(false);
        }
    }

    public void StartSandbox()
    {
        GameState.currentSceneSwitcher.SwitchToScene(18, "DefaultSpawnPoint");
    }
}
