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
        if (HarvestSettings.isPCMode)
        {
            GetComponent<HeadsetDetector>().enabled = false;
            VRPlayerObject.SetActive(false);
            Instantiate(PCPlayerObject);
        }
    }
#endif
}
