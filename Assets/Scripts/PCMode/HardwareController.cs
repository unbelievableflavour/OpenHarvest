using UnityEngine;

public class HardwareController : MonoBehaviour
{
    public bool enablePCMode = false;
    public GameObject VRPlayerObject;
    public GameObject PCPlayerObject;

    void Start()
    {
        if (enablePCMode)
        {
            GetComponent<HeadsetDetector>().enabled = false;
            VRPlayerObject.SetActive(false);
            PCPlayerObject.SetActive(true);
        }
    }
}
