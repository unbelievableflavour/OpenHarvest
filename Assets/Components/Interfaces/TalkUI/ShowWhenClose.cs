using UnityEngine;

public class ShowWhenClose : MonoBehaviour
{
    public GameObject gameObject;
    public TalkUIController talkUIController;
    public float maxShowDistance = 3f;

    bool shouldBeEnabled = true;
    Transform mainCam;

    void Start()
    {
        mainCam = Camera.main.transform;
    }

    void Update()
    {
        float currentDistance = Vector3.Distance(transform.position, mainCam.position);
        UpdateActiveState(currentDistance <= maxShowDistance);
    }

    void UpdateActiveState(bool newState)
    {
        if (shouldBeEnabled == newState) {
            return;
        }
        shouldBeEnabled = newState;

        if (shouldBeEnabled) {
            gameObject.SetActive(true);
        } else {
            talkUIController.Reset();
            gameObject.SetActive(false);
        }
    }
}
