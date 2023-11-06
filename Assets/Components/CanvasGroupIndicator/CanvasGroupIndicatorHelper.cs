using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows a canvas group at the grab point of a grabbable if within a certain distance
/// </summary>
public class CanvasGroupIndicatorHelper : MonoBehaviour
{
    /// <summary>
    /// How fast to lerp the opacity if being hidden / shown
    /// </summary>
    public float RingFadeSpeed = 5;
    public float maxShowDistance = 2f;

    //Canvas canvas;
    CanvasGroup canvasGroup;

    // Animate opacity
    private float _minOpacity;
    private float _maxOpacity;
    private float _currentOpacity;

    Transform mainCam;
    Vector3 transformPos;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main.transform;
        transformPos = transform.position; // storing the transform here saves about 10ms in performance;
        canvasGroup = GetComponent<CanvasGroup>();

        _maxOpacity = canvasGroup.alpha;
        _minOpacity = 0;

        _currentOpacity = _minOpacity;
        canvasGroup.alpha = _minOpacity;
    }

    void Update()
    {
        // Show if within range
        float currentDistance = Vector3.Distance(transformPos, mainCam.position);
        bool showRings = currentDistance <= maxShowDistance;

        if (!showRings && _currentOpacity == _minOpacity)
        {
            return;
        }

        if (showRings && _currentOpacity == _maxOpacity)
        {
            return;
        }

        // Fade canvas group opacity in / out
        if (showRings)
        {
            _currentOpacity += Time.deltaTime * RingFadeSpeed;
            if (_currentOpacity > _maxOpacity)
            {
                _currentOpacity = _maxOpacity;
            }

            canvasGroup.alpha = _currentOpacity;
        } else {
            _currentOpacity -= Time.deltaTime * RingFadeSpeed;
            if (_currentOpacity <= _minOpacity)
            {
                _currentOpacity = _minOpacity;
                canvasGroup.alpha = _minOpacity;
            }
            else
            {
                canvasGroup.alpha = _currentOpacity;
            }
        }
    }
}


