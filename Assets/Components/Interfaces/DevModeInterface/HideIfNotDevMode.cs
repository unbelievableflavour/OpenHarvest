using UnityEngine;

public class HideIfNotDevMode : MonoBehaviour
{
    public HarvestSettings HarvestSettings;

    void Start()
    {
        if (!HarvestSettings.enableDevMode)
        {
            gameObject.SetActive(false);
        }
    }
}
