using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class OpenHarvestFloatEvent : UnityEvent<float> { }

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class OpenHarvestHingeHelper : MonoBehaviour
{
    [Header("Snap Options")]
    [Tooltip("If true, SnapGraphics local Y rotation snaps to nearest SnapDegrees.")]
    public bool SnapToDegrees = false;

    [Tooltip("Snap rotation to nearest step.")]
    public float SnapDegrees = 5f;

    [Tooltip("Transform to rotate when snapping.")]
    public Transform SnapGraphics;

    [Tooltip("Play this sound on snap.")]
    public AudioClip SnapSound;

    [Tooltip("Randomize pitch amount for SnapSound.")]
    public float RandomizePitch = 0.001f;

    [Header("Text Label (Optional)")]
    public Text LabelToUpdate;

    [Header("Change Events")]
    public OpenHarvestFloatEvent onHingeChange;
    public OpenHarvestFloatEvent onHingeSnapChange;

    private Rigidbody rigid;
    private int touchingCount;
    private float lastDegrees;
    private float lastSnapDegrees;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!ShouldProcess())
        {
            return;
        }

        float degrees = GetSmoothedValue(transform.localEulerAngles.y);
        if (!Mathf.Approximately(degrees, lastDegrees))
        {
            OnHingeChange(degrees);
        }

        lastDegrees = degrees;

        float nearestSnap = degrees;
        if (SnapDegrees > 0.0001f)
        {
            nearestSnap = GetSmoothedValue(Mathf.Round(degrees / SnapDegrees) * SnapDegrees);
        }

        if (SnapToDegrees && SnapDegrees > 0.0001f)
        {
            if (!Mathf.Approximately(nearestSnap, lastSnapDegrees))
            {
                OnSnapChange(nearestSnap);
            }

            lastSnapDegrees = nearestSnap;
        }

        if (LabelToUpdate != null)
        {
            float val = GetSmoothedValue(SnapToDegrees ? nearestSnap : degrees);
            LabelToUpdate.text = val.ToString("n0");
        }
    }

    private bool ShouldProcess()
    {
        if (touchingCount > 0)
        {
            return true;
        }

        if (rigid == null)
        {
            rigid = GetComponent<Rigidbody>();
        }

        if (rigid == null)
        {
            return false;
        }

        if (!rigid.isKinematic && !rigid.IsSleeping())
        {
            return true;
        }

        return rigid.angularVelocity.sqrMagnitude > 0.0001f;
    }

    public void OnSnapChange(float yAngle)
    {
        if (SnapGraphics != null)
        {
            SnapGraphics.localEulerAngles = new Vector3(
                SnapGraphics.localEulerAngles.x,
                yAngle,
                SnapGraphics.localEulerAngles.z);
        }

        if (SnapSound != null)
        {
            AudioSource.PlayClipAtPoint(SnapSound, transform.position, 1f);
        }

        onHingeSnapChange?.Invoke(yAngle);
    }

    public void OnHingeChange(float hingeAmount)
    {
        onHingeChange?.Invoke(hingeAmount);
    }

    private static float GetSmoothedValue(float val)
    {
        if (val < 0)
        {
            val = 360 - val;
        }

        if (Mathf.Approximately(val, 360f))
        {
            val = 0f;
        }

        return val;
    }

    private void OnCollisionEnter(Collision collision)
    {
        touchingCount++;
    }

    private void OnCollisionExit(Collision collision)
    {
        touchingCount = Mathf.Max(0, touchingCount - 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        touchingCount++;
    }

    private void OnTriggerExit(Collider other)
    {
        touchingCount = Mathf.Max(0, touchingCount - 1);
    }
}
