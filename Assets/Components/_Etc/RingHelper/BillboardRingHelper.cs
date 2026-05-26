using UnityEngine;
using BNG;

/// <summary>
/// Shows a billboard ring at the grab point of a grabbable when within range.
/// Drop-in replacement for BNG RingHelper (world canvas + Text).
/// </summary>
public class BillboardRingHelper : MonoBehaviour
{
    static readonly int ColorId = Shader.PropertyToID("_Color");
    static readonly int OpacityId = Shader.PropertyToID("_Opacity");

    [Tooltip("The Grabbable Object to Observe")]
    public Grabbable grabbable;

    [Tooltip("(Optional) If specified, the ring helper will only be valid if this Grabpoint is the nearest on the grabbable object")]
    public GrabPoint Grabpoint;

    [Tooltip("Default Color of the ring")]
    public Color RingColor = Color.white;

    [Tooltip("Color to use if selected by primary controller")]
    public Color RingSelectedColor = Color.white;

    [Tooltip("Color to use if selected by secondary controller")]
    public Color RingSecondarySelectedColor = Color.white;

    [Tooltip("Legacy canvas scaler value — higher = smaller ring. 1500 matches old default.")]
    public float ringSizeInRange = 1500f;

    [Tooltip("Legacy canvas scaler value when grabbable — lower = larger ring. 1100 matches old default.")]
    public float ringSizeGrabbable = 1100f;

    [Tooltip("Don't show grab rings if left and right controllers / grabbers are holding something")]
    public bool HideIfHandsAreFull = true;

    [Tooltip("How fast to lerp the opacity if being hidden / shown")]
    public float RingFadeSpeed = 5;

    MeshRenderer meshRenderer;
    Transform ringVisual;
    MaterialPropertyBlock propertyBlock;

    Grabber leftGrabber;
    Grabber rightGrabber;

    float initialOpacity;
    float currentOpacity;

    Transform mainCam;
    Vector3 baseRingScale = Vector3.one;

    void Start()
    {
        AssignCamera();

        if (grabbable == null)
        {
            grabbable = transform.parent.GetComponent<Grabbable>();
        }

        meshRenderer = GetComponentInChildren<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogWarning("No MeshRenderer found on BillboardRingHelper children.", this);
            return;
        }

        ringVisual = meshRenderer.transform;
        baseRingScale = ringVisual.localScale;
        propertyBlock = new MaterialPropertyBlock();

        initialOpacity = RingColor.a;
        currentOpacity = 0f;
        SetRendererVisible(false);

        AssignGrabbers();
    }

    void Update()
    {
        AssignCamera();

        if (meshRenderer == null || mainCam == null || grabbable == null)
        {
            return;
        }

        if (grabbable.BeingHeld || !grabbable.isActiveAndEnabled)
        {
            SetRendererVisible(false);
            return;
        }

        if (grabbable.OtherGrabbableMustBeGrabbed != null
            && !grabbable.OtherGrabbableMustBeGrabbed.BeingHeld)
        {
            SetRendererVisible(false);
            return;
        }

        bool handsFull = AreHandsFull();
        float distance = Vector3.Distance(transform.position, mainCam.position);
        bool showRing = BillboardRingHelperLogic.ShouldShowRing(
            handsFull,
            distance,
            grabbable.RemoteGrabDistance);

        if (!showRing)
        {
            FadeOut();
            return;
        }

        bool isClosest = grabbable.GetClosestGrabber() != null && grabbable.IsGrabbable();
        Grabber closestGrabber = grabbable.GetClosestGrabber();
        Color displayColor = BillboardRingHelperLogic.GetDisplayColor(
            isClosest,
            closestGrabber,
            RingColor,
            RingSelectedColor,
            RingSecondarySelectedColor);

        float legacySize = isClosest ? ringSizeGrabbable : ringSizeInRange;
        float uniformScale = BillboardRingHelperLogic.ScaleFromLegacyRingSize(legacySize);
        ringVisual.localScale = baseRingScale * uniformScale;

        FadeIn();
        ApplyVisuals(displayColor, currentOpacity);
    }

    bool AreHandsFull()
    {
        if (!HideIfHandsAreFull)
        {
            return false;
        }

        if (leftGrabber == null || rightGrabber == null)
        {
            return false;
        }

        if (leftGrabber.HoldingItem && rightGrabber.HoldingItem)
        {
            return true;
        }

        if (grabbable.GrabButton != GrabButton.Grip)
        {
            return false;
        }

        return !leftGrabber.FreshGrip && !rightGrabber.FreshGrip;
    }

    void FadeIn()
    {
        SetRendererVisible(true);
        currentOpacity = BillboardRingHelperLogic.StepFadeOpacity(
            currentOpacity,
            initialOpacity,
            RingFadeSpeed,
            Time.deltaTime,
            fadingIn: true);
    }

    void FadeOut()
    {
        currentOpacity = BillboardRingHelperLogic.StepFadeOpacity(
            currentOpacity,
            initialOpacity,
            RingFadeSpeed,
            Time.deltaTime,
            fadingIn: false);

        if (currentOpacity <= 0f)
        {
            currentOpacity = 0f;
            SetRendererVisible(false);
            return;
        }

        SetRendererVisible(true);
        ApplyVisuals(RingColor, currentOpacity);
    }

    void ApplyVisuals(Color displayColor, float opacity)
    {
        meshRenderer.GetPropertyBlock(propertyBlock);
        Color shaderColor = displayColor;
        shaderColor.a = 1f;
        propertyBlock.SetColor(ColorId, shaderColor);
        propertyBlock.SetFloat(OpacityId, opacity);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    void SetRendererVisible(bool visible)
    {
        meshRenderer.enabled = visible;
    }

    public virtual void AssignCamera()
    {
        if (mainCam != null)
        {
            return;
        }

        GameObject mainCameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCameraObject == null)
        {
            return;
        }

        mainCam = mainCameraObject.transform;
    }

    public virtual void AssignGrabbers()
    {
        Grabber[] grabs;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            grabs = player.GetComponentsInChildren<Grabber>();
        }
        else
        {
            grabs = FindObjectsOfType<Grabber>();
        }

        for (int i = 0; i < grabs.Length; i++)
        {
            Grabber grabber = grabs[i];
            if (grabber.HandSide == ControllerHand.Left)
            {
                leftGrabber = grabber;
            }
            else if (grabber.HandSide == ControllerHand.Right)
            {
                rightGrabber = grabber;
            }
        }
    }
}

/// <summary>
/// Pure helpers for billboard ring visibility (unit tested).
/// </summary>
public static class BillboardRingHelperLogic
{
    const float LegacyReferenceSize = 1500f;
    const float BaseRingScale = 0.15f;

    public static bool ShouldShowRing(bool handsFull, float distanceToCamera, float remoteGrabDistance)
    {
        if (handsFull)
        {
            return false;
        }

        return distanceToCamera <= remoteGrabDistance;
    }

    public static Color GetDisplayColor(
        bool isClosestGrabbable,
        Grabber closestGrabber,
        Color ringColor,
        Color ringSelectedColor,
        Color ringSecondarySelectedColor)
    {
        if (!isClosestGrabbable)
        {
            return ringColor;
        }

        if (closestGrabber != null && closestGrabber.HandSide == ControllerHand.Left)
        {
            return ringSecondarySelectedColor;
        }

        return ringSelectedColor;
    }

    public static float StepFadeOpacity(
        float currentOpacity,
        float targetOpacity,
        float fadeSpeed,
        float deltaTime,
        bool fadingIn)
    {
        if (fadingIn)
        {
            float next = currentOpacity + deltaTime * fadeSpeed;
            return Mathf.Min(next, targetOpacity);
        }

        float faded = currentOpacity - deltaTime * fadeSpeed;
        return Mathf.Max(faded, 0f);
    }

    public static float ScaleFromLegacyRingSize(float legacyRingSize)
    {
        if (legacyRingSize <= 0f)
        {
            return BaseRingScale;
        }

        return BaseRingScale * (LegacyReferenceSize / legacyRingSize);
    }
}
