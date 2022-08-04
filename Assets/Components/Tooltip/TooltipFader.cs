using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BNG
{
    /// <summary>
    /// Shows a tooltip while holding down a button
    /// </summary>
    public class TooltipFader : MonoBehaviour
    {
        public List<ControllerBinding> Input = new List<ControllerBinding>() { ControllerBinding.None };
        public Text tooltipText;
        public GameObject tooltipLine;

        /// <summary>
        /// How fast to lerp the opacity if being hidden / shown
        /// </summary>
        public float RingFadeSpeed = 5;
        public float maxShowDistance = 2f;

        Canvas canvas;
        Image image;
        Grabbable grabbable;

        // Animate opacity
        private float _initialOpacity;
        private float _currentOpacity;

        // Start is called before the first frame update
        void Start()
        {
            grabbable = transform.parent.parent.GetComponent<Grabbable>();
            canvas = GetComponent<Canvas>();
            image = GetComponent<Image>();

            _initialOpacity = image.color.a;
            _currentOpacity = 0;
        }

        void Update()
        {
            // Bail if Image Component was removed or doesn't exist or no grabbable
            if (image == null || !grabbable)
            {
                return;
            }

            bool showRings;
            if (grabbable.BeingHeld && ifKeyDown())
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
                tooltipLine.SetActive(true);
                canvas.enabled = true;
                return;
                _currentOpacity += Time.deltaTime * RingFadeSpeed;
                if (_currentOpacity > _initialOpacity)
                {
                    _currentOpacity = _initialOpacity;
                }

                Color colorCurrent = image.color;
                colorCurrent.a = _currentOpacity;
                image.color = colorCurrent;

                Color colorTextCurrent = tooltipText.color;
                colorTextCurrent.a = _currentOpacity;
                tooltipText.color = colorTextCurrent;
            }
            else
            {

                //TMP
                canvas.enabled = false;
                tooltipLine.SetActive(false);
                return;

                //END TMP
                tooltipLine.SetActive(false);
                _currentOpacity -= Time.deltaTime * RingFadeSpeed;
                if (_currentOpacity <= 0)
                {
                    _currentOpacity = 0;
                    canvas.enabled = false;
                }
                else
                {
                    canvas.enabled = true;
                    Color colorCurrent = image.color;
                    colorCurrent.a = _currentOpacity;
                    image.color = colorCurrent;

                    Color colorTextCurrent = tooltipText.color;
                    colorTextCurrent.a = _currentOpacity;
                    tooltipText.color = colorTextCurrent;
                }
            }
        }

        public virtual bool ifKeyDown()
        {
            // Check for bound controller button
            for (int x = 0; x < Input.Count; x++)
            {
                if (InputBridge.Instance.GetControllerBindingValue(Input[x]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

