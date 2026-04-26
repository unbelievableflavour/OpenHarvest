using UnityEngine;

public class HardwareController : MonoBehaviour
{
    public HarvestSettings HarvestSettings;
    public GameObject VRPlayerObject;
    public GameObject PCPlayerObject;

    void Awake()
    {
        if (HarvestSettings.playerMode == PlayerMode.VR)
        {
            Instantiate(VRPlayerObject);
        }

        if (HarvestSettings.playerMode == PlayerMode.FPS)
        {
            Instantiate(PCPlayerObject);
        }
    }
}
