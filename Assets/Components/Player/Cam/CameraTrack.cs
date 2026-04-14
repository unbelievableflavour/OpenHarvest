using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraTransition
{
    public float movementSpeed = 0.1f;
    public float rotationSpeed = 0.1f;
    public Transform startLocation;
    public Transform endLocation;
}

public class CameraTrack : MonoBehaviour
{
    public int startTransitionIndex = 0;
    public bool loop = true;
    public float timeBetweenTransitions;
    public List<CameraTransition> cameraTransitions;

    CameraTransition currentCameraTransition;
    Transform mainCam;
    float timeCount = 0f;
    int currentIndex = 0;
    bool nextTransitionIsInvoked = false;

    void Start()
    {
        currentIndex = startTransitionIndex;
        currentCameraTransition = cameraTransitions[currentIndex];
        mainCam = Camera.main.transform;
        mainCam.position = currentCameraTransition.startLocation.position;

        // disable assist mode for camera.
        GameState.Instance.settings["useAssistMode"] = "false";
        PlayerCustomSettings.Instance.RefreshSettings();
    }

    void Update()
    {
        if(Vector3.Distance(mainCam.localPosition, currentCameraTransition.endLocation.position) <= 0) {
            if(currentIndex == cameraTransitions.Count - 1) {
                if(loop){
                    currentIndex = -1;
                    nextTransitionIsInvoked = true;
                    Invoke("StartNextTransition", timeBetweenTransitions);
                }
                return;
            }
            if(nextTransitionIsInvoked) {
                return;
            }
            nextTransitionIsInvoked = true;
            Invoke("StartNextTransition", timeBetweenTransitions);
        }
        moveObject();
    }

    public void moveObject() {
        timeCount += Time.deltaTime;
        mainCam.rotation = Quaternion.Lerp(currentCameraTransition.startLocation.rotation, currentCameraTransition.endLocation.rotation, timeCount * currentCameraTransition.rotationSpeed);
        mainCam.localPosition = Vector3.Lerp(currentCameraTransition.startLocation.position, currentCameraTransition.endLocation.position, timeCount * currentCameraTransition.movementSpeed);
    }

    void StartNextTransition() {
        nextTransitionIsInvoked = false;
        timeCount = 0;
        currentIndex = currentIndex+1;
        currentCameraTransition = cameraTransitions[currentIndex];
    }
}
