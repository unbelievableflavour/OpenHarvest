using UnityEngine;

public class ShowWhenClose : MonoBehaviour
{
    public GameObject gameObject;
    public TalkUIController talkUIController;
    public float maxShowDistance = 3f;

    bool shouldBeEnabled = true;

    bool isStartingUp = true;

    Transform mainCam;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("EndStartingUp", 1);
        mainCam = Camera.main.transform;
    }

    void Update()
    {
        float currentDistance = Vector3.Distance(transform.position, mainCam.position);
        UpdateActiveState(currentDistance <= maxShowDistance);
    }

    void UpdateActiveState(bool newState)
    {
        if (shouldBeEnabled == newState && !isStartingUp)
        {
            return;
        }
        shouldBeEnabled = newState;

        if (shouldBeEnabled)
        {
            gameObject.SetActive(true);
        } else
        {
            talkUIController.Reset();
            gameObject.SetActive(false);
        }
    }

    //This is a little hack to make sure the talkUI is there on start.
    void EndStartingUp()
    {
        isStartingUp = false;
    }
}
