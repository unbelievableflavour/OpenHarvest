using UnityEngine;
using UnityEngine.UI;

public class FadeOutWhenClose : MonoBehaviour
{
    public Image icon;
    public Text text;
    public Image pointer;
    public float maxShowDistance = 8f;

    bool shouldBeEnabled = true;
    Transform mainCam;

    // Start is called before the first frame update
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
        if (shouldBeEnabled == newState)
        {
            return;
        }

        shouldBeEnabled = newState;

        if (shouldBeEnabled) {
            moveClose();
        } else {
            moveAway();
        }
    }


    void moveClose()
    {
        icon.CrossFadeAlpha(0.0f, 1.0f, false);
        text.CrossFadeAlpha(0.0f, 1.0f, false);
        pointer.CrossFadeAlpha(0.0f, 1.0f, false);
    }

    void moveAway()
    {
        icon.CrossFadeAlpha(1.0f, 1.0f, false);
        text.CrossFadeAlpha(1.0f, 1.0f, false);
        pointer.CrossFadeAlpha(1.0f, 1.0f, false);
    }
}
