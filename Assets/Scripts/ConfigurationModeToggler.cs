using System.Collections.Generic;
using UnityEngine;

public class ConfigurationModeToggler : MonoBehaviour
{
    public enum ConfigurationLocations
    {
        Home,
        Greenhouse,
    };

    public ConfigurationLocations configurationLocation;
    public List<ItemLocation> itemLocations;
    private bool useConfigurationMode = false;

    public void Toggle()
    {
        useConfigurationMode = useConfigurationMode ? false : true;

        foreach (var itemLocation in itemLocations)
        {
            itemLocation.chooserUI.SetActive(useConfigurationMode);
            itemLocation.SetConfigurationModeToggler(this);
            itemLocation.Load();
        }
    }

    public void RefreshItems()
    {
        foreach (var itemLocation in itemLocations)
        {
            itemLocation.UpdateLabels();
        }
    }

    public bool configurationModeIsActive()
    {
        return useConfigurationMode;
    }
}
