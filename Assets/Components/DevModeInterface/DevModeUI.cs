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
        SceneSwitcher.Instance.SwitchToScene(18, "DefaultSpawnPoint");
    }
}
