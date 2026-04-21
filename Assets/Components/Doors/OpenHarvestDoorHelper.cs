using BNG;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class OpenHarvestDoorHelper : MonoBehaviour
{
    public AudioClip DoorOpenSound;
    public AudioClip DoorCloseSound;

    [Tooltip("Does the handle need to be turned in order to open the door from the closed position?")]
    public bool RequireHandleTurnToOpen = false;

    [Tooltip("This transform is used to determine how many degrees have been turned. Required if RequireHandleTurnToOpen is true.")]
    public Transform HandleFollower;

    public float DegreesTurned;

    [Tooltip("How many degrees the handle must turn in order for the latch to be open.")]
    public float DegreesTurnToOpen = 10f;

    [Tooltip("Rotate this transform with Handle Rotation.")]
    public Transform DoorLockTransform;

    public float AngularVelocitySnapDoor = 0.2f;

    public float angle;
    public float AngularVelocity = 0.2f;

    [Tooltip("If true the door will not respond to user input.")]
    public bool DoorIsLocked = false;

    public float lockPos;

    private Rigidbody rigid;
    private Transform tf;
    private float initialLockPosition;

    private bool handleLocked;
    private bool playedOpenSound;
    private bool readyToPlayCloseSound;

    // Change-detection for fast-path skip.
    private float lastHandleLocalY;
    private const float HandleMovementEpsilon = 0.05f;

    // Cache-for-GC (kept for parity with the original API).
    private Vector3 currentRotation;
    private float moveLockAmount, rotateAngles, ratio;

    private void Awake()
    {
        tf = transform;
        rigid = GetComponent<Rigidbody>();

        if (DoorLockTransform != null)
        {
            initialLockPosition = DoorLockTransform.localPosition.x;
        }
    }

    private void Update()
    {
        bool atRest = rigid.isKinematic || rigid.IsSleeping();
        if (atRest && !HandleIsBeingTurned())
        {
            return;
        }

        UpdateAngleAndVelocity();
        UpdateOpenCloseSounds();
        SnapShutIfNearlyClosed();
        UpdateHandleAndLock();
        UpdateKinematicState();
    }

    private bool HandleIsBeingTurned()
    {
        if (HandleFollower == null)
        {
            return false;
        }

        float y = HandleFollower.localEulerAngles.y;
        bool moved = Mathf.Abs(Mathf.DeltaAngle(y, lastHandleLocalY)) > HandleMovementEpsilon;
        lastHandleLocalY = y;
        return moved;
    }

    private void UpdateAngleAndVelocity()
    {
        AngularVelocity = rigid.angularVelocity.magnitude;

        currentRotation = tf.localEulerAngles;
        angle = Mathf.Floor(currentRotation.y);

        if (angle >= 180)
        {
            angle -= 180;
        }
        else
        {
            angle = 180 - angle;
        }
    }

    private void UpdateOpenCloseSounds()
    {
        if (angle > 10f && !playedOpenSound)
        {
            if (DoorOpenSound != null && VRUtils.Instance != null)
            {
                VRUtils.Instance.PlaySpatialClipAt(DoorOpenSound, tf.position, 1f, 1f);
            }
            playedOpenSound = true;
        }

        if (angle > 30f)
        {
            readyToPlayCloseSound = true;
        }

        if (angle < 2f && playedOpenSound)
        {
            playedOpenSound = false;
        }

        if (readyToPlayCloseSound && angle < 2f)
        {
            if (DoorCloseSound != null && VRUtils.Instance != null)
            {
                VRUtils.Instance.PlaySpatialClipAt(DoorCloseSound, tf.position, 1f, 1f);
            }
            readyToPlayCloseSound = false;
        }
    }

    private void SnapShutIfNearlyClosed()
    {
        if (rigid.isKinematic)
        {
            return;
        }

        if (angle < 1f && AngularVelocity <= AngularVelocitySnapDoor)
        {
            rigid.angularVelocity = Vector3.zero;
        }
    }

    private void UpdateHandleAndLock()
    {
        if (HandleFollower != null)
        {
            DegreesTurned = Mathf.Abs(HandleFollower.localEulerAngles.y - 270f);
        }

        if (DoorLockTransform != null)
        {
            moveLockAmount = 0.025f;
            rotateAngles = 55f;
            ratio = rotateAngles / (rotateAngles - Mathf.Clamp(DegreesTurned, 0f, rotateAngles));
            lockPos = initialLockPosition - (ratio * moveLockAmount) + moveLockAmount;
            lockPos = Mathf.Clamp(lockPos, initialLockPosition - moveLockAmount, initialLockPosition);

            Vector3 lp = DoorLockTransform.localPosition;
            lp.x = lockPos;
            DoorLockTransform.localPosition = lp;
        }

        if (RequireHandleTurnToOpen)
        {
            handleLocked = DegreesTurned < DegreesTurnToOpen;
        }
    }

    private void UpdateKinematicState()
    {
        if (angle < 0.02f && (handleLocked || DoorIsLocked))
        {
            if (rigid.collisionDetectionMode == CollisionDetectionMode.Continuous
                || rigid.collisionDetectionMode == CollisionDetectionMode.ContinuousDynamic)
            {
                rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            }

            if (!rigid.isKinematic)
            {
                rigid.isKinematic = true;
            }
        }
        else
        {
            if (rigid.collisionDetectionMode == CollisionDetectionMode.ContinuousSpeculative)
            {
                rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }

            if (rigid.isKinematic)
            {
                rigid.isKinematic = false;
            }
        }
    }
}
