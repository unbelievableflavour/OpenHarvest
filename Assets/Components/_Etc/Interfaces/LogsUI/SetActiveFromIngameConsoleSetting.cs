using UnityEngine;

public class SetActiveFromIngameConsoleSetting : MonoBehaviour
{
    [SerializeField] private HarvestSettings harvestSettings;

    private void Awake()
    {
        Apply();
    }

    public void Apply()
    {
        HarvestSettings settings = harvestSettings;
        if (settings == null)
        {
            settings = HarvestInputManager.Instance?.harvestSettings;
        }

        if (settings == null)
        {
            return;
        }

        if (!settings.enableIngameConsole)
        {
            gameObject.SetActive(false);
        }
    }
}
