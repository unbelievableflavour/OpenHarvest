using UnityEngine;

public class DevModeUI : MonoBehaviour
{
    public HarvestSettings HarvestSettings;

    void Start()
    {
        if (!HarvestSettings.enableSandbox)
        {
            gameObject.SetActive(false);
        }
    }

    public void StartSandbox()
    {
        GameState.currentSceneSwitcher.SwitchToScene(18, "DefaultSpawnPoint");
    }
}
