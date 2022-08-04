using UnityEngine;
using UnityEngine.UI;

namespace BNG
{
    /// <summary>
    /// Shows a ring at the grab point of a grabbable if within a certain distance
    /// </summary>
    public class SnapZoneRingHelperFader : MonoBehaviour
    {
        /// <summary>
        /// How fast to lerp the opacity if being hidden / shown
        /// </summary>
        public float RingFadeSpeed = 5;
        public float maxShowDistance = 2f;

        Canvas canvas;
        Text text;
        LookAtPlayer lookAtPlayer;

        // Animate opacity
        private float _initalOpacity;
        private float _currentOpacity;

        Transform mainCam;

        // Start is called before the first frame update
        void Start()
        {
            mainCam = Camera.main.transform;
            canvas = GetComponent<Canvas>();
            text = GetComponent<Text>();
            lookAtPlayer = GetComponent<LookAtPlayer>();

            _initalOpacity = text.color.a;
            _currentOpacity = _initalOpacity;
        }

        void Update()
        {
            // Bail if Text Component was removed or doesn't exist
            if (text == null)
            {
                return;
            }

            // Show if within range
            float currentDistance = Vector3.Distance(transform.position, mainCam.position);
            bool showRings;
            if (currentDistance <= maxShowDistance)
            {
                showRings = true;
            }
            else
            {
                showRings = false;
            }

            // Animate ring opacity in / out
            if (showRings)
            {
                lookAtPlayer.enabled = true;
                canvas.enabled = true;
                canvas.transform.LookAt(mainCam);

                _currentOpacity += Time.deltaTime * RingFadeSpeed;
                if (_currentOpacity > _initalOpacity)
                {
                    _currentOpacity = _initalOpacity;
                }

                Color colorCurrent = text.color;
                colorCurrent.a = _currentOpacity;
                text.color = colorCurrent;
            }
            else
            {
                lookAtPlayer.enabled = false;
                _currentOpacity -= Time.deltaTime * RingFadeSpeed;
                if (_currentOpacity <= 0)
                {
                    _currentOpacity = 0;
                    canvas.enabled = false;
                }
                else
                {
                    canvas.enabled = true;
                    Color colorCurrent = text.color;
                    colorCurrent.a = _currentOpacity;
                    text.color = colorCurrent;
                }
            }
        }
    }
}

