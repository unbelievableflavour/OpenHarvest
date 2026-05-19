using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BNG;

public class ItemInspectorFader : MonoBehaviour
{
    public List<ControllerBinding> Input = new List<ControllerBinding>() { ControllerBinding.None };
    public Text tooltipText;
    public GameObject tooltipLine;
    public Canvas canvas;    
    public ItemInspector tooltip;

    [Header("Filled during play")]
    public Grabber grabber;

    Grabbable grabbable;

    bool tooltipCurrentlyShown = false;
    HarvestDataTypes.Item item;
    
    void Start()
    {
        canvas.enabled = false;
        tooltipLine.SetActive(false);
    }

    void Update()
    {
        if (!ifKeyDown()) {
            FadeTooltip(false);
            return;
        }

        if (grabber.HeldGrabbable == null) {
            FadeTooltip(false);
            return;
        }

        if(tooltip.DrawLineTo != grabber.HeldGrabbable.transform) {
            Init(grabber.HeldGrabbable);
        }

        FadeTooltip(true);
    }

    public void Init(Grabbable heldGrabbable) {
        //if it is a subgrab, grab the parent grab.
        item = heldGrabbable.OtherGrabbableMustBeGrabbed 
        ? Definitions.GetItemFromObject(heldGrabbable.OtherGrabbableMustBeGrabbed) : Definitions.GetItemFromObject(heldGrabbable);

        //if no item can be found.
        if (item == null) {
            return;
        }

        tooltip.Init(heldGrabbable.transform);
        tooltipText.text = item.name + "\n\n" + item.description;
    }

    public void FadeTooltip(bool showTooltip) {
        if(showTooltip == tooltipCurrentlyShown) {
            return;
        }

        if (item == null) {
            return;
        }

        // Animate ring opacity in / out
        if (showTooltip)
        {
            tooltip.StartUpdating();
            tooltipCurrentlyShown = true;
            tooltipLine.SetActive(true);
            canvas.enabled = true;
            return;
        }
        
        tooltipCurrentlyShown = false;
        tooltip.StopUpdating();
        canvas.enabled = false;
        tooltipLine.SetActive(false);
        return;
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