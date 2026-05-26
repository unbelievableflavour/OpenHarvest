using UnityEngine;
using BNG;

/// <summary>
/// Shows a billboard ring at the grab point of a grabbable when within range.
/// Drop-in replacement for BNG RingHelper (world canvas + Text).
/// </summary>
public class BillboardRingHelper : MonoBehaviour
{
    static readonly int WorldHalfExtentId = Shader.PropertyToID("_WorldHalfExtent");
    static readonly int OpacityId = Shader.PropertyToID("_Opacity");

    [Tooltip("The Grabbable Object to Observe")]
    public Grabbable grabbable;

    [Tooltip("(Optional) If specified, the ring helper will only be valid if this Grabpoint is the nearest on the grabbable object")]
    public GrabPoint Grabpoint;

    [Tooltip("How fast to lerp the opacity if being hidden / shown")]
    public float RingFadeSpeed = 5;

    MeshRenderer meshRenderer;
    MaterialPropertyBlock propertyBlock;

    Grabber leftGrabber;
    Grabber rightGrabber;

    float baseWorldHalfExtent;
    float maxOpacity;
    float currentOpacity;

    Transform mainCam;

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

        propertyBlock = new MaterialPropertyBlock();
        CacheMaterialDefaults(meshRenderer.sharedMaterial);

        currentOpacity = 0f;
        SetRendererVisible(false);

        AssignGrabbers();
    }

    void CacheMaterialDefaults(Material material)
    {
        if (material == null)
        {
            baseWorldHalfExtent = 0.1f;
            maxOpacity = 1f;
            return;
        }

        baseWorldHalfExtent = material.HasProperty(WorldHalfExtentId)
            ? material.GetFloat(WorldHalfExtentId)
            : 0.1f;
        maxOpacity = material.HasProperty(OpacityId)
            ? material.GetFloat(OpacityId)
            : material.color.a;
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
            FadeOut(false);
            return;
        }

        bool isTargetedGrabbable = BillboardRingHelperLogic.IsTargetedGrabbable(grabbable);
        FadeIn(isTargetedGrabbable);
    }

    bool AreHandsFull()
    {
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

    void FadeIn(bool isTargetedGrabbable)
    {
        SetRendererVisible(true);
        currentOpacity = BillboardRingHelperLogic.StepFadeOpacity(
            currentOpacity,
            maxOpacity,
            RingFadeSpeed,
            Time.deltaTime,
            fadingIn: true);
        ApplyVisuals(currentOpacity, isTargetedGrabbable);
    }

    void FadeOut(bool isTargetedGrabbable)
    {
        currentOpacity = BillboardRingHelperLogic.StepFadeOpacity(
            currentOpacity,
            maxOpacity,
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
        ApplyVisuals(currentOpacity, isTargetedGrabbable);
    }

    void ApplyVisuals(float opacity, bool isTargetedGrabbable)
    {
        meshRenderer.GetPropertyBlock(propertyBlock);
        float extent = BillboardRingHelperLogic.GetWorldHalfExtent(
            baseWorldHalfExtent,
            isTargetedGrabbable);
        propertyBlock.SetFloat(WorldHalfExtentId, extent);
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
    const float TargetedExtentMultiplier = 1.12f;

    public static bool ShouldShowRing(bool handsFull, float distanceToCamera, float remoteGrabDistance)
    {
        if (handsFull)
        {
            return false;
        }

        return distanceToCamera <= remoteGrabDistance;
    }

    public static bool IsTargetedGrabbable(Grabbable grabbable)
    {
        if (grabbable == null)
        {
            return false;
        }

        return grabbable.GetClosestGrabber() != null && grabbable.IsGrabbable();
    }

    public static float GetWorldHalfExtent(float baseExtent, bool isTargetedGrabbable)
    {
        if (!isTargetedGrabbable)
        {
            return baseExtent;
        }

        return baseExtent * TargetedExtentMultiplier;
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
}
