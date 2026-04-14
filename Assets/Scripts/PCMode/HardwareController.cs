using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class HardwareController : MonoBehaviour
{    
    public HarvestSettings HarvestSettings;
    public GameObject VRPlayerObject;
    public GameObject PCPlayerObject;

#if UNITY_EDITOR
    void Start()
    {
        if (HarvestSettings.playerMode == PlayerMode.FPS)
        {
            GetComponent<HeadsetDetector>().enabled = false;
            VRPlayerObject.SetActive(false);
            Instantiate(PCPlayerObject);
        }

        if (HarvestSettings.playerMode == PlayerMode.Showcase)
        {
            GetComponent<HeadsetDetector>().enabled = false;
            VRPlayerObject.SetActive(false);
        }
    }
#endif
}
