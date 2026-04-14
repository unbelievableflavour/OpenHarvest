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
    public CanvasGroup canvasGroup;

    // Animate opacity
    private float _minOpacity;
    private float _maxOpacity;
    private float _currentOpacity;

    Transform mainCam;
    Vector3 transformPos;
    bool showCanvasGroup;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main.transform;
        transformPos = transform.position; // storing the transform here saves about 10ms in performance;

        _maxOpacity = canvasGroup.alpha;
        _minOpacity = 0;

        _currentOpacity = _minOpacity;
        canvasGroup.alpha = _minOpacity;

        canvasGroup.gameObject.SetActive(false);
    }

    void Update()
    {
        // Show if within range
        float currentDistance = Vector3.Distance(transformPos, mainCam.position);
        showCanvasGroup = currentDistance <= maxShowDistance;

        if (!showCanvasGroup && _currentOpacity == _minOpacity)
        {
            return;
        }

        if (showCanvasGroup && _currentOpacity == _maxOpacity)
        {
            return;
        }

        UpdateCG();
    }

    void UpdateCG()
    {
        if (showCanvasGroup)
        {
            if(!canvasGroup.gameObject.activeSelf) {
              canvasGroup.gameObject.SetActive(true);
            }

            // fade in
            _currentOpacity += Time.deltaTime * RingFadeSpeed;
            if (_currentOpacity > _maxOpacity)
            {
                _currentOpacity = _maxOpacity;
            }

            canvasGroup.alpha = _currentOpacity;
            return;
        }

        // fade out
        _currentOpacity -= Time.deltaTime * RingFadeSpeed;
        if (_currentOpacity <= _minOpacity) {
            _currentOpacity = _minOpacity;
        }
        
        canvasGroup.alpha = _currentOpacity;

        if(_currentOpacity == _minOpacity) {
            canvasGroup.gameObject.SetActive(false);
        }
    }
}


