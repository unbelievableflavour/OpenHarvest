using BNG;
using UnityEngine;

public class SwitchSceneOnDoorOpen : MonoBehaviour
{
    public bool doorOpensLeft;
    public int sceneIndex;
    public string sceneEnterLocationName;

    private bool doorIsEnabled = true;
    private DoorHelper doorHelper;
    
    private bool heldOnce = false;

    void Start()
    {
        doorHelper = GetComponent<DoorHelper>();
    }

    void Update()
    {
        if(!doorHelper || !doorIsEnabled)
        {
            return;
        }

        if (!heldOnce)
        {
            if (!doorHelper.HandleFollower || !doorHelper.HandleFollower.GetComponent<HandleGFXHelper>())
            {
                return;
            }

            if (doorHelper.HandleFollower.GetComponent<HandleGFXHelper>().HandleGrabbable.BeingHeld)
            {
                heldOnce = true;
            }
            return;
        }



        if (doorOpensLeft)
        {
            if (doorHelper.angle < 175 )
            {
                SwitchScene();
            }
            return;
        }
       

        if (doorHelper.angle > 5)
        {
            SwitchScene();
        }
    }

    private void SwitchScene()
    {
        SceneSwitcher.Instance.SwitchToScene(sceneIndex, sceneEnterLocationName);
        doorIsEnabled = false;
    }
}


