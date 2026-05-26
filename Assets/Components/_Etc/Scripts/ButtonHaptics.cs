using BNG;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHaptics : MonoBehaviour, IPointerEnterHandler
{
    private float VibrateFrequency = 0.5f;
    private float VibrateAmplitude = 2f;
    private float VibrateDuration = 0.02f;

    protected InputBridge input;

    private void Awake()
    {
        //input = InputBridge.Instance;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //NOT WORKING NOW ANYWAYSSS

        return;
        var defaultButton = GetComponent<UnityEngine.UI.Button>();
        if (defaultButton)
        {
            if (!defaultButton.interactable)
            {
                return;
            }
        }

        //doHaptics(ControllerHand.Left);
        //doHaptics(ControllerHand.Right);
    }

    void doHaptics(ControllerHand touchingHand)
    {
        if (input)
        {
        //    input.VibrateController(VibrateFrequency, VibrateAmplitude, VibrateDuration, touchingHand);
        }
    }
}
